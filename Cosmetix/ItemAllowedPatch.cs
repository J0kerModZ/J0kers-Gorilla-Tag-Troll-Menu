using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J0kersTrollMenu.Cosmetix
{
    // RIPPED FROM DNSPY!!! 
    // YES skidded =(
    [HarmonyPatch(typeof(VRRig))]
    [HarmonyPatch("IsItemAllowed", 0)]
    internal class ItemAllowedPatch
    {
        // Token: 0x0600000F RID: 15 RVA: 0x00002300 File Offset: 0x00000500
        private static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
