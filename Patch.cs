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
}

