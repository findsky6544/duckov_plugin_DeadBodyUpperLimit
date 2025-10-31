using Duckov;
using Duckov.Rules;
using Duckov.UI;
using Duckov.Utilities;
using HarmonyLib;
using ItemStatsSystem;
using ItemStatsSystem.Data;
using ItemStatsSystem.Items;
using Saves;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static Duckov.DeadBodyManager;

namespace DeadBodyUpperLimit
{
    public class Patch
    {
        //修改尸体上限
        [HarmonyPatch(typeof(Ruleset), "SaveDeadbodyCount", MethodType.Getter)]
        public class Patch_Ruleset_SaveDeadbodyCount
        {
            static bool Prefix(Ruleset __instance, ref int __result)
            {
                Console.WriteLine("Patch_Ruleset_SaveDeadbodyCount");
                __result = ModBehaviour.SaveDeadbodyCount;
                return false;
            }
        }

        //始终生成尸体，包括极限模式
        [HarmonyPatch(typeof(Ruleset), "SpawnDeadBody", MethodType.Getter)]
        public class Patch_Ruleset_SpawnDeadBody
        {
            static bool Prefix(Ruleset __instance, ref bool __result)
            {
                Console.WriteLine("Patch_Ruleset_SpawnDeadBody");
                __result = true;
                return false;
            }
        }

        private static List<DeathInfo>? deaths;
        private static DeathInfo? deathInfo;
        //尸体摸空后才消失
        [HarmonyPatch(typeof(DeadBodyManager), "OnDeadbodyTouched", new Type[] { typeof(DeathInfo) })]
        public class Patch_DeadBodyManager_OnDeadbodyTouched
        {
            static bool Prefix(DeadBodyManager __instance, DeathInfo info)
            {
                Console.WriteLine("Patch_DeadBodyManager_OnDeadbodyTouched");
                deaths = Traverse.Create(__instance).Field("deaths").GetValue<List<DeathInfo>>();

                deathInfo = deaths.Find((DeathInfo e) => e.raidID == info.raidID);
                return false;
            }
        }
        //从尸体拿东西到背包
        [HarmonyPatch(typeof(LootView), "OnLootTargetItemDoubleClicked")]
        public class Patch_LootView_OnLootTargetItemDoubleClicked
        {
            static bool Prefix(LootView __instance, InventoryEntry entry)
            {
                Console.WriteLine("Patch_LootView_OnLootTargetItemDoubleClicked_prefix");
                Item item = entry.Item;
                if (item == null)
                {
                    return true;
                }
                if (deathInfo == null)
                {
                    return true;
                }
                ItemStatsSystem.Data.ItemTreeData.DataEntry deTemp = deathInfo.itemTreeData.entries.Find((x) => (x.typeID == item.TypeID && x.StackCount == item.StackCount));
                if (deTemp != null)
                {
                    deathInfo.itemTreeData.entries.Remove(deTemp);
                }
                return true;
            }
            //尸体摸空则不再生成
            static void Postfix(LootView __instance, InventoryEntry entry)
            {
                Console.WriteLine("Patch_LootView_OnLootTargetItemDoubleClicked_postfix");
                Console.WriteLine("__instance.TargetInventory.GetItemCount():" + __instance.TargetInventory.GetItemCount());
                if (__instance.TargetInventory.GetItemCount() == 0 && deathInfo != null)
                {
                    deathInfo.touched = true;
                    SavesSystem.Save<List<DeadBodyManager.DeathInfo>>("DeathList", deaths);
                }
            }
        }

        [HarmonyPatch(typeof(LootView), "OnClose")]
        public class Patch_LootView_OnClose
        {
            static bool Prefix(LootView __instance)
            {
                Console.WriteLine("Patch_LootView_OnClose");

                deaths = null;
                deathInfo = null;
                return true;
            }
        }
    }
}
