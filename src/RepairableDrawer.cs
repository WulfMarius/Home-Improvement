using UnityEngine;

using static HomeImprovement.HomeImprovementUtils;

namespace HomeImprovement
{
    internal class RepairableDrawer : GameObjectSearchFilter
    {
        private static RepairableDrawer instance = new RepairableDrawer();

        private RepairableDrawer()
        {
        }

        public static RepairableDrawer FilterInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RepairableDrawer();
                }
                return instance;
            }
        }

        public override SearchResult Filter(GameObject gameObject)
        {
            if (gameObject.layer == vp_Layer.Gear)
            {
                return SearchResult.SKIP_CHILDREN;
            }

            if ((gameObject.GetComponent<Renderer>()?.isPartOfStaticBatch).GetValueOrDefault(false))
            {
                return SearchResult.CONTINUE;
            }

            if (gameObject.GetComponent<Container>() != null)
            {
                return SearchResult.SKIP_CHILDREN;
            }

            if (gameObject.name.StartsWith("OBJ_DresserDrawer") || gameObject.name.StartsWith("OBJ_DresserTallDrawer"))
            {
                return SearchResult.INCLUDE_SKIP_CHILDREN;
            }

            return SearchResult.CONTINUE;
        }

        internal static void Prepare(GameObject target, Container template)
        {
            Prepare(target, template, target.transform.localPosition);
        }

        internal static void Prepare(GameObject target, Container template, Vector3 referencePoint)
        {
            RepairableContainer repairable = target.AddComponent<RepairableContainer>();
            repairable.Template = template.gameObject;
            repairable.ParentContainer = GetParent(repairable.Template);
            repairable.TargetPosition = RepairableContainer.GetTargetPosition(repairable.ParentContainer, referencePoint);
            repairable.TargetRotation = repairable.Template.transform.localRotation;
            repairable.RequiresTools = false;

            target.AddComponent<BoxCollider>();
            target.layer = vp_Layer.Container;
        }
    }
}