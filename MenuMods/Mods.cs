using BepInEx;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using GorillaNetworking;
using System.Collections.Generic;
using J0kersTrollMenu.Notifications;

namespace J0kersTrollMenu.MenuMods
{
    internal class Mods : BaseUnityPlugin
    {
        private static float beesDelay;
        private static float laggyRigDelay;
        public static VRRig VrRigPlayers = null;
        public static bool DoThis = false;
        private static bool CopyPlayer;


        #region VRRig Stuff

        public static VRRig GetVRRigFromPlayer(Player p)
        {
            return GorillaGameManager.instance.FindPlayerVRRig(p);
        }

        public static PhotonView GetPhotonViewFromVRRig(VRRig p)
        {
            return (PhotonView)Traverse.Create(p).Field("photonView").GetValue();
        }

        public static VRRig FindVRRigForPlayer(Photon.Realtime.Player player)
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                bool flag = !vrrig.isOfflineVRRig && vrrig.GetComponent<PhotonView>().Owner == player;
                if (flag)
                {
                    return vrrig;
                }
            }
            return null;
        }

        public static VRRig GetClosestVRRig()
        {
            float num = float.MaxValue;
            VRRig result = null;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                bool flag = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position) < num;
                if (flag)
                {
                    num = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position);
                    result = vrrig;
                }
            }
            return result;
        }

        #endregion

        #region GorillaTagger Mods
        public static void BallsOnHands()
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

        public static void LongArms()
        {
            GorillaTagger.Instance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }

        public static void LongArmsFix()
        {
            GorillaTagger.Instance.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public static void HELICOPTERHEAD()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x = Random.Range(0f, 360f);
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = Random.Range(0f, 360f);
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = Random.Range(0f, 360f);
        }

        public static void FixHead()
        {
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.x = 0f;
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = 0f;
            GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.z = 0f;
        }

        public static void AutoFunnyRun()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat >= 0.1)
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
                }
            }
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

                    Vector3 position = GorillaTagger.Instance.offlineVRRig.transform.position + (look * (15f * Time.deltaTime));

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
                if (ControllerInputPoller.instance.rightControllerIndexFloat >= 0.1)
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

        public static void LagRigSelf()
        {
            BallsOnHands();
            if (Time.time > Mods.laggyRigDelay)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                Mods.laggyRigDelay = Time.time + 0.211f;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

            }
        }

        public static void Bees()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat >= 0.1)
            {
                BallsOnHands();
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GameObject gameObject = new GameObject("Line1");
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
                if (Time.time > Mods.beesDelay)
                {
                    VRRig vrrig4 = GorillaParent.instance.vrrigs[Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
                    GorillaTagger.Instance.offlineVRRig.transform.position = vrrig4.transform.position + new Vector3(0f, 1f, 0f);
                    GorillaTagger.Instance.myVRRig.transform.position = vrrig4.transform.position + new Vector3(0f, 1f, 0f);
                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = vrrig4.transform.position;
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = vrrig4.transform.position;
                    Mods.beesDelay = Time.time + 0.777f;
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }


        public static void Ghost()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                BallsOnHands();
                GorillaTagger.Instance.offlineVRRig.enabled = false;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }



        public static void Invis()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.4f)
            {
                BallsOnHands();
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(GorillaLocomotion.Player.Instance.headCollider.transform.position.x, -646.46466f, GorillaLocomotion.Player.Instance.headCollider.transform.position.z);
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
            GorillaLocomotion.Player.Instance.maxJumpSpeed = 9.8f;
        }

        public static void SpeedFix()
        {
            GorillaLocomotion.Player.Instance.maxJumpSpeed = 6.5f;
        }

        #endregion

        #region Room Mods

        public static void Join666()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("666");
        }


        public static void JoinDaisy09()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("DAISY09");
        }

        public static void JoinPBBV()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("PBBV");
        }

        public static void JoinSren17()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("SREN17");
        }

        public static void JoinSren18()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("SREN18");
        }

        public static void JoinAI()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("AI");
        }

        public static void JoinGhost()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("GHOST");
        }

        public static void JoinJ3vu()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("J3VU");
        }

        public static void JoinRun()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("RUN");
        }

        public static void JoinBot()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("BOT");
        }

        public static void JoinTipToe()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("TIPTOE");
        }

        public static void JoinSpider()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("SPIDER");
        }

        public static void JoinSTATUE()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("STATUE");
        }

        public static void JoinBANSHEE()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("BANSHEE");
        }

        public static void JoinRabbit()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("RABBIT");
        }

        public static void JoinError()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("ERROR");
        }

        public static void JoinISeeYouBan()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("ISEEYOUBAN");
        }

        public static void JoinPL7()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("PL7");
        }
        #endregion

        #region Plats

        private static GameObject PlatFL;
        private static GameObject PlatFR;
        private static bool PlatLSpawn;
        private static bool PlatRpawn;
        private static GradientColorKey[] colorKeys;

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

        public static void Flight()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * 15;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
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
        public static void DisableMouthMovement()
        {
            GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaMouthFlap>().enabled = false;
            GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaEyeExpressions>().enabled = false;
        }

        public static void EnableMouthMovement()
        {
            GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaMouthFlap>().enabled = true;
            GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaEyeExpressions>().enabled = true;
        }

        #endregion

        #region Safty

        public static List<GorillaScoreBoard> currentboards = new List<GorillaScoreBoard>() { };
        public static GorillaScoreBoard[] boards = null;
        private static bool bothHands;

        public static void AntiReportDisconnect()
        {
            try
            {
                if (boards == null)
                {
                    boards = GameObject.FindObjectsOfType<GorillaScoreBoard>();
                    foreach (GorillaScoreBoard fix in boards)
                    {
                        try
                        {
                            if (!currentboards.Contains(fix) || currentboards.Count <= 0)
                            {
                                currentboards.Add(fix);
                            }
                        }
                        catch
                        {
                            currentboards.Add(fix);
                        }
                    }
                }
                foreach (GorillaScoreBoard board in currentboards)
                {
                    foreach (GorillaPlayerScoreboardLine line in board.lines)
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

                                    float threshold = 0.35f;

                                    if (D1 < threshold || D2 < threshold)
                                    {
                                        PhotonNetwork.Disconnect();
                                        NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>Someone attempted to report you, you have been disconnected.</color>");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        } 

        #endregion
    }
}