using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J0kersTrollMenu.Cosmetix
{
    // RIPPED FROM DNSPY!!! 
    // YES skidded =(
    [HarmonyPatch(typeof(PhotonView), "RPC", new Type[]
    {
        typeof(string),
        typeof(RpcTarget),
        typeof(object[])
    })]
    internal class PhotonViewPatch
    {
        // Token: 0x06000011 RID: 17 RVA: 0x00002320 File Offset: 0x00000520
        private static bool Prefix(PhotonView __instance, ref string methodName)
        {
            bool flag = methodName == "UpdateCosmeticsWithTryon" || methodName == "UpdatePlayerCosmetic";
            return !flag;
        }
    }
}
