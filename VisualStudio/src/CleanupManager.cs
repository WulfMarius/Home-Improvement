using UnityEngine;
using UnityEngine.SceneManagement;

using static HomeImprovement.HomeImprovement;
using static HomeImprovement.HomeImprovementUtils;

namespace HomeImprovement
{
    public class CleanupManager
    {
        internal static void PrepareCleanables()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            int count = 0;

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                count += PreparePaper(scene);
                count += PrepareCorpses(scene);
            }

            stopwatch.Stop();
            Log("Prepared {0} cleanable(s) for scene '{1}' in {2} ms", count, GameManager.m_ActiveScene, stopwatch.ElapsedMilliseconds);
        }

        private static int PrepareCorpses(Scene scene)
        {
            if (!HomeImprovement.RemovableCorpses)
            {
                return 0;
            }

            int count = 0;

            foreach (GameObject eachCorpseClutter in GetSceneObjects(scene, CorpseClutter.FilterInstance))
            {
                CorpseClutter.Prepare(eachCorpseClutter);
                count++;
            }

            return count;
        }

        private static int PreparePaper(Scene scene)
        {
            int count = 0;

            foreach (GameObject eachPaperClutter in GetSceneObjects(scene, PaperClutter.FilterInstance))
            {
                PaperClutter.Prepare(eachPaperClutter);
                count++;
            }

            return count;
        }
    }
}