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

        internal void StartProgressBar(string displayTextKey, string audio, float seconds)
        {
            InterfaceManager.m_Panel_GenericProgressBar.Launch(Localization.Get(displayTextKey), seconds, 5, 0.0f, audio, null, true, true, this.OnRepairedFinished);
        }

        internal void OnRepairedFinished(bool success, bool playerCancel, float progress)
        {
            if (success)
            {
                this.Repair();
            }
        }

        internal abstract bool PerformRepair();
    }
}