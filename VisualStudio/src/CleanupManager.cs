using UnityEngine;

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

            count += PreparePaper();
            count += PrepareCorpses();

            stopwatch.Stop();
            Log("Prepared " + count + " cleanables(s) in scene '" + GameManager.m_ActiveScene + "' in " + stopwatch.ElapsedMilliseconds + " ms");
        }

        private static int PreparePaper()
        {
            int count = 0;

            foreach (GameObject eachPaperClutter in GetSceneObjects(PaperClutter.FilterInstance))
            {
                PaperClutter.Prepare(eachPaperClutter);
                count++;
            }

            return count;
        }

        private static int PrepareCorpses()
        {
            if (!HomeImprovement.RemovableCorpses)
            {
                return 0;
            }

            int count = 0;

            foreach (GameObject eachCorpseClutter in GetSceneObjects(CorpseClutter.FilterInstance))
            {
                Debug.Log("Preparing Corpse " + eachCorpseClutter);
                CorpseClutter.Prepare(eachCorpseClutter);
                count++;
            }

            return count;
        }
    }
}