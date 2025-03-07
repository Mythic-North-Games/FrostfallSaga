using System;
using UnityEngine;

namespace FrostfallSaga.Core.Quests
{
    [Serializable]
    public abstract class AQuestAction
    {
        [field: SerializeField] public bool IsCompleted { get; protected set; }
        
        public Action<AQuestAction> onActionCompleted;
        public MonoBehaviour SceneManager { get; protected set; }

        /// <summary>
        /// Get the instruction of the action.
        /// </summary>
        public abstract string GetInstruction();

        /// <summary>
        /// Start listening to the events that will update the action completion.
        /// </summary>
        /// <param name="sceneManager">The specific manager of the scene the action is related to.</param>
        public virtual void Initialize(MonoBehaviour sceneManager)
        {
            SceneManager = sceneManager;
        }

        /// <summary>
        /// Stop listening to the events that will update the action completion.
        /// </summary>
        protected abstract void UnbindSceneManagerEvents();

        protected void CompleteAction()
        {
            UnbindSceneManagerEvents();
            SceneManager = null;
            
            IsCompleted = true;
            onActionCompleted?.Invoke(this);
        }
    }
}