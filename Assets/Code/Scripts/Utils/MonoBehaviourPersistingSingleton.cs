using System;
using UnityEngine;

namespace FrostfallSaga.Utils
{
    public class MonoBehaviourPersistingSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _instanceLock = new();
        private static bool _quitting;

        // Optional flags
        protected static bool AutoInitializeOnSceneLoad = false;
        protected static bool PersistAcrossScenes = true;

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
                            GameObject go = new(typeof(T).ToString());
                            _instance = go.AddComponent<T>();

                            if (PersistAcrossScenes) DontDestroyOnLoad(_instance.gameObject);
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
                throw new Exception($"Instance of {GetType().FullName} already exists, removing {ToString()}");
            }

            Init();
        }

        protected virtual void OnApplicationQuit()
        {
            _quitting = true;
        }


        // This method runs automatically when any scene is loaded
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeOnSceneLoad()
        {
            if (AutoInitializeOnSceneLoad)
            {
                T _ = Instance; // Trigger instance creation
            }
        }

        protected virtual void Init()
        {
        }
    }
}