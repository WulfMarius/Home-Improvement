using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HomeImprovement
{
    internal class HomeImprovementUtils
    {
        internal static T DeserializeSaveProxy<T>(string saveProxyData)
        {
            if (string.IsNullOrEmpty(saveProxyData))
            {
                return default(T);
            }

            SaveProxy saveProxy = Utils.DeserializeObject<SaveProxy>(saveProxyData);
            if (string.IsNullOrEmpty(saveProxy.data))
            {
                return default(T);
            }

            return Utils.DeserializeObject<T>(saveProxy.data);
        }

        internal static void FindChildren(GameObject gameObject, List<GameObject> result, GameObjectSearchFilter filter)
        {
            SearchResult searchResult = filter.Filter(gameObject);

            if (searchResult.IsInclude())
            {
                result.Add(gameObject);
            }

            if (searchResult.IsContinue())
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    GameObject child = gameObject.transform.GetChild(i).gameObject;
                    FindChildren(child, result, filter);
                }
            }
        }

        internal static Container FindContainerTemplate(GameObject gameObject)
        {
            Transform parentTransform = gameObject.transform.parent;
            if (parentTransform == null)
            {
                return null;
            }

            GameObject parent = parentTransform.gameObject;
            if (!parent.name.Contains("CONTAINER_"))
            {
                return null;
            }

            Container[] containers = parent.GetComponentsInChildren<Container>();
            if (containers == null)
            {
                return null;
            }

            foreach (Container eachContainer in containers)
            {
                if (eachContainer.enabled)
                {
                    return eachContainer;
                }
            }

            return null;
        }

        internal static GameObject GetParent(Component component)
        {
            if (component == null)
            {
                return null;
            }

            return GetParent(component.gameObject);
        }

        internal static GameObject GetParent(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return null;
            }

            Transform parent = gameObject.transform.parent;
            if (parent == null)
            {
                return null;
            }

            return parent.gameObject;
        }

        internal static string GetPath(GameObject gameObject)
        {
            StringBuilder stringBuilder = new StringBuilder();

            Transform current = gameObject.transform;

            while (current != null)
            {
                stringBuilder.Insert(0, current.name);
                stringBuilder.Insert(0, "/");

                current = current.transform.parent;
            }

            return stringBuilder.ToString();
        }

        internal static List<GameObject> GetSceneObjects(GameObjectSearchFilter filter)
        {
            List<GameObject> result = new List<GameObject>();

            Scene scene = SceneManager.GetActiveScene();

            if (scene != null)
            {
                foreach (GameObject eachRoot in scene.GetRootGameObjects())
                {
                    FindChildren(eachRoot, result, filter);
                }
            }

            return result;
        }

        internal static void SetGuid(GameObject target, string guid)
        {
            ObjectGuid objectGuid = target.GetComponent<ObjectGuid>();
            if (objectGuid == null)
            {
                objectGuid = target.AddComponent<ObjectGuid>();
            }

            objectGuid.m_Guid = guid;
            ObjectGuidManager.RegisterGuid(guid, target);
        }
    }
}