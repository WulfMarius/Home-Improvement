using UnityEngine;

namespace HomeImprovement
{
    [DisallowMultipleComponent]
    public abstract class Repairable : MonoBehaviour
    {
        public LocalizedString InteractiveDisplayText = new LocalizedString()
        {
            m_LocalizationID = "GAMEPLAY_Repair"
        };

        public bool RequiresTools;
        public bool Applied;

        public virtual string GetInteractiveObjectDisplayText()
        {
            if (this.InteractiveDisplayText == null)
            {
                return "";
            }

            return this.InteractiveDisplayText.Text();
        }

        public abstract bool ProcessInteraction();

        public void Repair()
        {
            if (this.PerformRepair())
            {
                Applied = true;
                Destroy(this);
            }
        }

        public void DegradeTools()
        {
            if (RequiresTools)
            {
                GearItem tools = GameManager.GetInventoryComponent().GetBestGearItemWithName("GEAR_SimpleTools");
                if (tools != null)
                {
                    tools.DegradeOnUse();
                    return;
                }

                tools = GameManager.GetInventoryComponent().GetBestGearItemWithName("GEAR_HighQualityTools");
                if (tools != null)
                {
                    tools.DegradeOnUse();
                    return;
                }
            }
        }

        internal void StartProgressBar(string displayTextKey, string audio, float seconds)
        {
            InterfaceManager.m_Panel_GenericProgressBar.Launch(Localization.Get(displayTextKey), seconds, 5, 0.0f, audio, null, true, true, this.OnRepairedFinished);
        }

        internal void OnRepairedFinished(bool success, bool playerCancel, float progress)
        {
            if (success)
            {
                this.Repair();
                this.DegradeTools();
            }
        }

        internal abstract bool PerformRepair();
    }
}