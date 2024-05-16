using BepInEx;
using HarmonyLib;
using UnityEngine;
using J0kersTrollMenu.MenuMods;
using J0kersTrollMenu.ModMenu;
using J0kersTrollMenu.Patchers;

namespace J0kersTrollMenu.ModMenu
{
    [BepInPlugin("com.J0kerMenu.J0kerModZ", "J0kerMenu", "1.0.0")]
    [HarmonyPatch(typeof(GorillaLocomotion.Player), "LateUpdate", MethodType.Normal)]
    public class Loader : BaseUnityPlugin
    {
        public void FixedUpdate()
        {
            if (!GameObject.Find("J0kerLoader") && GorillaLocomotion.Player.hasInstance)
            {
                GameObject Loader = new GameObject("J0kerLoader");
                Loader.AddComponent<Plugin>();
                Loader.AddComponent<Mods>();
                Loader.AddComponent<AntiLag>();
            }
        }
    }
}
