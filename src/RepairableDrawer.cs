using UnityEngine;

namespace HomeImprovement
{
    public class RepairableDrawer : RepairableContainer
    {
        public Container Template;

        public override bool ProcessInteraction()
        {
            if (Template == null)
            {
                return false;
            }

            this.StartProgressBar("GAMEPLAY_RepairingProgress", "PLAY_REPAIRINGWOOD", 5);

            return true;
        }

        internal static void Prepare(GameObject target, Container template)
        {
            RepairableDrawer fixDrawer = target.AddComponent<RepairableDrawer>();
            fixDrawer.Template = template;
            fixDrawer.InteractiveDisplayText = new LocalizedString()
            {
                m_LocalizationID = "GAMEPLAY_Repair"
            };

            target.AddComponent<BoxCollider>();
            target.layer = vp_Layer.Container;
        }

        internal override bool PerformRepair()
        {
            if (string.IsNullOrEmpty(ContainerGuid))
            {
                ContainerGuid = System.Guid.NewGuid().ToString();
            }

            Vector3 location = this.transform.localPosition;
            location.x = Template.transform.localPosition.x;
            this.transform.localPosition = location;
            this.transform.localRotation = Quaternion.identity;

            RepairManager.RepairContainer(gameObject, Template, ContainerGuid);
            return true;
        }
    }

    internal class RepairableDrawerFilter : GameObjectSearchFilter
    {
        private static RepairableDrawerFilter instance = new RepairableDrawerFilter();

        private RepairableDrawerFilter()
        {
        }

        public static RepairableDrawerFilter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RepairableDrawerFilter();
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
    }
}