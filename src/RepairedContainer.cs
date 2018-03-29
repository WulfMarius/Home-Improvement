using System.Collections.Generic;

namespace HomeImprovement
{
    public struct RepairedContainer
    {
        public string guid;
        public string path;
        public string scene;

        public RepairedContainer(string scene, string path, string guid)
        {
            this.scene = scene;
            this.path = path;
            this.guid = guid;
        }
    }

    public class RepairedContainers
    {
        public List<RepairedContainer> containers = new List<RepairedContainer>();

        public void AddRepairedContainer(RepairedContainer repairedContainer)
        {
            this.containers.RemoveAll(container => container.scene == repairedContainer.scene && container.path == repairedContainer.path);
            this.containers.Add(repairedContainer);
        }

        public void Clear()
        {
            this.containers.Clear();
        }
    }
}