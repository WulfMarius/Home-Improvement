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

            foreach (GameObject eachPaperClutter in GetSceneObjects(PaperClutter.FilterInstance))
            {
                PaperClutter.Prepare(eachPaperClutter);
                count++;
            }

            stopwatch.Stop();
            Log("Prepared " + count + " cleanables(s) in scene '" + GameManager.m_ActiveScene + "' in " + stopwatch.ElapsedMilliseconds + " ms");
        }
    }
}