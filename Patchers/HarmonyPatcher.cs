using BepInEx;

namespace J0kersTrollMenu.Patchers
{
    [BepInPlugin("com.J0ker.Patch", "J0kerPatcher", "1.0.0")]
    internal class HarmonyPatcher : BaseUnityPlugin
    {
        private void OnEnable()
        {
            MenuPatch.ApplyHarmonyPatches();
        }

        private void OnDisable()
        {
            MenuPatch.RemoveHarmonyPatches();
        }
    }
}