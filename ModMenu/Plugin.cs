using GorillaNetworking;
using HarmonyLib;
using J0kersTrollMenu.MenuMods;
using J0kersTrollMenu.Notifications;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace J0kersTrollMenu.ModMenu
{
    [HarmonyPatch(typeof(GorillaLocomotion.Player), "LateUpdate", MethodType.Normal)]
    public class Plugin : MonoBehaviour
    {
        #region Instance
        public static Plugin _instance;
        public static Material boardmat;
        public static bool SendOneTime;
        public static Photon.Realtime.Player PlayerIDHelp;

        public static Plugin Instance
        {
            get
            {
                return _instance;
            }
        }

        public static bool CalculateGrabState(float grabValue, float grabThreshold)
        {
            return grabValue >= grabThreshold;
        }

        static float IsOn1;
        static float IsOn2;
        static float IsOn3;
        static float IsOn4;
        static float IsOn5;
        static float IsOn6;
        static float IsOn7;
        static float IsOn8;
        static float IsOn9;
        static float IsOn10;
        static float IsOn11;
        static float IsOn12;
        public static bool AllowNoClip = false;
        public static bool AntiNoClip = false;
        private static int pageSize = 4;
        private static int pageNumber = 0;
        public static bool gripDown;
        public static GameObject menu = null;
        public static GameObject canvasObj = null;
        public static string MenuPageActive = "1";
        public static GameObject referance = null;
        public static int framePressCooldown = 0;
        public static GameObject pointer = null;
        public static int btnCooldown = 0;
        public static Color coloresp;
        public static GradientColorKey[] colorKeys = new GradientColorKey[4];
        public static int[] bones = { 4, 3, 5, 4, 19, 18, 20, 19, 3, 18, 21, 20, 22, 21, 25, 21, 29, 21, 31, 29, 27, 25, 24, 22, 6, 5, 7, 6, 10, 6, 14, 6, 16, 14, 12, 10, 9, 7 };
        static Plugin sendOnAwake;
        internal static Plugin plugin
        {
            get
            {
                return sendOnAwake;
            }
        }
        #endregion
        private static void Prefix()
        {
            try
            {
                #region Stump   
                if (GameObject.Find("Environment Objects/LocalObjects_Prefab").transform.Find("TreeRoom").gameObject.activeSelf)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/CodeOfConduct").GetComponent<Text>().text = "< J0ker's Troll Menu >";
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/motd").GetComponent<Text>().text = "< J0ker's Troll Menu >";
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/CodeOfConduct/COC Text").GetComponent<Text>().text = "THAKNS FOR USING J0KER TROLL MENU!\nTHIS MENU IS FREE IF YOU BOUGHT IT YOU GOT SCAMMED! THIS MENU IS OPEN SOURCE!\nIF ITS OBFUSCATED ITS PROBABLY RATTED AND NEEDS TO BE REPORTED TO THE DISCORD\n<color=green>discord.gg/Kdwqg2VUHc</color>\n\nCREDS:\niidk - JumpScare Gun, Anti Report <3\nLARS: Notification Lib\n\nPLAYER NAME: " + PhotonNetwork.LocalPlayer.NickName + "\nFPS: " + ((int)(1f / Time.smoothDeltaTime)).ToString() + "\nPing: " + PhotonNetwork.GetPing().ToString();
                }


                Plugin.boardmat = new Material(Shader.Find("GorillaTag/UberShader"));
                Plugin.boardmat.color = Color.black;
                if (GameObject.Find("Environment Objects/LocalObjects_Prefab").transform.Find("TreeRoom").gameObject.activeSelf)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen").GetComponent<MeshRenderer>().material = Plugin.boardmat;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/forestatlas (combined by EdMeshCombinerSceneProcessor)").GetComponent<Renderer>().material = Plugin.boardmat;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/Wall Monitors Screens/wallmonitorforest").GetComponent<Renderer>().material = Plugin.boardmat;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/Wall Monitors Screens/wallmonitorcave").GetComponent<Renderer>().material = Plugin.boardmat;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/Wall Monitors Screens/wallmonitorskyjungle").GetComponent<Renderer>().material = Plugin.boardmat;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/Wall Monitors Screens/wallmonitorcosmetics").GetComponent<Renderer>().material = Plugin.boardmat;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/Wall Monitors Screens/wallmonitorcanyon").GetComponent<Renderer>().material = Plugin.boardmat;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/motd").GetComponent<Text>().color = Color.white;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/CodeOfConduct").GetComponent<Text>().color = Color.white;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/CodeOfConduct/COC Text").GetComponent<Text>().color = Color.white;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Tree Room Texts/WallScreenForest").GetComponent<Text>().color = Color.white;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Tree Room Texts/WallScreenCave").GetComponent<Text>().color = Color.white;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Tree Room Texts/WallScreenCity Front").GetComponent<Text>().color = Color.white;
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Tree Room Texts/WallScreenCanyon").GetComponent<Text>().color = Color.white;
                }
                #endregion
                GorillaLocomotion.Player __instance = GorillaLocomotion.Player.Instance;
                List<UnityEngine.XR.InputDevice> list = new List<UnityEngine.XR.InputDevice>();
                if (ControllerInputPoller.instance.controllerType == GorillaControllerType.OCULUS_DEFAULT)
                {
                    if (ControllerInputPoller.instance.leftControllerSecondaryButton && menu == null)
                    {
                        if (MenuPageActive == "1")
                        {
                            Draw();
                        }
                        if (referance == null)
                        {
                            referance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            referance.name = "pointer";
                            referance.transform.parent = __instance.rightControllerTransform;
                            referance.transform.localPosition = new Vector3(0f, -0.1f, 0f);
                            referance.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                            colorKeys[0].color = Color.black;
                            colorKeys[0].time = 0f;
                            colorKeys[1].color = Color.white;
                            colorKeys[1].time = 0.3f;
                            colorKeys[2].color = Color.black;
                            colorKeys[2].time = 0.6f;


                            ColorChanger colorChanger = referance.AddComponent<ColorChanger>();
                            colorChanger.colors = new Gradient
                            {
                                colorKeys = colorKeys
                            };
                            colorChanger.Start();
                        }
                    }
                    else if (!ControllerInputPoller.instance.leftControllerSecondaryButton && menu != null)
                    {
                        Destroy(referance);
                        menu.AddComponent<Rigidbody>();
                        UnityEngine.Object.Destroy(menu, 1f);
                        menu = null;
                        referance = null;
                    }
                    if (ControllerInputPoller.instance.leftControllerSecondaryButton && menu != null)
                    {
                        menu.transform.position = __instance.leftControllerTransform.position;
                        menu.transform.rotation = __instance.leftControllerTransform.rotation;
                    }
                }
                if (ControllerInputPoller.instance.controllerType == GorillaControllerType.INDEX)
                {
                    if (CalculateGrabState(ControllerInputPoller.instance.leftControllerIndexFloat, 0.1f) && menu == null)
                    {
                        if (MenuPageActive == "1")
                        {
                            Draw();
                        }
                        if (referance == null)
                        {
                            referance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            referance.name = "pointer";
                            referance.transform.parent = __instance.rightControllerTransform;
                            referance.transform.localPosition = new Vector3(0f, -0.1f, 0f);
                            referance.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                            colorKeys[0].color = Color.black;
                            colorKeys[0].time = 0f;
                            colorKeys[1].color = Color.white;
                            colorKeys[1].time = 0.3f;
                            colorKeys[2].color = Color.black;
                            colorKeys[2].time = 0.6f;


                            ColorChanger colorChanger = referance.AddComponent<ColorChanger>();
                            colorChanger.colors = new Gradient
                            {
                                colorKeys = colorKeys
                            };
                            colorChanger.Start();
                        }
                    }
                    else if (!CalculateGrabState(ControllerInputPoller.instance.leftControllerIndexFloat, 0.1f) && menu != null)
                    {
                        Destroy(referance);
                        menu.AddComponent<Rigidbody>();
                        UnityEngine.Object.Destroy(menu, 1f);
                        menu = null;
                        referance = null;
                    }
                    if (CalculateGrabState(ControllerInputPoller.instance.leftControllerIndexFloat, 0.1f) && menu != null)
                    {
                        menu.transform.position = __instance.leftControllerTransform.position;
                        menu.transform.rotation = __instance.leftControllerTransform.rotation;
                    }
                }

                #region Buttons
                if (buttonsActive[0] == true)
                {
                    Mods.Flight();
                }

                if (buttonsActive[1] == true)
                {
                    Mods.Platforms();
                }

                if (buttonsActive[2] == true)
                {
                    Mods.Speed();
                }
                else
                {
                    Mods.SpeedFix();
                }

                if (buttonsActive[3] == true)
                {
                    Mods.LongArms();
                }
                else
                {
                    Mods.LongArmsFix();
                }

                if (buttonsActive[4] == true)
                {
                    Mods.NoGrav();
                }
                else
                {
                    Mods.NoGravOff();
                }


                if (buttonsActive[5] == true)
                {
                    #region Bone-ESP
                    Material material = new Material(Shader.Find("GUI/Text Shader"));
                    material.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 1));

                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        bool flag = !vrrig.isOfflineVRRig && !vrrig.isMyPlayer;
                        if (flag)
                        {

                            if (!vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>())
                            {
                                vrrig.head.rigTarget.gameObject.AddComponent<LineRenderer>();
                            }



                            vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().endWidth = 0.025f;
                            vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().startWidth = 0.025f;
                            vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().material = material;
                            vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0, 0.160f, 0));
                            vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0, 0.4f, 0));



                            for (int i = 0; i < bones.Count(); i += 2)
                            {
                                if (!vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>())
                                {
                                    vrrig.mainSkin.bones[bones[i]].gameObject.AddComponent<LineRenderer>();
                                }
                                vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>().endWidth = 0.025f;
                                vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>().startWidth = 0.025f;
                                vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>().material = material;
                                vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>().SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                                vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>().SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);
                            }
                        }

                    }
                }
                else
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        bool flag = !vrrig.isOfflineVRRig && !vrrig.isMyPlayer;
                        if (flag)
                        {
                            for (int i = 0; i < bones.Count(); i += 2)
                            {
                                if (vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>())
                                {
                                    UnityEngine.Object.Destroy(vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>());
                                }
                                if (vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>())
                                {
                                    UnityEngine.Object.Destroy(vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>());
                                }
                            }
                        }
                    }
                    #endregion
                }


                if (buttonsActive[6] == true)
                {
                    Mods.Ghost();
                }

                if (buttonsActive[7] == true)
                {
                    Mods.Invis();
                }

                if (buttonsActive[8] == true)
                {
                    GorillaLocomotion.Player.Instance.disableMovement = false;
                }

                if (buttonsActive[9] == true)
                {
                    Mods.FlyAtPlayer();
                }

                if (buttonsActive[10] == true)
                {
                    Mods.AutoFunnyRun();
                }

                if (buttonsActive[11] == true)
                {
                    Mods.Bees();
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }

                if (buttonsActive[12] == true)
                {
                    Mods.NoFinger();
                }

                if (buttonsActive[13] == true)
                {
                    Mods.DisableMouthMovement();
                }
                else
                {
                    Mods.EnableMouthMovement();
                }

                if (buttonsActive[14] == true)
                {
                    Mods.HELICOPTERHEAD();
                }
                else
                {
                    Mods.FixHead();
                }

                if (buttonsActive[15] == true)
                {
                    Mods.JoinDaisy09();
                    buttonsActive[15] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[16] == true)
                {
                    Mods.JoinPBBV();
                    buttonsActive[16] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[17] == true)
                {
                    Mods.JoinSren17();
                    buttonsActive[17] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[18] == true)
                {
                    Mods.JoinSren18();
                    buttonsActive[18] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[19] == true)
                {
                    Mods.JoinJ3vu();
                    buttonsActive[19] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[20] == true)
                {
                    Mods.JoinAI();
                    buttonsActive[20] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[21] == true)
                {
                    Mods.JoinGhost();
                    buttonsActive[21] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[22] == true)
                {
                    Mods.JoinRun();
                    buttonsActive[22] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[23] == true)
                {
                    Mods.JoinRabbit();
                    buttonsActive[23] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[24] == true)
                {
                    Mods.JoinError();
                    buttonsActive[24] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[25] == true)
                {
                    Mods.JoinBot();
                    buttonsActive[25] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[26] == true)
                {
                    Mods.JoinTipToe();
                    buttonsActive[26] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[27] == true)
                {
                    Mods.JoinSpider();
                    buttonsActive[27] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[28] == true)
                {
                    Mods.JoinSTATUE();
                    buttonsActive[28] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[29] == true)
                {
                    Mods.JoinBANSHEE();
                    buttonsActive[29] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[30] == true)
                {
                    Mods.JoinISeeYouBan();
                    buttonsActive[30] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[31] == true)
                {
                    Mods.Join666();
                    buttonsActive[31] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[32] == true)
                {
                    NotifiLib.IsEnabled = false;
                    buttonsActive[32] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[33] == true)
                {
                    NotifiLib.IsEnabled = true;
                    buttonsActive[33] = false;
                    Destroy(menu);
                    menu = null;
                    Draw();
                }

                if (buttonsActive[34] == true)
                {
                    Mods.AntiReportDisconnect();
                }

                #endregion
            }
            catch (Exception ex)
            {
                Debug.LogError($"[J0kers ERROR Log] : {ex}");
            }
        }

        public static string[] buttons = new string[]
        {
            "Fly",
            "Plats [B]",
            "Speed Boost",
            "Long Arms",
            "No Grav",
            "Bone ESP",
            "Ghost Monke [B]",
            "Invis Monke [T]",
            "No Tag Freeze",
            "Fly At Player Gun",
            "Auto Run [T]",
            "Bees [T]",
            "No Finger Movement",
            "No Mouth Move",
            "Head Spaz",
            "Join Daisy09",
            "Join PBBV",
            "Join Sren17",
            "Join Sren18",
            "Join J3VU",
            "Join AI",
            "Join Ghost",
            "Join Run",
            "Join Rabbit",
            "Join Error",
            "Join Bot",
            "Join TipToe",
            "Join Spider",
            "Join Statue",
            "Join BanShee",
            "Join ISeeYouBan",
            "Join 666",
            "Notifications [Off]",
            "Notifications [On]",
            "Anti Report [Disconnect]"
        };
        #region ButtonsActive
        public static bool?[] buttonsActive = new bool?[]
        {
            false,false,false,false,false, false,false,false,false,false,false, false,false,false,false,false,false, false,false,false,false,false,false, false,false,false,false,false,false, false,false,false,false,false,false, false,false,false,false,false,false, false,false,false,false,false, false,false,false,false,false, false,false,false,false,false, false,false,false,false,false, false,false,false,false,false, false,false,false,false,false, false,false,false,false,false, false,false,false,false,false, false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
            false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
        };
        #endregion

    #region Draw
        private static void AddButton(float offset, string text)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f);
            gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.53f - offset);
            gameObject.AddComponent<BtnCollider>().relatedText = text;
            gameObject.name = text;

            int num = -1;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (text == buttons[i])
                {
                    num = i;
                    break;
                }
            }

            if (buttonsActive[num] == false)
            {
                gameObject.GetComponent<Renderer>().material.color = Color.black;
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.color = Color.white;
            }

            GameObject gameObject2 = new GameObject();
            gameObject2.transform.parent = canvasObj.transform;

            Text text2 = gameObject2.AddComponent<Text>();
            text2.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text2.text = text;
            text2.fontSize = 1;
            text2.fontStyle = FontStyle.Italic;
            text2.alignment = TextAnchor.MiddleCenter;
            text2.resizeTextForBestFit = true;
            text2.resizeTextMinSize = 0;

            if (buttonsActive[num] == false)
            {
                text2.color = Color.white;
            }

            if (buttonsActive[num] == true)
            {
                text2.color = Color.black;
            }

            RectTransform component = text2.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f);
            component.localPosition = new Vector3(0.064f, 0f, 0.211f - offset / 2.55f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }
        public static void Draw()
        {
            MenuPageActive = "1";

            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(menu.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(menu.GetComponent<BoxCollider>());
            UnityEngine.Object.Destroy(menu.GetComponent<Renderer>());
            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f);
            menu.name = "Menu";

            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.1f, 0.86f, 0.6f);
            gameObject.name = "Menucolor";
            gameObject.transform.position = new Vector3(0.05f, 0f, 0.03f);

            GameObject Outline = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(Outline.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(Outline.GetComponent<BoxCollider>());
            Outline.transform.parent = menu.transform;
            Outline.GetComponent<Renderer>().material.shader = Shader.Find("UI/Default");
            Outline.transform.rotation = Quaternion.identity;
            Outline.transform.localScale = new Vector3(0.09f, 0.87f, -0.61f);
            Outline.name = "MenuOutline";
            Outline.transform.position = new Vector3(0.05f, 0f, 0.03f);

            canvasObj = new GameObject();
            canvasObj.transform.parent = menu.transform;
            canvasObj.name = "j0kercanvas";
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            CanvasScaler canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = 1000f;

            GameObject gameObject2 = new GameObject();
            gameObject2.transform.parent = canvasObj.transform;
            gameObject2.name = "Title";
            Text text = gameObject2.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.text = "J0ker Troll Menu";
            text.fontSize = 1;
            text.color = Color.white;
            text.fontStyle = FontStyle.Italic;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.28f, 0.05f);
            component.position = new Vector3(0.06f, 0f, 0.175f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            AddPageButtons();
            string[] array2 = buttons.Skip(pageNumber * pageSize).Take(pageSize).ToArray();
            for (int i = 0; i < array2.Length; i++)
            {
                AddButton((float)i * 0.13f + 0.26f, array2[i]);
            }
        }
        private static void AddPageButtons()
        {
            int num = (buttons.Length + pageSize - 1) / pageSize;
            int num2 = pageNumber + 1;
            int num3 = pageNumber - 1;
            if (num2 > num - 1)
            {
                num2 = 0;
            }
            if (num3 < 0)
            {
                num3 = num - 1;
            }
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.09f, 0.15f, 0.58f);
            gameObject.transform.localPosition = new Vector3(0.56f, 0.5833f, 0.07f);
            gameObject.AddComponent<BtnCollider>().relatedText = "PreviousPage";
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            gameObject.name = "back";
            gameObject.GetComponent<Renderer>().material.color = Color.black;
            GameObject gameObject2 = new GameObject();
            gameObject2.transform.parent = canvasObj.transform;
            gameObject2.name = "back";
            Text text = gameObject2.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text.text = "<";
            text.fontSize = 1;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f);
            component.localPosition = new Vector3(0.064f, 0.175f, 0.04f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            component.localScale = new Vector3(1.3f, 1.3f, 1.3f);


            #region Outlineback

            GameObject OutlineBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(OutlineBack.GetComponent<Rigidbody>());
            OutlineBack.GetComponent<BoxCollider>().isTrigger = true;
            OutlineBack.transform.parent = menu.transform;
            OutlineBack.transform.rotation = Quaternion.identity;
            OutlineBack.transform.localScale = new Vector3(0.08f, 0.16f, 0.59f);
            OutlineBack.transform.localPosition = new Vector3(0.56f, 0.5833f, 0.07f);
            OutlineBack.name = "BackLine";
            OutlineBack.GetComponent<Renderer>().material.shader = Shader.Find("UI/Default");


            #endregion

            GameObject gameObject3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject3.GetComponent<Renderer>().material.color = Color.black;
            Destroy(gameObject3.GetComponent<Rigidbody>());
            gameObject3.GetComponent<BoxCollider>().isTrigger = true;
            gameObject3.transform.parent = menu.transform;
            gameObject3.transform.rotation = Quaternion.identity;
            gameObject3.name = "Next";
            gameObject3.transform.localScale = new Vector3(0.09f, 0.15f, 0.58f);
            gameObject3.transform.localPosition = new Vector3(0.56f, -0.5833f, 0.07f);
            gameObject3.AddComponent<BtnCollider>().relatedText = "NextPage";
            gameObject3.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            GameObject gameObject4 = new GameObject();
            gameObject4.transform.parent = canvasObj.transform;
            gameObject4.name = "Next";
            Text text2 = gameObject4.AddComponent<Text>();
            text2.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text2.text = ">";
            text2.fontSize = 1;
            text2.alignment = TextAnchor.MiddleCenter;
            text2.resizeTextForBestFit = true;
            text2.resizeTextMinSize = 0;
            RectTransform component2 = text2.GetComponent<RectTransform>();
            component2.localPosition = Vector3.zero;
            component2.sizeDelta = new Vector2(0.2f, 0.03f);
            component2.localPosition = new Vector3(0.064f, -0.175f, 0.04f);
            component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            component2.localScale = new Vector3(1.3f, 1.3f, 1.3f);

            #region Outline Next

            GameObject OutlineNext = GameObject.CreatePrimitive(PrimitiveType.Cube);
            OutlineNext.GetComponent<Renderer>().material.color = Color.black;
            Destroy(OutlineNext.GetComponent<Rigidbody>());
            OutlineNext.GetComponent<BoxCollider>().isTrigger = true;
            OutlineNext.transform.parent = menu.transform;
            OutlineNext.transform.rotation = Quaternion.identity;
            OutlineNext.name = "Nextline";
            OutlineNext.transform.localScale = new Vector3(0.08f, 0.16f, 0.59f);
            OutlineNext.transform.localPosition = new Vector3(0.56f, -0.5833f, 0.07f);
            OutlineNext.GetComponent<Renderer>().material.shader = Shader.Find("UI/Default");

            #endregion




            GameObject gameObject5 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject5.GetComponent<Renderer>().material.color = Color.black;
            Destroy(gameObject5.GetComponent<Rigidbody>());
            gameObject5.GetComponent<BoxCollider>().isTrigger = true;
            gameObject5.transform.parent = menu.transform;
            gameObject5.transform.rotation = Quaternion.identity;
            gameObject5.name = "LeaveButton";
            gameObject5.transform.localScale = new Vector3(0.09f, 0.7682f, 0.075f);
            gameObject5.transform.localPosition = new Vector3(0.56f, -0.0076f, 0.5755f);
            gameObject5.AddComponent<BtnCollider>().relatedText = "Cum";
            gameObject5.GetComponent<Renderer>().material.SetColor("_Color", Color.black);


            #region Outline 

            GameObject LeaveOutline = GameObject.CreatePrimitive(PrimitiveType.Cube);
            LeaveOutline.GetComponent<Renderer>().material.shader = Shader.Find("UI/Default");
            Destroy(LeaveOutline.GetComponent<Rigidbody>());
            LeaveOutline.transform.parent = menu.transform;
            LeaveOutline.transform.rotation = Quaternion.identity;
            LeaveOutline.name = "LeaveButtonLine";
            LeaveOutline.transform.localScale = new Vector3(0.08f, 0.7783f, 0.079f);
            LeaveOutline.transform.localPosition = new Vector3(0.56f, -0.0076f, 0.5755f);

            #endregion

            GameObject gameObject6 = new GameObject();
            gameObject6.transform.parent = canvasObj.transform;
            gameObject6.name = "LeaveButton";
            Text text3 = gameObject6.AddComponent<Text>();
            text3.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            text3.text = "Leave";
            text3.fontSize = 1;
            text3.alignment = TextAnchor.MiddleCenter;
            text3.resizeTextForBestFit = true;
            text3.resizeTextMinSize = 0;
            RectTransform component3 = text3.GetComponent<RectTransform>();
            component3.localPosition = Vector3.zero;
            component3.sizeDelta = new Vector2(0.2f, 0.03f);
            component3.localPosition = new Vector3(0.064f, 0, 0.23f);
            component3.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            component3.localScale = new Vector3(1f, 1f, 1f);
        }
        public static void Toggle(string relatedText)
        {
            int num = (buttons.Length + pageSize - 1) / pageSize;
            if (relatedText == "Cum")
            {
                PhotonNetwork.Disconnect();
                Destroy(menu);
                menu = null;
                Draw();
                return;
            }
            if (relatedText == "NextPage")
            {
                if (pageNumber < num - 1)
                {
                    pageNumber++;
                }
                else
                {
                    pageNumber = 0;
                }
                Destroy(menu);
                menu = null;
                Draw();
                return;
            }
            if (relatedText == "PreviousPage")
            {
                if (pageNumber > 0)
                {
                    pageNumber--;
                }
                else
                {
                    pageNumber = num - 1;
                }
                Destroy(menu);
                menu = null;
                Draw();
                return;
            }
            int num2 = -1;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (relatedText == buttons[i])
                {
                    num2 = i;
                    break;
                }
            }
            if (buttonsActive[num2].HasValue)
            {
                buttonsActive[num2] = !buttonsActive[num2];
                Destroy(menu);
                menu = null;
                Draw();
            }
        }


        public void Awake()
        {
            sendOnAwake = this;
        }
    }

    #endregion

    #region TimedBehaviour
    public class TimedBehaviour : MonoBehaviour
        {
            public virtual void Start()
            {
                startTime = Time.time;
            }
            public virtual void Update()
            {
                if (!complete)
                {
                    progress = Mathf.Clamp((Time.time - startTime) / duration, 0f, 1.5f);
                    if ((double)Time.time - startTime > duration)
                    {
                        bool flag3 = loop;
                        if (flag3)
                        {
                            OnLoop();
                        }
                        else
                        {
                            complete = true;
                        }
                    }
                }
            }
            public virtual void OnLoop()
            {
                startTime = Time.time;
            }
            public bool complete = false;
            public bool loop = true;
            public float progress = 0f;
            protected bool paused = false;
            protected float startTime;
            protected float duration = 2f;
        }
        public class ColorChanger : TimedBehaviour
        {
            public override void Start()
            {
                base.Start();
                gameObjectRenderer = GetComponent<Renderer>();
            }
            public override void Update()
            {
                base.Update();
                if (colors != null)
                {
                    if (timeBased)
                    {
                        color = colors.Evaluate(progress);
                    }
                    gameObjectRenderer.material.color = color;
                    gameObjectRenderer.material.SetColor("_Color", color);
                    gameObjectRenderer.material.SetColor("_EmissionColor", color);
                }
            }
            public Renderer gameObjectRenderer;
            public Gradient colors = null;
            public Color color;
            public bool timeBased = true;
        }
    #endregion


    #region BtnCollider
    internal class BtnCollider : MonoBehaviour
        {
            private void OnTriggerEnter(Collider collider)
            {
                if (Time.frameCount >= Plugin.framePressCooldown + 10 && collider.gameObject.name == "pointer")
                {
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(66, false, 0.1f);
                    GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength / 2, GorillaTagger.Instance.tagHapticDuration / 2);
                    if (Plugin.MenuPageActive == "1")
                    {
                        Plugin.Toggle(relatedText);
                    }
                    Plugin.framePressCooldown = Time.frameCount;
                }
            }
            public string relatedText;
        }
    #endregion
}
