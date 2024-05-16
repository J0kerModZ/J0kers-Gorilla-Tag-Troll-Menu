using System;
using System.Collections.Generic;
using GorillaNetworking;
using HarmonyLib;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace J0kersTrollMenu.Cosmetix
{
    // RIPPED FROM DNSPY!!! 
    // YES skidded =(
    [HarmonyPatch(typeof(CosmeticsController))]
    [HarmonyPatch("GetUserCosmeticsAllowed", 0)]
    internal class CosmeticsAllowedPatch
    {
        // Token: 0x0600000B RID: 11 RVA: 0x00002164 File Offset: 0x00000364
        private static bool Prefix(CosmeticsController __instance, ref List<CosmeticsController.CosmeticItem> ___unlockedCosmetics, ref List<CosmeticsController.CosmeticItem> ___unlockedHats, ref List<CosmeticsController.CosmeticItem> ___unlockedBadges, ref List<CosmeticsController.CosmeticItem> ___unlockedFaces, ref List<CosmeticsController.CosmeticItem> ___unlockedHoldable, ref List<CosmeticsController.CosmeticItem> ___allCosmetics, ref Dictionary<string, CosmeticsController.CosmeticItem> ___allCosmeticsDict, ref Dictionary<string, string> ___allCosmeticsItemIDsfromDisplayNamesDict, ref string ___concatStringCosmeticsAllowed, ref bool ___playedInBeta, ref int ___currencyBalance, ref List<CosmeticsController.CosmeticItem>[] ___itemLists)
        {
            new CosmeticsAllowedPatch.CosmetxController(__instance, ref ___unlockedCosmetics, ref ___unlockedHats, ref ___unlockedBadges, ref ___unlockedFaces, ref ___unlockedHoldable, ref ___allCosmetics, ref ___allCosmeticsDict, ref ___allCosmeticsItemIDsfromDisplayNamesDict, ref ___concatStringCosmeticsAllowed, ref ___playedInBeta, ref ___currencyBalance, ref ___itemLists).GetUserCosmeticsAllowed();
            return false;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002198 File Offset: 0x00000398
        public CosmeticsAllowedPatch()
        {
        }

        // Token: 0x02000009 RID: 9
        private class CosmetxController
        {
            // Token: 0x06000013 RID: 19 RVA: 0x00002364 File Offset: 0x00000564
            internal CosmetxController(CosmeticsController instance, ref List<CosmeticsController.CosmeticItem> unlockedCosmetics, ref List<CosmeticsController.CosmeticItem> unlockedHats, ref List<CosmeticsController.CosmeticItem> unlockedBadges, ref List<CosmeticsController.CosmeticItem> unlockedFaces, ref List<CosmeticsController.CosmeticItem> unlockedHoldable, ref List<CosmeticsController.CosmeticItem> allCosmetics, ref Dictionary<string, CosmeticsController.CosmeticItem> allCosmeticsDict, ref Dictionary<string, string> allCosmeticsItemIDsfromDisplayNamesDict, ref string concatStringCosmeticsAllowed, ref bool playedInBeta, ref int currencyBalance, ref List<CosmeticsController.CosmeticItem>[] itemLists)
            {
                this.instance = instance;
                this.unlockedCosmetics = unlockedCosmetics;
                this.unlockedHats = unlockedHats;
                this.unlockedBadges = unlockedBadges;
                this.unlockedFaces = unlockedFaces;
                this.unlockedHoldable = unlockedHoldable;
                this.allCosmetics = allCosmetics;
                this.allCosmeticsDict = allCosmeticsDict;
                this.allCosmeticsItemIDsfromDisplayNamesDict = allCosmeticsItemIDsfromDisplayNamesDict;
                this.concatStringCosmeticsAllowed = concatStringCosmeticsAllowed;
                this.playedInBeta = playedInBeta;
                this.currencyBalance = currencyBalance;
                this.itemLists = itemLists;
            }

            // Token: 0x06000014 RID: 20 RVA: 0x000023EA File Offset: 0x000005EA
            internal void GetUserCosmeticsAllowed()
            {
                PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate (GetUserInventoryResult result)
                {
                    GetCatalogItemsRequest getCatalogItemsRequest = new GetCatalogItemsRequest();
                    getCatalogItemsRequest.CatalogVersion = "DLC";
                    PlayFabClientAPI.GetCatalogItems(getCatalogItemsRequest, delegate (GetCatalogItemsResult result2)
                    {
                        this.unlockedCosmetics.Clear();
                        this.unlockedHats.Clear();
                        this.unlockedBadges.Clear();
                        this.unlockedFaces.Clear();
                        this.unlockedHoldable.Clear();
                        List<CatalogItem> catalog = result2.Catalog;
                        using (List<CatalogItem>.Enumerator enumerator = catalog.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                CatalogItem catalogItem = enumerator.Current;
                                this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => catalogItem.DisplayName == x.displayName);
                                bool flag = this.searchIndex > -1;
                                if (flag)
                                {
                                    string[] bundledItems = null;
                                    bool canTryOn = false;
                                    bool flag2 = catalogItem.Bundle != null;
                                    if (flag2)
                                    {
                                        bundledItems = catalogItem.Bundle.BundledItems.ToArray();
                                    }
                                    uint cost;
                                    bool flag3 = catalogItem.VirtualCurrencyPrices.TryGetValue("SR", out cost);
                                    if (flag3)
                                    {
                                        canTryOn = true;
                                    }
                                    this.allCosmetics[this.searchIndex] = new CosmeticsController.CosmeticItem
                                    {
                                        itemName = catalogItem.ItemId,
                                        displayName = catalogItem.DisplayName,
                                        cost = (int)cost,
                                        itemPicture = this.allCosmetics[this.searchIndex].itemPicture,
                                        itemCategory = this.allCosmetics[this.searchIndex].itemCategory,
                                        bundledItems = bundledItems,
                                        canTryOn = canTryOn,
                                        bothHandsHoldable = this.allCosmetics[this.searchIndex].bothHandsHoldable,
                                        overrideDisplayName = this.allCosmetics[this.searchIndex].overrideDisplayName
                                    };
                                    this.allCosmeticsDict[this.allCosmetics[this.searchIndex].itemName] = this.allCosmetics[this.searchIndex];
                                    this.allCosmeticsItemIDsfromDisplayNamesDict[this.allCosmetics[this.searchIndex].displayName] = this.allCosmetics[this.searchIndex].itemName;
                                }
                            }
                        }
                        for (int i = this.allCosmetics.Count - 1; i > -1; i--)
                        {
                            this.tempItem = this.allCosmetics[i];
                            bool flag4 = this.tempItem.itemCategory == CosmeticsController.CosmeticCategory.Set && this.tempItem.canTryOn;
                            if (flag4)
                            {
                                string[] bundledItems2 = this.tempItem.bundledItems;
                                for (int j = 0; j < bundledItems2.Length; j++)
                                {
                                    string setItemName = bundledItems2[j];
                                    this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName == x.itemName);
                                    bool flag5 = this.searchIndex > -1;
                                    if (flag5)
                                    {
                                        this.tempItem = new CosmeticsController.CosmeticItem
                                        {
                                            itemName = this.allCosmetics[this.searchIndex].itemName,
                                            displayName = this.allCosmetics[this.searchIndex].displayName,
                                            cost = this.allCosmetics[this.searchIndex].cost,
                                            itemPicture = this.allCosmetics[this.searchIndex].itemPicture,
                                            itemCategory = this.allCosmetics[this.searchIndex].itemCategory,
                                            overrideDisplayName = this.allCosmetics[this.searchIndex].overrideDisplayName,
                                            bothHandsHoldable = this.allCosmetics[this.searchIndex].bothHandsHoldable,
                                            canTryOn = true
                                        };
                                        Debug.Log(this.allCosmetics[this.searchIndex].itemName);
                                        this.allCosmetics[this.searchIndex] = this.tempItem;
                                        this.allCosmeticsDict[this.allCosmetics[this.searchIndex].itemName] = this.allCosmetics[this.searchIndex];
                                        this.allCosmeticsItemIDsfromDisplayNamesDict[this.allCosmetics[this.searchIndex].displayName] = this.allCosmetics[this.searchIndex].itemName;
                                    }
                                }
                            }
                        }
                        this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => "Slingshot" == x.itemName);
                        this.allCosmeticsDict["Slingshot"] = this.allCosmetics[this.searchIndex];
                        this.allCosmeticsItemIDsfromDisplayNamesDict[this.allCosmetics[this.searchIndex].displayName] = this.allCosmetics[this.searchIndex].itemName;
                        foreach (CosmeticsController.CosmeticItem cosmeticItem in this.allCosmetics)
                        {
                            bool flag6 = cosmeticItem.itemName == "null" || cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Set;
                            if (!flag6)
                            {
                                this.unlockedCosmetics.Add(cosmeticItem);
                                bool flag7 = cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Hat && !this.unlockedHats.Contains(cosmeticItem);
                                if (flag7)
                                {
                                    this.unlockedHats.Add(cosmeticItem);
                                    this.itemLists[0].Add(cosmeticItem);
                                }
                                else
                                {
                                    bool flag8 = cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Face && !this.unlockedFaces.Contains(cosmeticItem);
                                    if (flag8)
                                    {
                                        this.unlockedFaces.Add(cosmeticItem);
                                        this.itemLists[1].Add(cosmeticItem);
                                    }
                                    else
                                    {
                                        bool flag9 = cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Badge && !this.unlockedBadges.Contains(cosmeticItem);
                                        if (flag9)
                                        {
                                            this.unlockedBadges.Add(cosmeticItem);
                                            this.itemLists[2].Add(cosmeticItem);
                                        }
                                        else
                                        {
                                            bool flag10 = cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Holdable && !this.unlockedHoldable.Contains(cosmeticItem);
                                            if (flag10)
                                            {
                                                this.unlockedHoldable.Add(cosmeticItem);
                                                this.itemLists[3].Add(cosmeticItem);
                                            }
                                            else
                                            {
                                                bool flag11 = cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Gloves && !this.unlockedHoldable.Contains(cosmeticItem);
                                                if (flag11)
                                                {
                                                    this.unlockedHoldable.Add(cosmeticItem);
                                                    this.itemLists[3].Add(cosmeticItem);
                                                }
                                                else
                                                {
                                                    bool flag12 = cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Slingshot && !this.unlockedHoldable.Contains(cosmeticItem);
                                                    if (flag12)
                                                    {
                                                        this.unlockedHoldable.Add(cosmeticItem);
                                                        this.itemLists[3].Add(cosmeticItem);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                this.concatStringCosmeticsAllowed += cosmeticItem.itemName;
                            }
                        }
                        foreach (CosmeticStand cosmeticStand in this.instance.cosmeticStands)
                        {
                            bool flag13 = cosmeticStand != null;
                            if (flag13)
                            {
                                cosmeticStand.InitializeCosmetic();
                            }
                        }
                        this.currencyBalance = result.VirtualCurrency[this.instance.currencyName];
                        int num;
                        this.playedInBeta = (result.VirtualCurrency.TryGetValue("TC", out num) && num > 0);
                        this.instance.currentWornSet.LoadFromPlayerPreferences(this.instance);
                        this.instance.SwitchToStage(CosmeticsController.ATMStages.Begin);
                        this.instance.ProcessPurchaseItemState(null, false);
                        this.instance.UpdateShoppingCart();
                        this.instance.UpdateCurrencyBoard();
                    }, delegate (PlayFabError a)
                    {
                    }, null, null);
                }, delegate (PlayFabError a)
                {
                }, null, null);
            }

            // Token: 0x04000006 RID: 6
            private CosmeticsController instance;

            // Token: 0x04000007 RID: 7
            private List<CosmeticsController.CosmeticItem> unlockedCosmetics;

            // Token: 0x04000008 RID: 8
            private List<CosmeticsController.CosmeticItem> unlockedHats;

            // Token: 0x04000009 RID: 9
            private List<CosmeticsController.CosmeticItem> unlockedBadges;

            // Token: 0x0400000A RID: 10
            private List<CosmeticsController.CosmeticItem> unlockedFaces;

            // Token: 0x0400000B RID: 11
            private List<CosmeticsController.CosmeticItem> unlockedHoldable;

            // Token: 0x0400000C RID: 12
            private List<CosmeticsController.CosmeticItem> allCosmetics;

            // Token: 0x0400000D RID: 13
            private Dictionary<string, CosmeticsController.CosmeticItem> allCosmeticsDict;

            // Token: 0x0400000E RID: 14
            private Dictionary<string, string> allCosmeticsItemIDsfromDisplayNamesDict;

            // Token: 0x0400000F RID: 15
            private string concatStringCosmeticsAllowed;

            // Token: 0x04000010 RID: 16
            private bool playedInBeta;

            // Token: 0x04000011 RID: 17
            private int currencyBalance;

            // Token: 0x04000012 RID: 18
            private List<CosmeticsController.CosmeticItem>[] itemLists;

            // Token: 0x04000013 RID: 19
            private int searchIndex;

            // Token: 0x04000014 RID: 20
            private CosmeticsController.CosmeticItem tempItem;
        }
    }
}