using System;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalCompany_Advanced_Teleporter
{
    [BepInPlugin("Void.LethalCompanyAdvancedTeleporter", "Advanced Teleporter", "1.1.0.0")]
    public class AdvancedTeleporter : BaseUnityPlugin
    {
        private Harmony harmony;
        private static bool isTeleporting;

        private void Awake()
        {
            harmony = new Harmony("Void.LethalCompanyAdvancedTeleporter");
            harmony.PatchAll(typeof(AdvancedTeleporterPatches));
            Logger.LogInfo("Advanced Teleporter v1.1.0.0 has been loaded successfully!");
        }

        private void OnDestroy() => harmony.UnpatchSelf();

        public static void SetTeleporting(bool state) => isTeleporting = state;

        [HarmonyPatch]
        private static class AdvancedTeleporterPatches
        {
            // Assume this method is called when the teleportation starts
            [HarmonyPatch(typeof(ShipTeleporter), "BeginTeleportationSequence"), HarmonyPrefix]
            private static void PrefixStartTeleport() => AdvancedTeleporter.SetTeleporting(true);

            // Assume this method is called when the teleportation ends
            [HarmonyPatch(typeof(ShipTeleporter), "EndTeleportationSequence"), HarmonyPostfix]
            private static void PostfixOnTeleport() => AdvancedTeleporter.SetTeleporting(false);

            // Prevent dropping items if teleporting
            [HarmonyPatch(typeof(PlayerControllerB), "DropAllHeldItems"), HarmonyPrefix]
            private static bool PrefixDropAllHeldItems() =>!isTeleporting;

            // Prevent updating item holding if teleporting
            [HarmonyPatch(typeof(PlayerControllerB), "UpdateItemHoldingState"), HarmonyPrefix]
            private static bool PrefixUpdateItemHolding() =>!isTeleporting;

            // New patch for version 50: Prevent teleportation cooldown if teleporting
            [HarmonyPatch(typeof(ShipTeleporter), "StartTeleportationCooldown"), HarmonyPrefix]
            private static bool PrefixStartTeleportationCooldown() =>!isTeleporting;
        }
    }
}