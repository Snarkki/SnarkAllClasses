using HarmonyLib;
using System.Reflection;
using UnityModManagerNet;

namespace SnarkAllClasses;

#if DEBUG
[EnableReloading]
#endif
static class Main
{
    internal static Harmony HarmonyInstance;
    internal static UnityModManager.ModEntry.ModLogger log;

    static bool Load(UnityModManager.ModEntry modEntry)
    {
        log = modEntry.Logger;
#if DEBUG
        modEntry.OnUnload = OnUnload;
#endif
        modEntry.OnGUI = OnGUI;
        HarmonyInstance = new Harmony(modEntry.Info.Id);
        Harmony.DEBUG = false;
        //AnimationSkipper.ApplyPatch(HarmonyInstance);
        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

        return true;
    }

    static void OnGUI(UnityModManager.ModEntry modEntry)
    {

    }

#if DEBUG
    static bool OnUnload(UnityModManager.ModEntry modEntry)
    {

        HarmonyInstance.UnpatchAll(modEntry.Info.Id);
        return true;
    }
#endif


}