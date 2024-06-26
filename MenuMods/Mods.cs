
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using GorillaNetworking;
using System.Collections.Generic;
using J0kersTrollMenu.Notifications;
using System.IO;
using GorillaTagScripts;
using System.Reflection;

namespace J0kersTrollMenu.MenuMods
{
    internal class Mods : MonoBehaviour
    {
        static float beesDelay;
        static VRRig VrRigPlayers = null;
        static bool CopyPlayer;
        static bool bothHands;
        static GameObject pointer;
        static bool isFirstActivation = true;
        static float activationTime = 0f;
        static float activationDuration = 2f;
        static bool isFirstOn = true;

        #region VRRig Stuff

        public static VRRig GetVRRigFromPlayer(Player p)
        {
            return GorillaGameManager.instance.FindPlayerVRRig(p);
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

        #region Gorilla Tagger Mods
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
                    GorillaTagger.Instance.leftHandTransform.position = GorillaTagger.Instance.headCollider.transform.position + (GorillaTagger.Instance.headCollider.transform.forward * Mathf.Cos(time + 180) / 10) + new Vector3(0, -0.5f - (Mathf.Sin(time + 180) / 7), 0) + (GorillaTagger.Instance.headCollider.transform.right * 0.05f);
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

        public static void PlayerHoldMe()
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
                    GorillaTagger.Instance.offlineVRRig.transform.position = VrRigPlayers.rightHandTransform.position;
                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * -1f);
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 1f);
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

        public static void RigGun()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out var hitinfo);
                pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pointer.transform.position = hitinfo.point;
                pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                pointer.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
                pointer.GetComponent<Renderer>().material.color = Color.white;
                GameObject.Destroy(pointer.GetComponent<BoxCollider>());
                GameObject.Destroy(pointer.GetComponent<Rigidbody>());
                GameObject.Destroy(pointer.GetComponent<Collider>());

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
                liner.SetPosition(1, pointer.transform.position);
                UnityEngine.Object.Destroy(line, Time.deltaTime);

                if (ControllerInputPoller.instance.rightControllerIndexFloat >= 0.1)
                {
                    GameObject.Destroy(pointer, Time.deltaTime);
                    pointer.GetComponent<Renderer>().material.color = Color.black;
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * -1f);
                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 1f);
                    GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                    GorillaTagger.Instance.offlineVRRig.transform.position = pointer.transform.position + new Vector3(0f, 1f, 0f);
                }

            }
            if (pointer != null)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
                GameObject.Destroy(pointer, Time.deltaTime);
            }
        }

        public static void Bees()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat >= 0.1)
            {
                BallsOnHands();
                GorillaTagger.Instance.offlineVRRig.enabled = false;

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

        public static void GhostTPose()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                BallsOnHands();
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * -1f);
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + (GorillaTagger.Instance.offlineVRRig.transform.right * 1f);
                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void StareAtNearby()
        {
            GorillaTagger.Instance.offlineVRRig.headConstraint.LookAt(Mods.GetClosestVRRig().headMesh.transform.position);
        }

        public static void Invis()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.4f)
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

        public static void GhostFlight()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position += GorillaTagger.Instance.offlineVRRig.headConstraint.transform.forward * Time.deltaTime * 1;
                GorillaTagger.Instance.offlineVRRig.GetComponent<Rigidbody>().velocity = Vector3.zero;

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

                AION();
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        #region Shitty AI Code
        public static void AION()
        {
            if (isFirstActivation)
            {
                activationTime = Time.time;
                ActivateFirst();
                isFirstActivation = false;
            }
            if (Time.time - activationTime >= activationDuration)
            {
                isFirstOn = !isFirstOn;
                activationTime = Time.time;
            }

            if (isFirstOn)
            {
                ActivateFirst();
            }
            else
            {
                ActivateSecond();
            }
        }



        static void ActivateFirst()
        {
            float time = Time.frameCount;
            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * -1f;
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * 1f;
        }

        static void ActivateSecond()
        {
            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * 0f;
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * 0f;
        }
        #endregion

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

        public static void Join666()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("666", JoinType.Solo);
        }


        public static void JoinDaisy09()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("DAISY09", JoinType.Solo);
        }

        public static void JoinPBBV()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("PBBV", JoinType.Solo);
        }

        public static void JoinSren17()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("SREN17", JoinType.Solo);
        }

        public static void JoinSren18()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("SREN18", JoinType.Solo);
        }

        public static void JoinAI()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("AI", JoinType.Solo);
        }

        public static void JoinGhost()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("GHOST", JoinType.Solo);
        }

        public static void JoinJ3vu()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("J3VU", JoinType.Solo);
        }

        public static void JoinRun()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("RUN", JoinType.Solo);
        }

        public static void JoinBot()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("BOT", JoinType.Solo);
        }

        public static void JoinTipToe()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("TIPTOE", JoinType.Solo);
        }

        public static void JoinSpider()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("SPIDER", JoinType.Solo);
        }

        public static void JoinSTATUE()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("STATUE", JoinType.Solo);
        }

        public static void JoinBANSHEE()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("BANSHEE", JoinType.Solo);
        }

        public static void JoinRabbit()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("RABBIT", JoinType.Solo);
        }

        public static void JoinError()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("ERROR", JoinType.Solo);
        }

        public static void JoinISeeYouBan()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("ISEEYOUBAN", JoinType.Solo);
        }

        public static void JoinPL7()
        {
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("PL7", JoinType.Solo);
        }
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

        public static void FlightWithNoClip()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * 15;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
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

        #endregion 

        #region Safty

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
              "NATHANVR",
              "OLIVIA",
              "DAISY09",
              "QUINCY",
              "RACHELVR"
            };
            int num = new System.Random().Next(SpoofNames.Length);
            PhotonNetwork.LocalPlayer.NickName = SpoofNames[num];
        }
    


        public static List<GorillaScoreBoard> currentboards = new List<GorillaScoreBoard>() { };
        public static GorillaScoreBoard[] boards = null;

        public static void AntiReportDisconnect()
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
            catch { } // Not connected
        }
        #endregion

        #region Cave Mods
        public static void OpenGates()
        {
            NotifiLib.SendNotification("<color=grey>[</color><color=green>Outside Gates</color><color=grey>]</color> <color=white> Are Opening</color>");
            NotifiLib.SendNotification("<color=grey>[</color><color=green>Info:</color><color=grey>]</color> <color=white> USE THE OTHER GATE BUTTON NOW!</color>");
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

        #region Time Mods 

        public static void TimeYup()
        {
            int[] TimeOfDay = { 3, 1, 7, 0 };
            int num = new System.Random().Next(TimeOfDay.Length);
            BetterDayNightManager.instance.SetTimeOfDay(TimeOfDay[num]);
        }

        #endregion
    }
}
