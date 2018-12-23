using Harmony;
using UnityEngine;

namespace HomeImprovement
{
    [HarmonyPatch(typeof(Panel_ActionPicker), "TakeCharcoalCallback")]
    internal class Panel_ActionPicker_TakeCharcoalCallback
    {
        private static void Postfix(Panel_ActionPicker __instance)
        {
            if (!HomeImprovement.RemovableCampfires)
            {
                return;
            }

            GameObject gameObject = Traverse.Create(__instance).Field("m_ObjectInteractedWith").GetValue() as GameObject;
            if (gameObject == null || gameObject.GetComponent<Campfire>() == null)
            {
                return;
            }

            Fire fire = gameObject.GetComponent<Fire>();
            if (fire == null)
            {
                return;
            }

            FireManager.DestroyFireObject(fire);
        }
    }
}