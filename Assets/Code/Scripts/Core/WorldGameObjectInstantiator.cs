using UnityEngine;

namespace FrostfallSaga.Core
{
    public class WorldGameObjectInstantiator : MonoBehaviour
    {
        private static string DEFAULT_WORLD_GO_PARENT_NAME = "WORLD";

        [field: SerializeField, Tooltip("The game object that will contain all the world game object.")]
        public GameObject DefaultWorldGOParent { get; private set; }

        private void Awake()
        {
            if (DefaultWorldGOParent == null)
            {
                Debug.Log("DefaultWorldGOParent is null. Trying to find it in the scene.");
                DefaultWorldGOParent = GameObject.Find(DEFAULT_WORLD_GO_PARENT_NAME);
            }

            if (DefaultWorldGOParent == null)
            {
                Debug.Log("DefaultWorldGOParent is still null. Creating a new one.");
                DefaultWorldGOParent = new GameObject(DEFAULT_WORLD_GO_PARENT_NAME);
            }
        }

        public GameObject Instantiate(GameObject prefab)
        {
            return Instantiate(prefab, DefaultWorldGOParent.transform);
        }
    }
}