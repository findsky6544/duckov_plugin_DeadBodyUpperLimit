using Duckov;
using Duckov.Rules;
using Duckov.UI;
using HarmonyLib;
using ItemStatsSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeadBodyUpperLimit
{

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

    [HarmonyPatch(typeof(DeadBodyManager), "OnSubSceneLoaded")]
    public class Patch_DeadBodyManager_OnSubSceneLoaded
    {
        static bool Prefix(DeadBodyManager __instance)
        {
            Console.WriteLine("Patch_DeadBodyManager_OnSubSceneLoaded");
            return true;
        }
    }
}
