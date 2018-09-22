using Harmony;
using System;
using System.Reflection;
using UnityEngine;

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
        }

        internal static void Log(string message)
        {
            Debug.Log("[" + name + "] " + message);
        }
    }
}