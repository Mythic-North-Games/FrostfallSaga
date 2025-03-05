using System;
using UnityEngine;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.QuestActions
{
    /// <summary>
    /// If the allies won the fight, the action is completed.
    /// </summary>
    [Serializable]
    public class WinFightQuestAction : AQuestAction
    {
        public override string GetInstruction() => "Win the fight";

        public override void Initialize(MonoBehaviour sceneManager)
        {
            base.Initialize(sceneManager);
            if (sceneManager.GetType() != typeof(FightManager))
            {
                return;
            }

            FightManager fightManager = (FightManager)sceneManager;
            fightManager.onFightEnded += OnFightEnded;
        }

        private void OnFightEnded(Fighter[] allies, Fighter[] ennemies)
        {
            if (AlliesHaveWon(allies))
            {
                CompleteAction();
            }
        }

        private bool AlliesHaveWon(Fighter[] allies)
        {
            foreach (Fighter ally in allies)
            {
                if (ally.GetHealth() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void UnbindSceneManagerEvents()
        {
            FightManager fightManager = (FightManager)SceneManager;
            fightManager.onFightEnded -= OnFightEnded;
        }
    }
}