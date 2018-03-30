using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static HomeImprovement.HomeImprovementUtils;

namespace HomeImprovement
{
    public class RepairableContainer : Repairable
    {
        public string ContainerGuid;

        public GameObject ParentContainer;
        public Vector3 TargetPosition;
        public Quaternion TargetRotation;
        public GameObject Template;

        public override bool ProcessInteraction()
        {
            if (GameManager.GetWeatherComponent().IsTooDarkForAction(ActionsToBlock.Repair))
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_RequiresLightToRepair"));
                GameAudioManager.PlayGUIError();
                return true;
            }

            if (RequiresTools && !GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_SimpleTools") && !GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_HighQualityTools"))
            {
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_ToolRequiredToForceOpen").Replace("{item-name}", Localization.Get("GAMEPLAY_RadialTools")));
                GameAudioManager.PlayGUIError();
                return true;
            }

            this.StartProgressBar("GAMEPLAY_RepairingProgress", "PLAY_REPAIRINGWOOD", 5);

            return true;
        }

        internal static Vector3 GetTargetPosition(GameObject containerParent, Vector3 relativePosition)
        {
            List<Vector3> positions = GetAllPositions(containerParent);
            if (positions == null || positions.Count == 0)
            {
                return Vector3.zero;
            }

            for (int i = 0; i < containerParent.transform.childCount; i++)
            {
                Transform child = containerParent.transform.GetChild(i);
                positions.RemoveAll(value => Vector3.Distance(value, child.transform.localPosition) < 0.05f);
            }

            return positions.OrderBy(value => Vector3.Distance(relativePosition, value)).FirstOrDefault();
        }

        internal override bool PerformRepair()
        {
            if (string.IsNullOrEmpty(ContainerGuid))
            {
                ContainerGuid = System.Guid.NewGuid().ToString();
            }

            GameObject clonedTemplate = Instantiate(Template, ParentContainer.transform);
            clonedTemplate.transform.localPosition = TargetPosition;
            clonedTemplate.transform.localRotation = TargetRotation;

            Container container = clonedTemplate.GetComponentInChildren<Container>();
            SetGuid(container.gameObject, ContainerGuid);
            SetEmpty(container);

            RepairManager.AddRepairedContainer(ContainerGuid, this.gameObject, GameManager.m_ActiveScene);
            Destroy(this.gameObject);

            return true;
        }

        private static List<Vector3> GetAllPositions(GameObject containerParent)
        {
            if (containerParent.name.StartsWith("CONTAINER_KitchenCabinetA") && containerParent.name.EndsWith("Sink"))
            {
                return new List<Vector3>
                {
                    new Vector3(-1.084f, 0.613f, -0.503f),
                    new Vector3(-0.473f, 0.613f, -0.503f),
                    new Vector3(0.138f, 0.613f, -0.502f),
                    new Vector3(0.748f, 0.613f, -0.498f),
                    new Vector3(0.800f, 0.612f, 0.060f),
                    new Vector3(0.798f, 0.613f, 0.672f),
                    new Vector3(0.798f, 0.613f, 1.283f)
                };
            }

            if (containerParent.name.StartsWith("CONTAINER_KitchenCabinetB") && containerParent.name.EndsWith("Sink"))
            {
                return new List<Vector3>
                {
                    new Vector3(-0.419f, 0.613f, 1.169f),
                    new Vector3(-0.419f, 0.613f, 0.556f),
                    new Vector3(-0.420f, 0.613f, -0.055f),
                    new Vector3(-0.420f, 0.613f, -0.662f)
                };
            }

            if (containerParent.name.StartsWith("CONTAINER_KitchenCabinetB"))
            {
                return new List<Vector3>
                {
                    new Vector3(-0.901f, 0.613f, 1.169f),
                    new Vector3(-0.901f, 0.613f, 0.556f),
                    new Vector3(-0.901f, 0.613f, -0.055f),
                    new Vector3(-0.901f, 0.613f, -0.666f)
                };
            }

            if (containerParent.name.StartsWith("CONTAINER_KitchenCabinetD"))
            {
                return new List<Vector3>
                {
                    new Vector3(-0.409f, -0.025f, 0.862f),
                    new Vector3(-0.409f, -0.025f, 0.253f),
                    new Vector3(-0.409f, -0.026f, -0.361f)
                };
            }

            if (containerParent.name.StartsWith("CONTAINER_LargeDresserA"))
            {
                return new List<Vector3>
                {
                    new Vector3(0.107f, 0.973f, -0.508f),
                    new Vector3(0.107f, 0.639f, -0.508f),
                    new Vector3(0.107f, 0.295f, -0.508f),
                    new Vector3(0.107f, 0.973f, 0.510f),
                    new Vector3(0.107f, 0.639f, 0.510f),
                    new Vector3(0.107f, 0.295f, 0.510f)
                };
            }

            if (containerParent.name.StartsWith("CONTAINER_LargeDresserB"))
            {
                return new List<Vector3>
                {
                    new Vector3(0.107f, 0.973f, -0.508f),
                    new Vector3(0.107f, 0.643f, -0.508f),
                    new Vector3(0.107f, 0.313f, -0.508f),
                    new Vector3(0.107f, 0.973f, 0.510f),
                    new Vector3(0.107f, 0.643f, 0.510f),
                    new Vector3(0.107f, 0.313f, 0.510f)
                };
            }

            if (containerParent.name.StartsWith("CONTAINER_DresserTallA"))
            {
                return new List<Vector3>
                {
                    new Vector3(0.135f, 1.547f, 0.000f),
                    new Vector3(0.135f, 1.232f, 0.000f),
                    new Vector3(0.135f, 0.917f, 0.000f),
                    new Vector3(0.135f, 0.602f, 0.000f),
                    new Vector3(0.135f, 0.287f, 0.000f)
                };
            }

            if (containerParent.name.StartsWith("CONTAINER_DresserTallB"))
            {
                return new List<Vector3>
                {
                    new Vector3(0.109f, 1.547f, 0.000f),
                    new Vector3(0.109f, 1.232f, 0.000f),
                    new Vector3(0.109f, 0.917f, 0.000f),
                    new Vector3(0.109f, 0.602f, 0.000f),
                    new Vector3(0.109f, 0.287f, 0.000f)
                };
            }

            return null;
        }

        private static void SetEmpty(Container container)
        {
            container.Start();
            container.DestroyAllGear();
            container.MarkAsInspected();
        }
    }
}