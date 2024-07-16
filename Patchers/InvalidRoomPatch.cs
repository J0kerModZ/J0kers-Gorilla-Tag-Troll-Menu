using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J0kersTrollMenu.Patchers
{
    [HarmonyPatch(typeof(GorillaNot), "CloseInvalidRoom")]

    internal class InvalidRoomPatch
    {
        private static bool Prefix()
        {
            return false;
        }
    }
}
