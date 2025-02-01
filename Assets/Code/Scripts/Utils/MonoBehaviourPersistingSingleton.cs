using UnityEngine;

namespace FrostfallSaga.Utils
{
    public class MonoBehaviourPersistingSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _instanceLock = new object();
        private static bool _quitting = false;

        // Optional flags
        protected static bool AutoInitializeOnSceneLoad = false;
        protected static bool PersistAcrossScenes = true;


        // This method runs automatically when any scene is loaded
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeOnSceneLoad()
        {
            if (AutoInitializeOnSceneLoad)
            {
                var _ = Instance; // Trigger instance creation
            }
        }

        public static T Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null && !_quitting)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            GameObject go = new GameObject(typeof(T).ToString());
                            _instance = go.AddComponent<T>();

                            if (PersistAcrossScenes)
                            {
                                DontDestroyOnLoad(_instance.gameObject);
                            }
                        }
                    }
                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = GetComponent<T>();
            }
            else if (_instance.GetInstanceID() != GetInstanceID())
            {
                Destroy(gameObject);
                throw new System.Exception($"Instance of {GetType().FullName} already exists, removing {ToString()}");
            }
            Init();
        }

        protected virtual void OnApplicationQuit()
        {
            _quitting = true;
        }

        protected virtual void Init() { }
    }
}
