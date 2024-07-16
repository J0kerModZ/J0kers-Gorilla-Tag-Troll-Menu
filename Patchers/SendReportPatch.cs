using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J0kersTrollMenu.Patchers
{
    [HarmonyPatch(typeof(GorillaNot), "SendReport")]
    internal class SendReportPatch
    {
        private static bool Prefix(string susReason, string susId, string susNick)
        {
            return false;
        }
    }
}
