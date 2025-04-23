using System;
using FrostfallSaga.Core.Quests;
using UnityEngine;

namespace FrostfallSaga.Dungeon.QuestActions
{
    /// <summary>
    ///     If a dungeon is completed, the action is completed.
    /// </summary>
    [Serializable]
    public class WinFightQuestAction : AQuestAction
    {
        public override string GetInstruction() => "Complete a dungeon.";

        public override void Initialize(MonoBehaviour sceneManager)
        {
            base.Initialize(sceneManager);
            if (sceneManager.GetType() != typeof(DungeonManager)) return;

            DungeonManager dungeonManager = (DungeonManager)sceneManager;
            dungeonManager.onDungeonCompleted += OnDungeonCompleted;
        }

        private void OnDungeonCompleted()
        {
            CompleteAction();
        }

        protected override void UnbindSceneManagerEvents()
        {
            DungeonManager dungeonManager = (DungeonManager)SceneManager;
            dungeonManager.onDungeonCompleted -= OnDungeonCompleted;
        }
    }
}