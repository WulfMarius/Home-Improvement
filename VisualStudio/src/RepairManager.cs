using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using static HomeImprovement.HomeImprovement;
using static HomeImprovement.HomeImprovementUtils;

namespace HomeImprovement
{
    public struct SaveProxy
    {
        public string data;
    }

    public class RepairManager
    {
        private const string REPAIRED_CONTAINERS_SUFFIX = "-RepairedContainers";

        private static RepairableContainerDefinitions definitions;
        private static RepairedContainers repairedContainers = new RepairedContainers();

        internal static void AddRepairedContainer(string guid, GameObject repairableContainer, string scene)
        {
            RepairedContainer repairedContainer = new RepairedContainer(scene, GetPath(repairableContainer), repairableContainer.transform.position, guid);
            repairedContainers.AddRepairedContainer(repairedContainer);
        }

        internal static void Initialize()
        {
            System.IO.StreamReader streamReader = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HomeImprovement.repairable-container-definitions.json"));
            string json = streamReader.ReadToEnd();
            streamReader.Close();

            definitions = Utils.DeserializeObject<RepairableContainerDefinitions>(json);

            if (definitions == null)
            {
                definitions = new RepairableContainerDefinitions();
            }
        }

        internal static void LoadRepairs(string saveName, string sceneSaveName)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            repairedContainers.Clear();

            string saveProxyData = SaveGameSlots.LoadDataFromSlot(saveName, sceneSaveName + REPAIRED_CONTAINERS_SUFFIX);
            RepairedContainers loadedRepairedContainers = DeserializeSaveProxy<RepairedContainers>(saveProxyData);
            if (loadedRepairedContainers == null)
            {
                loadedRepairedContainers = new RepairedContainers();
            }

            foreach (RepairedContainer eachRepairedContainer in loadedRepairedContainers.containers)
            {
                if (eachRepairedContainer.scene == GameManager.m_ActiveScene)
                {
                    RestoreRepairedContainer(eachRepairedContainer);
                }
            }

            stopwatch.Stop();
            Log("Loaded " + loadedRepairedContainers.containers.Count + " repair(s) for scene '" + GameManager.m_ActiveScene + "' in " + stopwatch.ElapsedMilliseconds + " ms");
        }

        internal static void PrepareRepairables()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            int count = 0;

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                count += PrepareDrawers(scene);
                count += PrepareDoors(scene);
            }

            count += PrepareDefinitions(GameManager.m_ActiveScene);

            stopwatch.Stop();
            Log("Prepared {0} repairable(s) for scene '{1}' in {2} ms", count, GameManager.m_ActiveScene, stopwatch.ElapsedMilliseconds);
        }

        internal static void SaveRepairs(SaveSlotType gameMode, string saveName, string sceneSaveName)
        {
            string saveProxyData = Utils.SerializeObject(new SaveProxy()
            {
                data = Utils.SerializeObject(repairedContainers)
            });

            SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, saveName, sceneSaveName + REPAIRED_CONTAINERS_SUFFIX, saveProxyData);
        }

        private static GameObject FindGameObject(string path, Vector3 position)
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                List<GameObject> targets = GetSceneObjects(scene, new PathGameObjectSearchFilter(path, position));
                if (targets.Count > 0)
                {
                    return targets[0];
                }
            }

            return null;
        }

        private static int PrepareDefinitions(string sceneName)
        {
            int count = 0;

            RepairableContainerDefinition[] repairableContainerDefinition = definitions.GetDefinitions(sceneName);
            Log("Found " + repairableContainerDefinition.Length + " definitions for scene " + sceneName);

            foreach (RepairableContainerDefinition eachDefinition in repairableContainerDefinition)
            {
                GameObject target = FindGameObject(eachDefinition.Target.Path, eachDefinition.Target.Position);
                if (target == null)
                {
                    Log("Could not find target of definition for " + eachDefinition.Target.Path + " @" + eachDefinition.Target.Position.ToString("F3"));
                    continue;
                }

                GameObject template = FindGameObject(eachDefinition.Template.Path, eachDefinition.Template.Position);
                if (template == null)
                {
                    Log("Could not find template of definition for " + eachDefinition.Target.Path + " @" + eachDefinition.Target.Position.ToString("F3"));
                    continue;
                }

                Container container = template.GetComponent<Container>();
                if (container == null)
                {
                    Log("Could not find container of definition for " + eachDefinition.Target.Path + " @" + eachDefinition.Target.Position.ToString("F3"));
                    continue;
                }

                if ("CabinetDoor" == eachDefinition.Type)
                {
                    RepairableCabinetDoor.Prepare(target, container, eachDefinition.Reference);
                    count++;
                }
                else if ("Drawer" == eachDefinition.Type)
                {
                    RepairableDrawer.Prepare(target, container, eachDefinition.Reference);
                    count++;
                }
                else
                {
                    Log("Unsupported type '" + eachDefinition.Type + "'");
                }
            }

            return count;
        }

        private static int PrepareDoors(Scene scene)
        {
            int count = 0;

            foreach (GameObject eachRepairableCabinetDoor in GetSceneObjects(scene, RepairableCabinetDoor.FilterInstance))
            {
                Container template = FindContainerTemplate(eachRepairableCabinetDoor);
                if (template != null)
                {
                    RepairableCabinetDoor.Prepare(eachRepairableCabinetDoor, template);
                    count++;
                }
            }

            return count;
        }

        private static int PrepareDrawers(Scene scene)
        {
            int count = 0;

            foreach (GameObject eachRepairableDrawer in GetSceneObjects(scene, RepairableDrawer.FilterInstance))
            {
                Container template = FindContainerTemplate(eachRepairableDrawer);
                if (template != null)
                {
                    RepairableDrawer.Prepare(eachRepairableDrawer, template);
                    count++;
                }
            }

            return count;
        }

        private static void RestoreRepairedContainer(RepairedContainer repairedContainer)
        {
            GameObject target = FindGameObject(repairedContainer.path, repairedContainer.position);
            if (target == null)
            {
                return;
            }

            RepairableContainer repairableContainer = target.GetComponentInChildren<RepairableContainer>();
            if (repairableContainer != null && !repairableContainer.Applied)
            {
                repairableContainer.ContainerGuid = repairedContainer.guid;
                repairableContainer.Repair();
            }
        }
    }
}