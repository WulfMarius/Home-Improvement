using UnityEngine;

namespace HomeImprovement
{
    public class PathGameObjectSearchFilter : GameObjectSearchFilter
    {
        private string targetPath;

        public PathGameObjectSearchFilter(string targetPath)
        {
            this.targetPath = targetPath;
        }

        public override SearchResult Filter(GameObject gameObject)
        {
            string path = HomeImprovementUtils.GetPath(gameObject);

            if (!targetPath.StartsWith(path))
            {
                return SearchResult.SKIP_CHILDREN;
            }

            if (targetPath.Equals(path))
            {
                return SearchResult.INCLUDE_SKIP_CHILDREN;
            }

            return SearchResult.CONTINUE;
        }
    }
}