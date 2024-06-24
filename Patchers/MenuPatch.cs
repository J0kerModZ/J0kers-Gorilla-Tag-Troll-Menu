using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace J0kersTrollMenu.Patchers
{
    internal class MenuPatch
    {
        public static bool IsPatched { get; private set; }

        private static Harmony instance;

        public const string InstanceId = "J0ker.Patchers";

        internal static void ApplyHarmonyPatches()
        {
            if (!MenuPatch.IsPatched)
            {
                if (MenuPatch.instance == null)
                {
                    MenuPatch.instance = new Harmony("J0ker.Patchers");
                }
                MenuPatch.instance.PatchAll(Assembly.GetExecutingAssembly());
                MenuPatch.IsPatched = true;
            }
        }

        internal static void RemoveHarmonyPatches()
        {
            if (MenuPatch.instance != null && MenuPatch.IsPatched)
            {
                MenuPatch.IsPatched = false;
            }
        }
    }
}

