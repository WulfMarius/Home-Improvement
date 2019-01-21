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
                if (eachRepairedContainer.scene == GameManager.m_ActiveScene)
                {
                    RestoreRepairedContainer(eachRepairedContainer);
                }
            }

            stopwatch.Stop();
            Log("Loaded " + loadedRepairedContainers.containers.Count + " repair(s) for scene '" + GameManager.m_ActiveScene + "' in " + stopwatch.ElapsedMilliseconds + " ms");
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