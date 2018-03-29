using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HomeImprovement
{
    internal class HomeImprovementUtils
    {
        internal static void AddAnimation(GameObject target)
        {
            ObjectAnim objectAnim = target.AddComponent<ObjectAnim>();
            objectAnim.m_Target = target;
        }

        internal static void CopyContainer(Container source, GameObject target)
        {
            Container container = target.AddComponent<Container>();
            container.m_LocalizedDisplayName = new LocalizedString()
            {
                m_LocalizationID = source.m_LocalizedDisplayName.m_LocalizationID
            };
            container.m_CloseAudio = source.m_CloseAudio;
            container.m_CapacityKG = source.m_CapacityKG;
            container.m_DecayScalar = source.m_DecayScalar;
            container.m_DefaultFilter = source.m_DefaultFilter;
            container.m_OpenAudio = source.m_OpenAudio;
            container.m_OpenDelaySeconds = source.m_OpenDelaySeconds;
            container.m_StartInspected = true;

            container.Start();
        }

        internal static void CopyTweenEvents(Component source, GameObject target)
        {
            foreach (iTweenEvent eachTemplate in source.GetComponentsInChildren<iTweenEvent>())
            {
                iTweenEvent iTweenEvent = target.AddComponent<iTweenEvent>();
                iTweenEvent.tweenName = eachTemplate.tweenName;
                iTweenEvent.type = eachTemplate.type;
                iTweenEvent.Values = eachTemplate.Values;
                iTweenEvent.playAutomatically = eachTemplate.playAutomatically;
            }
        }

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

            return parent.GetComponentInChildren<Container>();
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
            if (objectGuid != null)
            {
                ObjectGuidManager.UnRegisterGuid(objectGuid.m_Guid);
            }
            else
            {
                objectGuid = target.AddComponent<ObjectGuid>();
            }

            objectGuid.m_Guid = guid;
            ObjectGuidManager.RegisterGuid(guid, target);
        }
    }
}