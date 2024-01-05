using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnarkAllClasses
{
    internal class ContentAdder
    {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            [HarmonyPriority(Priority.LowerThanNormal)]
            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;

                ClassPatcher.ChangeBlueprints();

            }
        }
    }
}