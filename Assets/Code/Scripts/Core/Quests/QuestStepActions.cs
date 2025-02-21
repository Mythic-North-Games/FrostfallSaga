using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrostfallSaga.Core.Quests
{
    /// <summary>
    /// This class is used to define the actions that can be taken in a quest step.
    /// First, the player must complete all the NonDecisiveActions, 
    /// then he can do one of the DecisiveActions that lead to a different end.
    /// </summary>
    [CreateAssetMenu(fileName = "QuestStepActions", menuName = "ScriptableObjects/Quests/QuestStepActions", order = 0)]
    public class QuestStepActionsSO : ScriptableObject
    {
        [SerializeReference] public List<AQuestAction> NonDecisiveActions = new();

        [field: SerializeField, Tooltip("If the non decisive actions should be done in order or not.")]
        public bool OrderedNonDecisiveActions { get; private set; }

        [SerializeReference, Tooltip("Each action defined here leads to a different quest step.")]
        public List<AQuestAction> DecisiveActions = new();
        
        public Action onStepActionsCompleted;
        public int ChosenDecisiveActionIndex { get; private set; } = -1;

        private void Start()
        {
            if (DecisiveActions == null || DecisiveActions.Count == 0)
            {
                Debug.LogError("DecisiveActions is empty. Please add at least one decisive action.");
            }
        }

        /// <summary>
        /// Initiliaze only the active actions of the quest step.
        /// </summary>
        /// <param name="currentSceneManager">The current scene manager that the actions will use to determine completion.</param>
        public void Initialize(MonoBehaviour currentSceneManager)
        {
            if (ChosenDecisiveActionIndex > -1)
            {
                Debug.LogWarning("The quest step is already completed. No need to initialize the actions.");
                return;
            }

            // Init non decisive actions first if they are not completed.
            if (NonDecisiveActions.Any(action => !action.IsCompleted))
            {
                InitializeNonDecisiveActions(currentSceneManager);
            }
            else    // If all non decisive actions are completed, initialize decisive actions.
            {
                InitializeDecisiveActions(currentSceneManager);
            }
        }

        private void InitializeNonDecisiveActions(MonoBehaviour currentSceneManager)
        {
            foreach (AQuestAction action in NonDecisiveActions)
            {
                if (!action.IsCompleted)
                {
                    action.Initialize(currentSceneManager);
                    action.onActionCompleted += OnNonDecisiveActionCompleted;

                    if (OrderedNonDecisiveActions)
                    {
                        return;
                    }
                }
            }
        }

        private void InitializeDecisiveActions(MonoBehaviour currentSceneManager)
        {
            foreach (AQuestAction action in DecisiveActions)
            {
                if (!action.IsCompleted)
                {
                    action.Initialize(currentSceneManager);
                    action.onActionCompleted += OnDecisiveActionCompleted;
                }
            }
        }

        /// <summary>
        /// Initialize the next actions.
        /// </summary>
        /// <param name="completedAction">The just completed action.</param>
        private void OnNonDecisiveActionCompleted(AQuestAction completedAction)
        {
            // If all non decisive actions are completed, initialize decisive actions.
            if (NonDecisiveActions.All(action => action.IsCompleted))
            {
                InitializeDecisiveActions(completedAction.SceneManager);
                return;
            }

            // Initialize the next non decisive action if they are ordered. Otherwise, they already are initialized.
            if (OrderedNonDecisiveActions)
            {
                int nextActionIndex = NonDecisiveActions.IndexOf(completedAction) + 1;
                if (nextActionIndex < NonDecisiveActions.Count)
                {
                    NonDecisiveActions[nextActionIndex].Initialize(completedAction.SceneManager);
                }
            }
        }

        /// <summary>
        /// Set the chosen decisive action index to mark the step as completed.
        /// </summary>
        /// <param name="completedAction">The just completed action.</param>
        private void OnDecisiveActionCompleted(AQuestAction completedAction)
        {
            ChosenDecisiveActionIndex = DecisiveActions.IndexOf(completedAction);
            onStepActionsCompleted?.Invoke();
        }
    }
}