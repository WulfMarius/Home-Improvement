using UnityEngine;

using static HomeImprovement.HomeImprovementUtils;

namespace HomeImprovement
{
    internal class RepairableCabinetDoor : GameObjectSearchFilter
    {
        private static RepairableCabinetDoor instance = new RepairableCabinetDoor();

        private RepairableCabinetDoor()
        {
        }

        public static RepairableCabinetDoor FilterInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RepairableCabinetDoor();
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

            if (gameObject.name.StartsWith("OBJ_KitchenCabinetDoor"))
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
            if (target.GetComponent<RepairableContainer>() != null)
            {
                return;
            }

            RepairableContainer repairable = target.AddComponent<RepairableContainer>();
            repairable.Template = GetParent(template);
            repairable.ParentContainer = GetParent(repairable.Template);
            repairable.TargetPosition = RepairableContainer.GetTargetPosition(repairable.ParentContainer, referencePoint);
            repairable.TargetRotation = repairable.Template.transform.localRotation;
            repairable.RequiresTools = true;

            if (target.GetComponent<Collider>() == null)
            {
                target.AddComponent<BoxCollider>();
            }

            target.layer = vp_Layer.Container;
        }
    }
}