using UnityEngine;
using System.Reflection;

namespace HomeImprovement
{
    public class HomeImprovement
    {
        private static string name;

        public static void OnLoad()
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            name = assemblyName.Name;

            Log("Version " + assemblyName.Version);
        }

        internal static void Log(string message)
        {
            Debug.Log("[" + name + "] " + message);
        }
    }
}