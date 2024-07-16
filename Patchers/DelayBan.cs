using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace J0kersTrollMenu.Patchers
{
    [HarmonyPatch(typeof(GorillaGameManager), "ForceStopGame_DisconnectAndDestroy")]
    internal class DelayBan : MonoBehaviour
    {
        public static bool Prefix()
        {
            return false;
        }
    }
}