
using ExitGames.Client.Photon;
using Fusion;
using GorillaNetworking;
using GorillaTag;
using HarmonyLib;
using J0kersTrollMenu.ModMenu;
using J0kersTrollMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.Internal;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace J0kersTrollMenu.MenuMods
{
    internal class Mods : MonoBehaviour // Cleaned Up A Bit =)
    {
        // Out bc its in like all the regions
        static VRRig VrRigPlayers = null;
        static bool CopyPlayer;
        static GameObject GunSphere;

        #region Get Stuff

        public static VRRig GetVRRigFromPlayer(Player p)
        {
            return GorillaGameManager.instance.FindPlayerVRRig(p);
        }


        public static VRRig FindVRRigForPlayer(Photon.Realtime.Player player)
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isOfflineVRRig && vrrig.GetComponent<PhotonView>().Owner == player)
                {
                    return vrrig;
                }
            }
            return null;
        }

        public static VRRig GetClosestVRRig()
        {
            float num = float.MaxValue;
            VRRig outRig = null;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position) < num && vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    num = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position);
                    outRig = vrrig;
                }
            }
            return outRig;
        }

        public static MonkeyeAI[] monkeyeAI = null;

        static float Timer = 0f;
        public static MonkeyeAI[] FindMonkeyes()
        {
            if (Time.time > Timer)
            {
                monkeyeAI = null;
                Timer = Time.time + 5f;
            }
            if (monkeyeAI == null)
            {
                monkeyeAI = UnityEngine.Object.FindObjectsOfType<MonkeyeAI>();
            }
            return monkeyeAI;
        }

        public static void GetOwnership(PhotonView view) // Cred IIDK <3
        {
            if (!view.AmOwner)
            {
                try
                {
                    view.OwnershipTransfer = OwnershipOption.Request;
                    view.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                    view.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
                    view.RequestOwnership();
                    view.TransferOwnership(PhotonNetwork.LocalPlayer);

                    RequestableOwnershipGuard rog = view.GetComponent<RequestableOwnershipGuard>();
                    if (rog != null)
                    {
                        view.GetComponent<RequestableOwnershipGuard>().actualOwner = PhotonNetwork.LocalPlayer;
                        view.GetComponent<RequestableOwnershipGuard>().currentOwner = PhotonNetwork.LocalPlayer;
                        view.GetComponent<RequestableOwnershipGuard>().RequestTheCurrentOwnerFromAuthority();
                        view.GetComponent<RequestableOwnershipGuard>().TransferOwnership(PhotonNetwork.LocalPlayer);
                        view.GetComponent<RequestableOwnershipGuard>().TransferOwnershipFromToRPC(PhotonNetwork.LocalPlayer, view.GetComponent<RequestableOwnershipGuard>().ownershipRequestNonce, default(PhotonMessageInfo));
                    }
                    RpcCleanUp();
                }
                catch { UnityEngine.Debug.Log("Faliure to get ownership, is the PhotonView valid?"); }
            }
            else
            {
                view.OwnershipTransfer = OwnershipOption.Fixed;
            }
        }

        #endregion

        #region Gorilla Tagger Mods

        static float beesDelay;

        static void BallsOnHands()
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.Destroy(gameObject.GetComponent<Rigidbody>());
            Object.Destroy(gameObject.GetComponent<SphereCollider>());
            gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            gameObject.transform.position = GorillaTagger.Instance.leftHandTransform.position;
            GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.Destroy(gameObject2.GetComponent<Rigidbody>());
            Object.Destroy(gameObject2.GetComponent<SphereCollider>());
            gameObject2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            gameObject2.transform.position = GorillaTagger.Instance.rightHandTransform.position;
            gameObject.GetComponent<Renderer>().material.color = Color.black;
            gameObject2.GetComponent<Renderer>().material.color = Color.white;
            Object.Destroy(gameObject, Time.deltaTime);
            Object.Destroy(gameObject2, Time.deltaTime);
        }

        static void LineToRig()
        {
            GameObject gameObject = new GameObject("Line");
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.black;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.SetPosition(0, GorillaLocomotion.Player.Instance.rightControllerTransform.position);
            lineRenderer.SetPosition(1, GorillaTagger.Instance.offlineVRRig.transform.position);
            lineRenderer.material.shader = Shader.Find("GUI/Text Shader");
            UnityEngine.Object.Destroy(lineRenderer, Time.deltaTime);
            UnityEngine.Object.Destroy(gameObject, Time.deltaTime);
        }

        public static void LongArms()
        {
            GorillaTagger.Instance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }

        public static void LongArmsFix()
        {
            GorillaTagger.Instance.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public static void FixHead()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x = 0f;
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = 0f;
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 0f;
        }

        public static void FlyAtPlayer()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, -GorillaTagger.Instance.rightHandTransform.up, out var Ray);
                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = Color.white;
                NewPointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                NewPointer.transform.position = CopyPlayer ? VrRigPlayers.transform.position : Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = Color.white;
                liner.endColor = Color.black;
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, CopyPlayer ? VrRigPlayers.transform.position : Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                if (CopyPlayer && VrRigPlayers != null)
                {
                    BallsOnHands();
                    GorillaTagger.Instance.offlineVRRig.enabled = false;

                    Vector3 look = VrRigPlayers.transform.position - GorillaTagger.Instance.offlineVRRig.transform.position;
                    look.Normalize();

                    Vector3 position = GorillaTagger.Instance.offlineVRRig.transform.position + (look * (30f * Time.deltaTime));

                    GorillaTagger.Instance.offlineVRRig.transform.position = position;
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.position = position;
                    }
                    catch { }

                    GorillaTagger.Instance.offlineVRRig.transform.LookAt(VrRigPlayers.transform.position);
                    try
                    {
                        GorillaTagger.Instance.myVRRig.transform.LookAt(VrRigPlayers.transform.position);
                    }
                    catch { }

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * -1f);
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 1f);

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                }
                if (Plugin.CalculateGrabState(ControllerInputPoller.instance.rightControllerIndexFloat, 0.1f))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        CopyPlayer = true;
                        VrRigPlayers = possibly;
                    }
                }
            }
            else
            {
                if (CopyPlayer)
                {
                    CopyPlayer = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void CopyMovement()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, -GorillaTagger.Instance.rightHandTransform.up, out var Ray);
                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = Color.white;
                NewPointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                NewPointer.transform.position = CopyPlayer ? VrRigPlayers.transform.position : Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = Color.white;
                liner.endColor = Color.black;
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, CopyPlayer ? VrRigPlayers.transform.position : Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                if (CopyPlayer && VrRigPlayers != null)
                {
                    BallsOnHands();
                    GorillaTagger.Instance.offlineVRRig.enabled = false;

                    GorillaTagger.Instance.offlineVRRig.transform.position = VrRigPlayers.transform.position;
                    GorillaTagger.Instance.offlineVRRig.transform.rotation = VrRigPlayers.transform.rotation;

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.position = VrRigPlayers.head.rigTarget.transform.position;
                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = VrRigPlayers.head.rigTarget.transform.rotation;

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = VrRigPlayers.leftHandTransform.transform.position;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = VrRigPlayers.rightHandTransform.transform.position;

                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = VrRigPlayers.leftHandTransform.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = VrRigPlayers.rightHandTransform.transform.rotation;
                }
                if (Plugin.CalculateGrabState(ControllerInputPoller.instance.rightControllerIndexFloat, 0.1f))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        CopyPlayer = true;
                        VrRigPlayers = possibly;
                    }
                }
            }
            else
            {
                if (CopyPlayer)
                {
                    CopyPlayer = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void RigGun()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo);
                GunSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GunSphere.transform.position = hitinfo.point;
                GunSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                GunSphere.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                GunSphere.GetComponent<Renderer>().material.color = Color.white;
                GameObject.Destroy(GunSphere.GetComponent<BoxCollider>());
                GameObject.Destroy(GunSphere.GetComponent<Rigidbody>());
                GameObject.Destroy(GunSphere.GetComponent<Collider>());

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = Color.white;
                liner.endColor = Color.black;
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, GunSphere.transform.position);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                if (Plugin.CalculateGrabState(ControllerInputPoller.instance.rightControllerIndexFloat, 0.1f))
                {
                    GameObject.Destroy(GunSphere, Time.deltaTime);
                    GunSphere.GetComponent<Renderer>().material.color = Color.black;
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * -1f);
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 1f);
                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                                    // Fix For The hands rotation
                Quaternion handRotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;

                Quaternion leftHandRotation = handRotation * Quaternion.Euler(0f, -1f, 0f) * Quaternion.Euler(0f, 0f, 80f); // 80f bc 90f makes them go down a bit
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = leftHandRotation;

                Quaternion rightHandRotation = handRotation * Quaternion.Euler(0f, 1f, 0f) * Quaternion.Euler(0f, 0f, -80f); // ^^ Same here but negative ^^
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = rightHandRotation;
                    GorillaTagger.Instance.offlineVRRig.transform.position = GunSphere.transform.position + new Vector3(0f, 1f, 0f);
                }

            }
            if (GunSphere != null)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                GameObject.Destroy(GunSphere, Time.deltaTime);
            }
        }

        public static void Bees()
        {
            if (Plugin.CalculateGrabState(ControllerInputPoller.instance.rightControllerIndexFloat, 0.1f))
            {
                BallsOnHands();
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                LineToRig();

                if (Time.time > Mods.beesDelay)
                {
                    VRRig vrrig4 = GorillaParent.instance.vrrigs[Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
                    GorillaTagger.Instance.offlineVRRig.transform.position = vrrig4.transform.position + new Vector3(0f, 1f, 0f);
                    GorillaTagger.Instance.myVRRig.transform.position = vrrig4.transform.position + new Vector3(0f, 1f, 0f);
                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = vrrig4.transform.position;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = vrrig4.transform.position;
                    Mods.beesDelay = Time.time + 0.777f;
                }
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void Ghost()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                BallsOnHands();
                LineToRig();
                GorillaTagger.Instance.offlineVRRig.enabled = false;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void GhostTPose()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                BallsOnHands();
                LineToRig();
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * -1f);
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 1f);
                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;

                // Fix For The hands rotation
                Quaternion handRotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;

                Quaternion leftHandRotation = handRotation * Quaternion.Euler(0f, -1f, 0f) * Quaternion.Euler(0f, 0f, 80f); // 80f bc 90f makes them go down a bit
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation = leftHandRotation;

                Quaternion rightHandRotation = handRotation * Quaternion.Euler(0f, 1f, 0f) * Quaternion.Euler(0f, 0f, -80f); // ^^ Same here but negative ^^
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation = rightHandRotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void Invis()
        {
            if (Plugin.CalculateGrabState(ControllerInputPoller.instance.rightControllerIndexFloat, 0.1f))
            {
                BallsOnHands();
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(100000f, 100000f, 100000f);
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        #endregion

        #region Speed

        public static void Speed()
        {
            GorillaLocomotion.Player.Instance.maxJumpSpeed = 9.8f; // Fix To Your Settings
        }

        public static void SpeedFix()
        {
            GorillaLocomotion.Player.Instance.maxJumpSpeed = 6.5f; // Resets Speed
        }

        #endregion

        #region Room Mods

        public static void GetInfo()
        {
            string text = "=======================Player IDs!=========================";

            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                if (PhotonNetwork.InRoom || PhotonNetwork.InLobby)
                {
                    string playerName = player.NickName;
                    string playerId = player.UserId;
                    string roomId = PhotonNetwork.CurrentRoom.Name;
                    string isMaster = player.IsMasterClient.ToString();

                    text += $"\nPlayer Name: {playerName}, Player ID: {playerId}, Room ID: {roomId}, Master: {isMaster}";
                }
            }

            text += "\n==========================================================\n";
            File.AppendAllText("PLAYER INFO.txt", text);
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/motd/motdtext").GetComponent<Text>().text = "PLAYFAB ID: " + PlayFabSettings.TitleId.ToString() + "\nPhoton Real Time: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime.ToString() + "\nPhoton Voice: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice.ToString() + "\nRegion: " + PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion + "\nSDK Version: " + PlayFabSettings.VersionString.ToString() + "\nAUTH URL: " + PlayFabAuthenticator.Playfab_Auth_API + "\nGAME VERSION: " + GorillaComputer.instance.version + "\nMASTER: " + PhotonNetwork.MasterClient;
            NotifiLib.SendNotification("<color=green>INFO PULLED</color> <color=gray>CHECK GTAG FILE AND MOTD</color>");
        }
        public static float notiDelay = 0f;

        public static void RandomGhostCode()
        {
            string[] roomNames =
            {
                "666",
                "DAISY09",
                "PBBV",
                "SREN17",
                "SREN18",
                "AI",
                "GHOST",
                "J3VU",
                "RUN",
                "BOT",
                "TIPTOE",
                "SPIDER",
                "STATUE",
                "BANSHEE",
                "RABBIT",
                "ERROR",
                "ISEEYOUBAN"
            };
            int num = new System.Random().Next(roomNames.Length);
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomNames[num], JoinType.Solo);
        }

        /* 
         Sorry Got Rid Of The Other Ones Just Taking Up Space 
         But If You Want The Code Here It Is: 

         PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("Room Name Here", JoinType.Solo);
        */
        #endregion

        #region Plats

        private static GameObject PlatFL;
        private static GameObject PlatFR;
        private static bool PlatLSpawn;
        private static bool PlatRpawn;

        public static void PlatL()
        {
            // Plat L Game OBJS
            PlatFL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(PlatFL.GetComponent<Rigidbody>());
            Object.Destroy(PlatFL.GetComponent<BoxCollider>());
            Object.Destroy(PlatFL.GetComponent<Renderer>());
            PlatFL.transform.localScale = new Vector3(0.25f, 0.3f, 0.25f);

            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.transform.parent = PlatFL.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.1f, 1f, 1f);
            gameObject.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            gameObject.GetComponent<Renderer>().material.color = Color.black;
            gameObject.transform.position = new Vector3(0.02f, 0f, 0f);
        }

        public static void PlatR()
        {
            // Plat R Game OBJS
            PlatFR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(PlatFR.GetComponent<Rigidbody>());
            Object.Destroy(PlatFR.GetComponent<BoxCollider>());
            Object.Destroy(PlatFR.GetComponent<Renderer>());

            PlatFR.transform.localScale = new Vector3(0.25f, 0.3f, 0.25f);
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.transform.parent = PlatFR.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.1f, 1f, 1f);
            gameObject.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            gameObject.GetComponent<Renderer>().material.color = Color.white;
            gameObject.transform.position = new Vector3(-0.02f, 0f, 0f);
        }

        public static void Platforms()
        {
            List<UnityEngine.XR.InputDevice> list = new List<UnityEngine.XR.InputDevice>();

            #region Quest
            if (ControllerInputPoller.instance.controllerType == GorillaControllerType.OCULUS_DEFAULT)
            {
                if (ControllerInputPoller.instance.leftControllerSecondaryButton && PlatFL == null)
                {
                    PlatL();
                }
                if (ControllerInputPoller.instance.rightControllerSecondaryButton && PlatFR == null)
                {
                    PlatR();
                }

                // Tansform Game Obj To Hands
                if (ControllerInputPoller.instance.leftControllerSecondaryButton && PlatFL != null && !PlatLSpawn)
                {
                    PlatFL.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                    PlatFL.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                    PlatLSpawn = true;
                }
                if (ControllerInputPoller.instance.rightControllerSecondaryButton && PlatFR != null && !PlatRpawn)
                {
                    PlatFR.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
                    PlatFR.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
                    PlatRpawn = true;
                }

                // Apply Rig Id Body for falling and destroy on null
                if (!ControllerInputPoller.instance.leftControllerSecondaryButton && PlatFL != null)
                {
                    GameObject.Destroy(PlatFL);
                    PlatFL = null;
                    PlatLSpawn = false;
                }
                if (!ControllerInputPoller.instance.rightControllerSecondaryButton && PlatFR != null)
                {
                    GameObject.Destroy(PlatFR);
                    PlatFR = null;
                    PlatRpawn = false;
                }
            }
            #endregion

            #region Index

            if (ControllerInputPoller.instance.controllerType == GorillaControllerType.INDEX)
            {
                if (ControllerInputPoller.instance.leftControllerSecondaryButton && PlatFL == null)
                {
                    PlatL();
                }
                if (ControllerInputPoller.instance.rightControllerSecondaryButton && PlatFR == null)
                {
                    PlatR();
                }

                // Tansform Game Obj To Hands
                if (ControllerInputPoller.instance.leftControllerSecondaryButton && PlatFL != null && !PlatLSpawn)
                {
                    PlatFL.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                    PlatFL.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                    PlatLSpawn = true;
                }
                if (ControllerInputPoller.instance.rightControllerSecondaryButton && PlatFR != null && !PlatRpawn)
                {
                    PlatFR.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
                    PlatFR.transform.rotation = GorillaLocomotion.Player.Instance.rightControllerTransform.rotation;
                    PlatRpawn = true;
                }

                // Apply Rig Id Body for falling and destroy on null
                if (!ControllerInputPoller.instance.leftControllerSecondaryButton && PlatFL != null)
                {
                    GameObject.Destroy(PlatFL);
                    PlatFL = null;
                    PlatLSpawn = false;
                }
                if (!ControllerInputPoller.instance.rightControllerSecondaryButton && PlatFR != null)
                {
                    GameObject.Destroy(PlatFR);
                    PlatFR = null;
                    PlatRpawn = false;
                }
            }

            #endregion
        }


        #endregion

        #region Player

        static bool bothHands;

        public static void FlightWithNoClip()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * 15;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                #region No Clip
                foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
                {
                    collider.enabled = false;
                }
            }
            else
            {
                foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
                {
                    collider.enabled = true;
                }
                #endregion
            }
        }

        public static void Rocket()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.transform.up * Time.deltaTime * 25;
                //GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero; Add this if you want no velocity
            }
        }

        public static void NoGrav()
        {
            GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.useGravity = false;
        }

        public static void NoGravOff()
        {
            GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.useGravity = true;
        }

        public static void AutoFunnyRun()
        {
            if (Plugin.CalculateGrabState(ControllerInputPoller.instance.rightControllerIndexFloat, 0.1f))
            {
                if (bothHands)
                {
                    float time = Time.frameCount;
                    GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * Mathf.Cos(time) / 10) + new Vector3(0, -0.5f - (Mathf.Sin(time) / 7), 0) + (GorillaTagger.Instance.headCollider.transform.right * -0.05f);
                    GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * Mathf.Cos(time + 180) / 10) + new Vector3(0, -0.5f - (Mathf.Sin(time + 180) / 7), 0) + (GorillaTagger.Instance.headCollider.transform.right * 0.05f);
                }
                else
                {
                    float time = Time.frameCount;
                    GorillaTagger.Instance.rightHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * Mathf.Cos(time) / 10) + new Vector3(0, -0.5f - (Mathf.Sin(time) / 7), 0);
                    GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * Mathf.Cos(time + 180) / 10) + new Vector3(0, -0.5f - (Mathf.Sin(time + 180) / 7), 0) + (GorillaTagger.Instance.headCollider.transform.right * 0.05f);
                }
            }
        }
        #endregion

        #region Safty
        public static bool Frame = false;
        private static float SplashCoolDown;
        private static float threshold = 0.35f;

        public static void NoFinger()
        {
            ControllerInputPoller.instance.leftControllerGripFloat = 0f;
            ControllerInputPoller.instance.rightControllerGripFloat = 0f;
            ControllerInputPoller.instance.leftControllerIndexFloat = 0f;
            ControllerInputPoller.instance.rightControllerIndexFloat = 0f;
            ControllerInputPoller.instance.leftControllerPrimaryButton = false;
            ControllerInputPoller.instance.leftControllerSecondaryButton = false;
            ControllerInputPoller.instance.rightControllerPrimaryButton = false;
            ControllerInputPoller.instance.rightControllerSecondaryButton = false;
        }

        public static void SpoofName()
        {
            string[] SpoofNames =
            {
              "ALICEVR",
              "BOB",
              "JMAN",
              "DAVIDVR",
              "EVE",
              "FRANK",
              "VMT",
              "HANNAHVR",
              "ISAAC",
              "JACKVR",
              "KATHY",
              "LIAM",
              "MONAVR",
              "RARENAME",
              "NATHANVR",
              "OLIVIA",
              "DAISY09",
              "QUINCY",
              "RACHELVR",
              "UHH",
              "ILOVEYOU"
            };
            int num = new System.Random().Next(SpoofNames.Length);
            PhotonNetwork.LocalPlayer.NickName = SpoofNames[num];
        }

        public static void AntiReportDisconnect() // Cred iiDk for all anti reports
        {
            try
            {
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (vrrig != GorillaTagger.Instance.offlineVRRig)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                if (D1 < threshold || D2 < threshold)
                                {
                                    PhotonNetwork.Disconnect();
                                    NotifiLib.SendNotification("Someone attempted to report you, you have been disconnected.");
                                    RpcCleanUp();
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        }

        public static void QuestAntiReportTEST()
        {
            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line.reportInProgress == true || line.reportedCheating == true || line.reportedHateSpeech == true || line.reportedToxicity == true)
                {
                    PhotonNetwork.Disconnect();
                    NotifiLib.SendNotification("Someone attempted to report you, you have been disconnected.");
                }
            }
        }

        public static void RpcCleanUp()
        {
            float RemoveTimer;
            PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
            PhotonNetwork.RemoveBufferedRPCs();
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
            ScienceExperimentManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);
            GorillaGameManager.instance.OnMasterClientSwitched(PhotonNetwork.LocalPlayer);

            if (GorillaTagger.Instance.myVRRig != null)
            {
                PhotonNetwork.OpCleanRpcBuffer(GorillaTagger.Instance.myVRRig);
            }
            RemoveTimer = Time.time;
            if (!Frame)
            {
                Frame = true;
                GorillaNot.instance.rpcErrorMax = int.MaxValue;
                GorillaNot.instance.rpcCallLimit = int.MaxValue;
                GorillaNot.instance.logErrorMax = int.MaxValue;
                PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
                PhotonNetwork.OpCleanRpcBuffer(GorillaTagger.Instance.myVRRig);
                PhotonNetwork.RemoveBufferedRPCs(GorillaTagger.Instance.myVRRig.ViewID, null, null);
                PhotonNetwork.RemoveRPCsInGroup(int.MaxValue);
                PhotonNetwork.SendAllOutgoingCommands();
                GorillaNot.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
            }
        }
        #endregion

        #region Cave Mods
        public static void OpenGates()
        {
            NotifiLib.SendNotification("<color=grey>[</color><color=green>Gates</color><color=grey>]</color> <color=white> Are Opening</color>");
            GameObject.Find("Environment Objects/05Maze_PersistentObjects/GhostLab").GetComponent<GhostLabReliableState>().photonView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
            GameObject.Find("Environment Objects/05Maze_PersistentObjects/GhostLab").GetComponent<GhostLabReliableState>().photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;

            GameObject.Find("Environment Objects/05Maze_PersistentObjects/GhostLab").GetComponent<GhostLabReliableState>().doorState = GhostLab.EntranceDoorsState.OuterDoorOpen;
            GameObject.Find("Environment Objects/05Maze_PersistentObjects/GhostLab").GetComponent<GhostLabReliableState>().singleDoorOpen[8] = true;
        }

        public static void SpawnMineGhost()
        {
            GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton").GetComponent<SecondLookSkeleton>().tapped = true;
            GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton").GetComponent<SecondLookSkeletonSynchValues>().photonView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
            GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton").GetComponent<SecondLookSkeletonSynchValues>().photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
            GameObject.Find("Environment Objects/05Maze_PersistentObjects/MinesSecondLookSkeleton").GetComponent<SecondLookSkeleton>().currentState = SecondLookSkeleton.GhostState.Activated;
            NotifiLib.SendNotification("<color=grey>[</color><color=green>LUCY</color><color=grey>]</color> <color=white> HAS SPAWN</color>");
        }
        #endregion

        #region Water Mods

        public static void AnnoyPlayerGun()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, -GorillaTagger.Instance.rightHandTransform.up, out var Ray);
                GameObject NewPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                NewPointer.GetComponent<Renderer>().material.color = Color.white;
                NewPointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                NewPointer.transform.position = CopyPlayer ? VrRigPlayers.transform.position : Ray.point;
                UnityEngine.Object.Destroy(NewPointer.GetComponent<BoxCollider>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(NewPointer.GetComponent<Collider>());
                UnityEngine.Object.Destroy(NewPointer, Time.deltaTime);

                GameObject line = new GameObject("Line");
                LineRenderer liner = line.AddComponent<LineRenderer>();
                liner.material.shader = Shader.Find("GUI/Text Shader");
                liner.startColor = Color.white;
                liner.endColor = Color.black;
                liner.startWidth = 0.025f;
                liner.endWidth = 0.025f;
                liner.positionCount = 2;
                liner.useWorldSpace = true;
                liner.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                liner.SetPosition(1, CopyPlayer ? VrRigPlayers.transform.position : Ray.point);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                if (CopyPlayer && VrRigPlayers != null && Time.time > SplashCoolDown)
                {
                    BallsOnHands();
                    GorillaTagger.Instance.offlineVRRig.enabled = false;

                    GorillaTagger.Instance.offlineVRRig.transform.position = VrRigPlayers.transform.position;
                    GorillaTagger.Instance.offlineVRRig.transform.rotation = VrRigPlayers.transform.rotation;

                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.position = VrRigPlayers.head.rigTarget.transform.position;
                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = VrRigPlayers.head.rigTarget.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 0f);
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 0f);
                    GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                    {
                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.position,
                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation,
                    4f,
                    100f,
                    true,
                    false
                    });
                    RpcCleanUp();
                    SplashCoolDown = Time.time + 0.1f;
                    foreach (MonkeyeAI monkeyeAI in Resources.FindObjectsOfTypeAll<MonkeyeAI>())
                    {
                        GetOwnership(monkeyeAI.gameObject.GetComponent<PhotonView>());
                        if (monkeyeAI.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                        {
                            monkeyeAI.gameObject.transform.position = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.position;
                        }
                        else
                        {
                            monkeyeAI.gameObject.GetComponent<PhotonView>().RequestOwnership();
                        }
                    }

                }
                if (Plugin.CalculateGrabState(ControllerInputPoller.instance.rightControllerIndexFloat, 0.1f))
                {
                    VRRig possibly = Ray.collider.GetComponentInParent<VRRig>();
                    if (possibly && possibly != GorillaTagger.Instance.offlineVRRig)
                    {
                        CopyPlayer = true;
                        VrRigPlayers = possibly;
                    }
                }
            }
            else
            {
                if (CopyPlayer)
                {
                    CopyPlayer = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }

        public static void WaterSplashHands()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton && Time.time > SplashCoolDown)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                {
                    GorillaTagger.Instance.rightHandTransform.position,
                    GorillaTagger.Instance.rightHandTransform.rotation,
                    4f,
                    100f,
                    true,
                    false
                });
                RpcCleanUp();
                SplashCoolDown = Time.time + 0.1f;
            }

            if (ControllerInputPoller.instance.leftControllerSecondaryButton && Time.time > SplashCoolDown)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                {
                    GorillaTagger.Instance.leftHandTransform.position,
                    GorillaTagger.Instance.leftHandTransform.rotation,
                    4f,
                    100f,
                    true,
                    false
                });
                RpcCleanUp();
                SplashCoolDown = Time.time + 0.1f;
            }
        }

        public static void WaterSplashBody()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton && Time.time > SplashCoolDown)
            {
                GorillaTagger.Instance.myVRRig.RPC("PlaySplashEffect", RpcTarget.All, new object[]
                {
                    GorillaTagger.Instance.bodyCollider.transform.position,
                    GorillaTagger.Instance.bodyCollider.transform.rotation,
                    4f,
                    100f,
                    true,
                    false
                });
                RpcCleanUp();
                SplashCoolDown = Time.time + 0.1f;
            }
        }

        #endregion

        #region Other Players
        public static void FreezeAll()
        {
            foreach (MonkeyeAI monkeyeAI in FindMonkeyes())
            {
                GetOwnership(monkeyeAI.gameObject.GetComponent<PhotonView>());
                monkeyeAI.gameObject.transform.position = new Vector3(1e11f, 1e11f, 1e11f);
            }
            if (Plugin.buttonsActive[9] == false)
            {
                Platforms();
            }
        }

        public static void FreezeAllOff()
        {
            foreach (MonkeyeAI monkeyeAI in FindMonkeyes())
            {
                GetOwnership(monkeyeAI.gameObject.GetComponent<PhotonView>());
                monkeyeAI.gameObject.transform.position = Vector3.zero;
            }
        }
        #endregion
    }
}
