using System.Collections.Generic;
using UnityEngine;

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

        private static RepairedContainers repairedContainers = new RepairedContainers();
        private static RepairableContainerDefinitions definitions;

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

        internal static void AddRepairedContainer(string guid, GameObject repairableContainer, string scene)
        {
            RepairedContainer repairedContainer = new RepairedContainer(scene, GetPath(repairableContainer), repairableContainer.transform.position, guid);
            repairedContainers.AddRepairedContainer(repairedContainer);
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

            foreach (GameObject eachRepairableDrawer in GetSceneObjects(RepairableDrawer.FilterInstance))
            {
                Container template = FindContainerTemplate(eachRepairableDrawer);
                if (template != null)
                {
                    RepairableDrawer.Prepare(eachRepairableDrawer, template);
                    count++;
                }
            }

            foreach (GameObject eachRepairableCabinetDoor in GetSceneObjects(RepairableCabinetDoor.FilterInstance))
            {
                Container template = FindContainerTemplate(eachRepairableCabinetDoor);
                if (template != null)
                {
                    RepairableCabinetDoor.Prepare(eachRepairableCabinetDoor, template);
                    count++;
                }
            }

            RepairableContainerDefinition[] repairableContainerDefinition = definitions.GetDefinitions(GameManager.m_ActiveScene);
            foreach (RepairableContainerDefinition eachDefinition in repairableContainerDefinition)
            {
                GameObject target = FindGameObject(eachDefinition.Target.Path, eachDefinition.Target.Position);
                if (target == null)
                {
                    continue;
                }

                GameObject template = FindGameObject(eachDefinition.Template.Path, eachDefinition.Template.Position);
                if (template == null)
                {
                    continue;
                }

                Container container = template.GetComponent<Container>();
                if (container == null)
                {
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

            stopwatch.Stop();
            Log("Prepared " + count + " repairable(s) in scene '" + GameManager.m_ActiveScene + "' in " + stopwatch.ElapsedMilliseconds + " ms");
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
            List<GameObject> targets = GetSceneObjects(new PathGameObjectSearchFilter(path));
            return targets.Find(target => Vector3.Distance(target.transform.position, position) < 0.05f);
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