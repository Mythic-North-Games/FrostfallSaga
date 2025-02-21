using System;
using UnityEngine;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Kingdom.EntitiesGroups;

namespace FrostfallSaga.Kingdom.QuestActions
{
    /// <summary>
    /// If the player encounters an enemies group, the action is completed.
    /// </summary>
    [Serializable]
    public class EncounterEnemiesGroupQuestAction : AQuestAction
    {
        public override void Initialize(MonoBehaviour sceneManager)
        {
            base.Initialize(sceneManager);
            if (sceneManager.GetType() != typeof(KingdomManager))
            {
                return;
            }

            KingdomManager kingdomManager = (KingdomManager)sceneManager;
            kingdomManager.onEnemiesGroupEncountered += OnEnemiesGroupEncountered;
        }

        private void OnEnemiesGroupEncountered(EntitiesGroup encounteredEnemiesGroup, bool heroGroupInitiating)
        {
            CompleteAction();
        }

        protected override void UnbindSceneManagerEvents()
        {
            KingdomManager kingdomManager = (KingdomManager)SceneManager;
            kingdomManager.onEnemiesGroupEncountered -= OnEnemiesGroupEncountered;
        }
    }
}