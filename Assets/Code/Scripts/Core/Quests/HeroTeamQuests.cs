using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Core.Quests
{
    public class HeroTeamQuests : MonoBehaviourPersistingSingleton<HeroTeamQuests>
    {
        [field: SerializeField] public List<AQuestSO> Quests { get; private set; } = new();
        public List<AQuestSO> HistoryQuests => Quests.Where(quest => quest.Type == EQuestType.MAIN).ToList();
        public List<AQuestSO> SecondaryQuests => Quests.Where(quest => quest.Type == EQuestType.SECONDARY).ToList();
        public List<AQuestSO> MissionsQuests => Quests.Where(quest => quest.Type == EQuestType.MISSION).ToList();
        public List<AQuestSO> CompletedQuests => Quests.Where(quest => quest.IsCompleted).ToList();
        public List<AQuestSO> UncompletedQuests => Quests.Where(quest => !quest.IsCompleted).ToList();

        /// <summary>
        /// Add a new quest to the list of quests of the player.
        /// </summary>
        /// <param name="quest">The new quest to add.</param>
        public void AddQuest(AQuestSO quest)
        {
            Quests.Add(quest);
        }

        /// <summary>
        /// Make all the uncompleted quests actions listen to the events of the current scene
        //  to possibly resolve the action and so the quests.
        /// </summary>
        /// <param name="sceneManager">The current scene manager (ex: KingdomManager, FightManager...)</param>
        public void InitializeQuests(MonoBehaviour sceneManager)
        {
            UncompletedQuests.ForEach(quest => quest.Initialize(sceneManager));
        }
    }
}