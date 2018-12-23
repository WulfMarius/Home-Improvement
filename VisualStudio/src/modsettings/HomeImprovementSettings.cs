using ModSettings;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace HomeImprovement
{
    internal class HomeImprovementSettings : ModSettingsBase
    {
        [Name("Removable Campfires")]
        [Description("If enabled campfires will be removed when taking their charcoal.")]
        public bool RemovableCampfires = false;

        [Name("Removable Corpses")]
        [Description("If enabled corpses can be removed with a right click.")]
        public bool RemovableCorpses = false;

        private static readonly string MODS_FOLDER_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string SETTINGS_PATH = Path.Combine(MODS_FOLDER_PATH, "Home-Improvement.json");

        internal static HomeImprovementSettings Load()
        {
            if (!File.Exists(SETTINGS_PATH))
            {
                HomeImprovement.Log("Settings file did not exist, using default settings.");
                return new HomeImprovementSettings();
            }

            try
            {
                string json = File.ReadAllText(SETTINGS_PATH, System.Text.Encoding.UTF8);
                return JsonUtility.FromJson<HomeImprovementSettings>(json);
            }
            catch (Exception ex)
            {
                HomeImprovement.Log("Error while trying to read config file:");
                Debug.LogException(ex);

                // Re-throw to make error show up in main menu
                throw new IOException("Error while trying to read config file", ex);
            }
        }

        internal void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(this, true);
                File.WriteAllText(SETTINGS_PATH, json, System.Text.Encoding.UTF8);
                HomeImprovement.Log("Config file saved to " + SETTINGS_PATH);
            }
            catch (Exception ex)
            {
                HomeImprovement.Log("Error while trying to write config file:");
                Debug.LogException(ex);
            }
        }

        protected override void OnConfirm()
        {
            this.Save();
        }
    }
}