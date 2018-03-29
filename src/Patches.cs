using Harmony;
using UnityEngine;

namespace HomeImprovement
{
    [HarmonyPatch(typeof(GameManager), "Start")]
    internal class GameManager_Awake
    {
        public static void Postfix()
        {
            RepairManager.PrepareRepairables();
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "GetInteractiveObjectDisplayText")]
    internal class PlayerManager_GetInteractiveObjectDisplayText
    {
        public static void Postfix(GameObject interactiveObject, ref string __result)
        {
            if (interactiveObject == null)
            {
                return;
            }

            Repairable repairable = interactiveObject.GetComponent<Repairable>();
            if (repairable != null)
            {
                __result = repairable.GetInteractiveObjectDisplayText();
            }
        }
    }

    [HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessInteraction")]
    internal class PlayerManager_InteractiveObjectsProcessInteraction
    {
        public static void Postfix(PlayerManager __instance, ref bool __result)
        {
            if (__result || __instance.m_InteractiveObjectUnderCrosshair == null)
            {
                return;
            }

            Repairable repairable = __instance.m_InteractiveObjectUnderCrosshair.GetComponent<Repairable>();
            if (repairable != null)
            {
                __result = repairable.ProcessInteraction();
            }
        }
    }

    [HarmonyPatch(typeof(SaveGameSystem), "LoadSceneData")]
    internal class SaveGameSystem_LoadSceneData
    {
        public static void Prefix(string name, string sceneSaveName)
        {
            RepairManager.LoadRepairs(name, sceneSaveName);
        }
    }

    [HarmonyPatch(typeof(SaveGameSystem), "SaveSceneData")]
    internal class SaveGameSystem_SaveSceneData
    {
        public static void Prefix(SaveSlotType gameMode, string name, string sceneSaveName)
        {
            RepairManager.SaveRepairs(gameMode, name, sceneSaveName);
        }
    }
}