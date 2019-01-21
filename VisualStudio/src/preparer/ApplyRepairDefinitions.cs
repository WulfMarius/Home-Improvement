using UnityEngine;
using UnityEngine.SceneManagement;

using static HomeImprovement.HomeImprovement;
using static HomeImprovement.HomeImprovementUtils;

namespace HomeImprovement.Preparer
{
    internal class ApplyRepairDefinitions : IScenePreparer
    {
        private RepairableContainerDefinitions definitions;

        internal ApplyRepairDefinitions()
        {
            LoadDefinitions();
        }

        public int PrepareScene(Scene scene)
        {
            int count = 0;

            RepairableContainerDefinition[] repairableContainerDefinition = definitions.GetDefinitions(scene.name);

            foreach (RepairableContainerDefinition eachDefinition in repairableContainerDefinition)
            {
                GameObject target = FindGameObject(eachDefinition.Target.Path, eachDefinition.Target.Position);
                if (target == null)
                {
                    Log("Could not find target of definition for " + eachDefinition.Target.Path + " @" + eachDefinition.Target.Position.ToString("F3"));
                    continue;
                }

                GameObject template = FindGameObject(eachDefinition.Template.Path, eachDefinition.Template.Position);
                if (template == null)
                {
                    Log("Could not find template of definition for " + eachDefinition.Target.Path + " @" + eachDefinition.Target.Position.ToString("F3"));
                    continue;
                }

                Container container = template.GetComponent<Container>();
                if (container == null)
                {
                    Log("Could not find container of definition for " + eachDefinition.Target.Path + " @" + eachDefinition.Target.Position.ToString("F3"));
                    continue;
                }

                if ("CabinetDoor" == eachDefinition.Type)
                {
                    if (RepairCabinetDoors.Prepare(target, container, eachDefinition.Reference))
                    {
                        count++;
                    }
                }
                else if ("Drawer" == eachDefinition.Type)
                {
                    if (RepairDrawers.Prepare(target, container, eachDefinition.Reference)) {
                        count++;
                    }
                }
                else
                {
                    Log("Unsupported type '" + eachDefinition.Type + "'");
                }
            }

            return count;
        }

        internal void LoadDefinitions()
        {
            System.IO.StreamReader streamReader = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HomeImprovement.repairable-container-definitions.json"));
            string json = streamReader.ReadToEnd();
            streamReader.Close();

            definitions = Utils.DeserializeObject<RepairableContainerDefinitions>(json);

            if (definitions == null)
            {
                definitions = new RepairableContainerDefinitions();
            }
        }
    }
}