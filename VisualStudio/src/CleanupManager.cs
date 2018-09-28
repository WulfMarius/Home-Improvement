using UnityEngine;
using UnityEngine.SceneManagement;

using static HomeImprovement.HomeImprovement;
using static HomeImprovement.HomeImprovementUtils;

namespace HomeImprovement
{
    public class CleanupManager
    {
        internal static void PrepareCleanables(Scene scene)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            int count = 0;

            count += PreparePaper(scene);
            count += PrepareCorpses(scene);

            stopwatch.Stop();
            Log("Prepared " + count + " cleanables(s) in scene '" + scene.name + "' in " + stopwatch.ElapsedMilliseconds + " ms");
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
    }
}