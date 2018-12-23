using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HomeImprovement
{
    public class HomeImprovement
    {
        public const string NAME = "Home-Improvement";
        private static HomeImprovementSettings settings;

        internal static bool RemovableCorpses
        {
            get { return settings.RemovableCorpses; }
        }

        internal static bool RemovableCampfires
        {
            get { return settings.RemovableCampfires; }
        }

        public static void OnLoad()
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            Log("Version " + assemblyName.Version);

            RepairManager.Initialize();

            settings = HomeImprovementSettings.Load();
            settings.AddToModSettings(NAME, ModSettings.MenuType.MainMenuOnly);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += PrepareScene;
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

        internal static void PrepareScene(Scene scene, LoadSceneMode mode)
        {
            if (GameManager.m_ActiveScene == null || "Empty" == scene.name || "MainMenu" == scene.name || "Boot" == scene.name)
            {
                return;
            }

            RepairManager.PrepareRepairables(scene);
            CleanupManager.PrepareCleanables(scene);
        }
    }
}