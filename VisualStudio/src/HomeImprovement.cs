using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HomeImprovement
{
    public class HomeImprovement
    {
        private static string name;
        private static HomeImprovementSettings settings;

        internal static bool RemovableCorpses
        {
            get { return settings.RemovableCorpses; }
        }

        public static void OnLoad()
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            name = assemblyName.Name;

            Log("Version " + assemblyName.Version);

            RepairManager.Initialize();

            settings = HomeImprovementSettings.Load();
            settings.AddToModSettings("Home-Improvement", ModSettings.MenuType.MainMenuOnly);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += PrepareScene;
        }

        internal static void Log(string message)
        {
            Debug.Log("[" + name + "] " + message);
        }

        internal static void PrepareScene(Scene scene, LoadSceneMode mode)
        {
            if (GameManager.m_ActiveScene == null)
            {
                return;
            }

            RepairManager.PrepareRepairables(scene);
            CleanupManager.PrepareCleanables(scene);
        }
    }
}