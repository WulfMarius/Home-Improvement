using UnityEngine;

namespace HomeImprovement.Preparer
{
    internal class ConvertToyBlocks : AbstractScenePreparer
    {
        protected override bool Disabled => false;

        protected override SearchResult Accept(GameObject gameObject)
        {
            if (!gameObject.activeInHierarchy || gameObject.layer == vp_Layer.Gear)
            {
                return SearchResult.SKIP_CHILDREN;
            }

            if (gameObject.name.StartsWith("OBJ_ToyBlock"))
            {
                return SearchResult.INCLUDE_SKIP_CHILDREN;
            }

            return SearchResult.CONTINUE;
        }

        protected override bool Prepare(GameObject gameObject)
        {
            string name = gameObject.name;
            if (name.EndsWith("_Prefab"))
            {
                name = name.Substring(0, name.Length - "_Prefab".Length);
            }
            name = name.Replace("OBJ_", "GEAR_");

            GameObject prefab = Resources.Load(name) as GameObject;
            if (prefab == null)
            {
                return false;
            }

            GameObject instance = GameObject.Instantiate(prefab, gameObject.transform.position, gameObject.transform.rotation);
            instance.name = prefab.name;

            GearItem gearItem =  instance.GetComponentInChildren<GearItem>();
            gearItem?.StickToGroundAndOrientOnSlope(instance.transform.position, NavMeshCheck.IgnoreNavMesh);

            GameObject.Destroy(gameObject);

            return true;
        }
    }
}