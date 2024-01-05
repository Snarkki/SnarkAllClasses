using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityModManagerNet;

namespace SnarkAllClasses
{
#if DEBUG
    [EnableReloading]
#endif
    partial class Main
    {


        internal static Main Instance = null!;

        internal Main(UnityModManager.ModEntry modEntry)
        {
            ModEntry = modEntry;
            this.Harmony = new Harmony(modEntry.Info.Id);

            modEntry.OnUnload = OnUnload;
            modEntry.OnGUI = OnGUI;
        }

        readonly Harmony Harmony;

        public static Harmony HarmonyInstance => Instance.Harmony;

        readonly UnityModManager.ModEntry ModEntry;

        private static UnityModManager.ModEntry.ModLogger Logger => Instance.ModEntry.Logger;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Instance = new(modEntry);


            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {

        }

        static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance.UnpatchAll(modEntry.Info.Id);
            Instance = null!;
            return true;
        }

    }
}