using System;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalCompany_Advanced_Teleporter
{
    [BepInPlugin("Void.LethalCompanyAdvancedTeleporter", "Advanced Teleporter", "1.0.0.0")]
    public class AdvancedTeleporter : BaseUnityPlugin
    {
        private Harmony harmony;
        private static bool isTeleporting;

        private void Awake()
        {
            harmony = new Harmony("Void.LethalCompanyAdvancedTeleporter");
            harmony.PatchAll(typeof(AdvancedTeleporterPatches));
            Logger.LogInfo("Advanced Teleporter v1.0.0.0 has been loaded successfully!");
        }

        private void OnDestroy() => harmony.UnpatchSelf();

        public static void SetTeleporting(bool state) => isTeleporting = state;

        [HarmonyPatch]
        private static class AdvancedTeleporterPatches
        {
            // Assume this method is called when the teleportation starts
            [HarmonyPatch(typeof(ShipTeleporter), "StartTeleport"), HarmonyPrefix]
            private static void PrefixStartTeleport() => AdvancedTeleporter.SetTeleporting(true);

            // Assume this method is called when the teleportation ends
            [HarmonyPatch(typeof(ShipTeleporter), "OnTeleport"), HarmonyPostfix]
            private static void PostfixOnTeleport() => AdvancedTeleporter.SetTeleporting(false);

            // Prevent dropping items if teleporting
            [HarmonyPatch(typeof(PlayerControllerB), "DropAllHeldItems"), HarmonyPrefix]
            private static bool PrefixDropAllHeldItems() => !isTeleporting;

           
        }
    }
}
