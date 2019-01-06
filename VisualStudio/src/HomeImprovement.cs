using ModComponentMapper;
using System.Reflection;
using UnityEngine;

namespace HomeImprovement
{
    public class HomeImprovement
    {
        public const string NAME = "Home-Improvement";

        private static HomeImprovementSettings settings;

        internal static bool RemovableCampfires
        {
            get { return settings.RemovableCampfires; }
        }

        internal static bool RemovableCorpses
        {
            get { return settings.RemovableCorpses; }
        }

        public static void OnLoad()
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            Log("Version " + assemblyName.Version);

            RepairManager.Initialize();

            settings = HomeImprovementSettings.Load();
            settings.AddToModSettings(NAME, ModSettings.MenuType.MainMenuOnly);

            ModComponentMapper.Implementation.OnSceneReady += PrepareScene;
        }

        internal static void Log(string message)
        {
            Debug.LogFormat("[" + NAME + "] {0}", message);
        }

        internal static void Log(string message, params object[] parameters)
        {
            string preformattedMessage = string.Format("[" + NAME + "] {0}", message);
            Debug.LogFormat(preformattedMessage, parameters);
        }

        internal static void PrepareScene()
        {
            if (ModUtils.IsNonGameScene())
            {
                return;
            }

            CleanupManager.PrepareCleanables();
            RepairManager.PrepareRepairables();
        }
    }
}