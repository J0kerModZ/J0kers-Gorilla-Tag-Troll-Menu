using UnityEngine;
using HarmonyLib;
using System;
using UnityEngine.XR;

namespace J0kersTrollMenu.Patchers
{
    #region Shader Patch

    [HarmonyPatch(typeof(GameObject))]
    [HarmonyPatch("CreatePrimitive", 0)]
    internal class ShaderFix : MonoBehaviour
    {
        private static void Postfix(GameObject __result)
        {
            __result.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            __result.GetComponent<Renderer>().material.color = Color.black;
        }
    }
    #endregion

    #region Ghost Patch

    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    internal class GhostPatch : MonoBehaviour
    {
        public static bool Prefix(VRRig __instance)
        {
            if (__instance == GorillaTagger.Instance.offlineVRRig)
            {
                return false;
            }
            return true;
        }
    }
    #endregion

    #region Input
    internal class ControllerInput : MonoBehaviour
    {
        public static void sharedUpdate()
        {
            if (!ControllerInput.leftController.isValid)
            {
                ControllerInput.leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            }
            if (!ControllerInput.rightController.isValid)
            {
                ControllerInput.rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            }
            try
            {
                ControllerInput.leftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out ControllerInput.isLeftControllerPrimaryAxisClick);
                ControllerInput.leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out ControllerInput.LeftControllerPrimaryAxis);
                ControllerInput.leftController.TryGetFeatureValue(CommonUsages.gripButton, out ControllerInput.isLeftControllerGripping);
                ControllerInput.leftController.TryGetFeatureValue(CommonUsages.triggerButton, out ControllerInput.isLeftControllerTrigger);
                ControllerInput.leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out ControllerInput.isLeftControllerSecondary);
                ControllerInput.leftController.TryGetFeatureValue(CommonUsages.primaryButton, out ControllerInput.isLeftControllerPrimary);
                ControllerInput.isHoldingLeftGrip = ControllerInput.isLeftControllerGripping;
                ControllerInput.isHoldingLeftPrimary = ControllerInput.isLeftControllerPrimary;
                ControllerInput.isHoldingLeftSecondary = ControllerInput.isLeftControllerSecondary;
                ControllerInput.isHoldingLeftTrigger = ControllerInput.isLeftControllerTrigger;
            }
            catch (Exception)
            {
            }
            try
            {
                ControllerInput.rightController.TryGetFeatureValue(CommonUsages.gripButton, out ControllerInput.isRightControllerGripping);
                ControllerInput.rightController.TryGetFeatureValue(CommonUsages.triggerButton, out ControllerInput.isRightControllerTrigger);
                ControllerInput.rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out ControllerInput.isRightControllerSecondary);
                ControllerInput.rightController.TryGetFeatureValue(CommonUsages.primaryButton, out ControllerInput.isRightControllerPrimary);
                ControllerInput.isHoldingRightGrip = ControllerInput.isRightControllerGripping;
                ControllerInput.isHoldingRightPrimary = ControllerInput.isRightControllerPrimary;
                ControllerInput.isHoldingRightSecondary = ControllerInput.isRightControllerSecondary;
                ControllerInput.isHoldingRightTrigger = ControllerInput.isRightControllerTrigger;
            }
            catch (Exception)
            {
            }
        }

        public static bool isRightControllerGripping;

        public static bool isRightControllerTrigger;

        public static bool isRightControllerSecondary;

        public static bool isRightControllerPrimary;

        public static bool isLeftControllerGripping;

        public static bool isLeftControllerTrigger;

        public static bool isLeftControllerSecondary;

        public static bool isLeftControllerPrimary;

        public static bool isLeftControllerPrimaryAxisClick;

        public static Vector2 LeftControllerPrimaryAxis;

        private static InputDevice leftController;

        private static InputDevice rightController;

        public static bool isHoldingRightGrip;

        public static bool isHoldingLeftGrip;

        public static bool isHoldingRightTrigger;

        public static bool isHoldingLeftTrigger;

        public static bool isHoldingRightPrimary;

        public static bool isHoldingRightSecondary;

        public static bool isHoldingLeftPrimary;

        public static bool isHoldingLeftSecondary;
        
    }
    #endregion
}
