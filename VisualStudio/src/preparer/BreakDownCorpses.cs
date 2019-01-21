using ModComponentAPI;
using UnityEngine;

namespace HomeImprovement.Preparer
{
    public class CorpseRemover : AlternativeAction
    {
        public override void Execute()
        {
            BreakDown breakDown = this.gameObject.GetComponent<BreakDown>();
            breakDown.ProcessInteraction();
        }

        public void OnDisable()
        {
            if (this.gameObject.activeInHierarchy)
            {
                return;
            }

            EmitterProxy[] emitterProxies = this.gameObject.GetComponentsInChildren<EmitterProxy>();
            foreach (var eachEmitterProxy in emitterProxies)
            {
                GameAudioManager.StopAllSoundsFromGameObject(eachEmitterProxy.m_Proxy);
            }
        }
    }

    internal class BreakDownCorpses : AbstractScenePreparer
    {
        protected override bool Disabled => !HomeImprovement.RemovableCorpses;

        protected override SearchResult Accept(GameObject gameObject)
        {
            if (!gameObject.activeInHierarchy || gameObject.layer == vp_Layer.Gear)
            {
                return SearchResult.SKIP_CHILDREN;
            }

            if (gameObject.name.StartsWith("CORPSE_Human") || gameObject.name.StartsWith("CORPSE_Story_"))
            {
                return SearchResult.INCLUDE_SKIP_CHILDREN;
            }

            return SearchResult.CONTINUE;
        }

        protected override bool Prepare(GameObject gameObject)
        {
            Renderer renderer = Utils.GetLargestBoundsRenderer(gameObject);
            if (renderer == null)
            {
                return false;
            }

            gameObject.AddComponent<CorpseRemover>();

            BreakDown breakDown = gameObject.AddComponent<BreakDown>();
            breakDown.enabled = false;
            breakDown.m_YieldObject = new GameObject[0];
            breakDown.m_YieldObjectUnits = new int[0];
            breakDown.m_TimeCostHours = 3;
            breakDown.m_BreakDownAudio = "PLAY_REMOVECORPSE";
            breakDown.m_LocalizedDisplayName = new LocalizedString() { m_LocalizationID = "GAMEPLAY_Corpse" };
            breakDown.m_UsableTools = new GameObject[0];

            return true;
        }
    }
}