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
                if (eachRepairedContainer.scene != GameManager.m_ActiveScene)
                {
                    continue;
                }
                RestoreRepairedContainer(eachRepairedContainer);
            }

            stopwatch.Stop();
            Log("Loaded and applied " + loadedRepairedContainers.containers.Count + " repairable(s) for scene '" + GameManager.m_ActiveScene + "' in " + stopwatch.ElapsedMilliseconds + " ms");
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

        private static void RestoreRepairedContainer(RepairedContainer repairedContainer)
        {
            List<GameObject> targets = GetSceneObjects(new PathGameObjectSearchFilter(repairedContainer.path));
            foreach (GameObject eachTarget in targets)
            {
                float distance = Vector3.Distance(eachTarget.transform.position, repairedContainer.position);
                if (distance > 0.05f)
                {
                    continue;
                }

                RepairableContainer repairableContainer = eachTarget.GetComponentInChildren<RepairableContainer>();
                if (repairableContainer == null)
                {
                    continue;
                }

                repairableContainer.ContainerGuid = repairedContainer.guid;
                repairableContainer.Repair();
                break;
            }
        }
    }
}