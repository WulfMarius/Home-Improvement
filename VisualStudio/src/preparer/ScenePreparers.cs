using System.Collections.Generic;

using UnityEngine.SceneManagement;
using static HomeImprovement.HomeImprovement;

namespace HomeImprovement.Preparer
{
    internal class ScenePreparers
    {
        private static List<IScenePreparer> preparers = new List<IScenePreparer>();

        static ScenePreparers()
        {
            preparers.Add(new BreakDownCorpses());
            preparers.Add(new BreakDownPapers());
            preparers.Add(new ConvertToyBlocks());
            preparers.Add(new RepairCabinetDoors());
            preparers.Add(new RepairDrawers());
            preparers.Add(new ApplyRepairDefinitions());
        }

        public static void PrepareScene()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            int count = 0;

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                foreach (IScenePreparer eachScenePreparer in preparers)
                {
                    count += eachScenePreparer.PrepareScene(scene);
                }
            }

            stopwatch.Stop();
            Log("Prepared {0} element(s) for scene '{1}' in {2} ms", count, GameManager.m_ActiveScene, stopwatch.ElapsedMilliseconds);
        }
    }
}