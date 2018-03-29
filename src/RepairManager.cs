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

        internal static void LoadRepairs(string saveName, string sceneSaveName)
        {
            repairedContainers.Clear();

            string saveProxyData = SaveGameSlots.LoadDataFromSlot(saveName, sceneSaveName + REPAIRED_CONTAINERS_SUFFIX);
            RepairedContainers loadedRepairedContainers = DeserializeSaveProxy<RepairedContainers>(saveProxyData);

            if (loadedRepairedContainers == null)
            {
                Log("Loaded 0 repair(s) for scene '" + GameManager.m_ActiveScene + "'.");
                return;
            }

            Log("Loaded " + loadedRepairedContainers.containers.Count + " repair(s) for scene '" + GameManager.m_ActiveScene + "'.");

            foreach (RepairedContainer eachRepairedContainer in loadedRepairedContainers.containers)
            {
                if (eachRepairedContainer.scene != GameManager.m_ActiveScene)
                {
                    continue;
                }
                RepairContainer(eachRepairedContainer);
            }
        }

        internal static void PrepareRepairables()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            List<GameObject> repairableDrawers = HomeImprovementUtils.GetSceneObjects(RepairableDrawerFilter.Instance);

            foreach (GameObject eachRepairableDrawer in repairableDrawers)
            {
                Container template = HomeImprovementUtils.FindContainerTemplate(eachRepairableDrawer);
                if (template == null)
                {
                    continue;
                }

                RepairableDrawer.Prepare(eachRepairableDrawer, template);
            }

            stopwatch.Stop();
            Log("Prepared " + repairableDrawers.Count + " repairable(s) in scene '" + GameManager.m_ActiveScene + "' in " + stopwatch.ElapsedMilliseconds + " ms");
        }

        internal static void RepairContainer(GameObject target, Container template, string guid)
        {
            HomeImprovementUtils.SetGuid(target, guid);
            HomeImprovementUtils.CopyTweenEvents(template, target);
            HomeImprovementUtils.AddAnimation(target);
            HomeImprovementUtils.CopyContainer(template, target);

            vp_Layer.Set(target, vp_Layer.Container);

            RepairManager.AddRepairedContainer(guid, HomeImprovementUtils.GetPath(target), GameManager.m_ActiveScene);
        }

        internal static void SaveRepairs(SaveSlotType gameMode, string saveName, string sceneSaveName)
        {
            string saveProxyData = Utils.SerializeObject(new SaveProxy()
            {
                data = Utils.SerializeObject(repairedContainers)
            });

            SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, saveName, sceneSaveName + REPAIRED_CONTAINERS_SUFFIX, saveProxyData);
        }

        private static void AddRepairedContainer(string guid, string path, string scene)
        {
            RepairedContainer repairedContainer = new RepairedContainer(scene, path, guid);
            repairedContainers.AddRepairedContainer(repairedContainer);
        }

        private static void RepairContainer(RepairedContainer repairedContainer)
        {
            GameObject target = GameObject.Find(repairedContainer.path);
            if (target == null)
            {
                return;
            }

            RepairableContainer repairableContainer = target.GetComponentInChildren<RepairableContainer>();
            if (repairableContainer == null)
            {
                return;
            }

            repairableContainer.ContainerGuid = repairedContainer.guid;
            repairableContainer.Repair();
        }
    }
}