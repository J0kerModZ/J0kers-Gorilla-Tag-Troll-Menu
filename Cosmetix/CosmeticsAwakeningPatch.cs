using GorillaNetworking;
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
    [HarmonyPatch(typeof(CosmeticsController))]
    [HarmonyPatch("Awake", 0)]
    internal class CosmeticsAwakeningPatch
    {
        // Token: 0x0600000D RID: 13 RVA: 0x000021A4 File Offset: 0x000003A4
        private static bool Prefix(CosmeticsController __instance, ref int[] ___cosmeticsPages, ref string ___catalog, ref string ___currencyName, ref CosmeticsController.CosmeticItem ___nullItem, ref List<CosmeticsController.CosmeticItem> ___allCosmetics, ref Dictionary<string, CosmeticsController.CosmeticItem> ___allCosmeticsDict, ref Dictionary<string, string> ___allCosmeticsItemIDsfromDisplayNamesDict, ref CosmeticsController.CosmeticSet ___tryOnSet, ref List<CosmeticsController.CosmeticItem> ___unlockedHats, ref List<CosmeticsController.CosmeticItem> ___unlockedBadges, ref List<CosmeticsController.CosmeticItem> ___unlockedFaces, ref List<CosmeticsController.CosmeticItem> ___unlockedHoldable, ref List<CosmeticsController.CosmeticItem>[] ___itemLists)
        {
            bool flag = CosmeticsController.instance == null;
            if (flag)
            {
                CosmeticsController.instance = __instance;
            }
            else
            {
                bool flag2 = CosmeticsController.instance != __instance;
                if (flag2)
                {
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }
            }
            bool activeSelf = __instance.gameObject.activeSelf;
            if (activeSelf)
            {
                ___catalog = "DLC";
                ___currencyName = "SR";
                ___nullItem = ___allCosmetics[0];
                ___nullItem.isNullItem = true;
                ___allCosmeticsDict[___nullItem.itemName] = ___nullItem;
                ___allCosmeticsItemIDsfromDisplayNamesDict[___nullItem.displayName] = ___nullItem.itemName;
                for (int i = 0; i < 10; i++)
                {
                    ___tryOnSet.items[i] = ___nullItem;
                }
                ___cosmeticsPages[0] = 0;
                ___cosmeticsPages[1] = 0;
                ___cosmeticsPages[2] = 0;
                ___cosmeticsPages[3] = 0;
                ___itemLists[0] = new List<CosmeticsController.CosmeticItem>();
                ___itemLists[1] = new List<CosmeticsController.CosmeticItem>();
                ___itemLists[2] = new List<CosmeticsController.CosmeticItem>();
                ___itemLists[3] = new List<CosmeticsController.CosmeticItem>();
                __instance.SwitchToStage(CosmeticsController.ATMStages.Unavailable);
                __instance.StartCoroutine((IEnumerator<object>)AccessTools.Method(typeof(CosmeticsController), "CheckCanGetDaily", null, null).Invoke(__instance, null));
            }
            return false;
        }
    }
}
