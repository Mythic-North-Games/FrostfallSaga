using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Core.Quests
{
    public class HeroTeamQuests : MonoBehaviourPersistingSingleton<HeroTeamQuests>
    {
        [field: SerializeField] public List<AQuestSO> Quests { get; private set; } = new();
        public List<AQuestSO> HistoryQuests => Quests.Where(quest => quest.Type == EQuestType.PRIMARY).ToList();
        public List<AQuestSO> SecondaryQuests => Quests.Where(quest => quest.Type == EQuestType.SECONDARY).ToList();
        public List<AQuestSO> MissionsQuests => Quests.Where(quest => quest.Type == EQuestType.MISSION).ToList();
        public List<AQuestSO> CompletedQuests => Quests.Where(quest => quest.IsCompleted).ToList();
        public List<AQuestSO> UncompletedQuests => Quests.Where(quest => !quest.IsCompleted).ToList();
        private AQuestSO CurrentTrackedQuest;

        public Action<AQuestSO> onTrackedQuestUpdated;

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

        /// <summary>
        /// Track a new quest and untrack the previous one if there is one.
        /// </summary>
        /// <param name="newTrackedQuest">The new quest to track.</param>
        /// <remarks>Do nothing if the quest is already tracked.</remarks>
        public void UpdateTrackedQuest(AQuestSO newTrackedQuest)
        {
            // Do nothing if the quest is already tracked
            if (newTrackedQuest.IsTracked) return;

            // Disable tracking on currently tracked quest if there is one
            if (CurrentTrackedQuest != null) CurrentTrackedQuest.IsTracked = false;

            // Enable tracking on the new tracked quest
            newTrackedQuest.IsTracked = true;
            CurrentTrackedQuest = newTrackedQuest;

            // Notify listeners that the tracked quest has changed
            onTrackedQuestUpdated?.Invoke(CurrentTrackedQuest);
        }

        /// <summary>
        /// Untrack a quest.
        /// </summary>
        /// <param name="quest">The quest to untrack.</param>
        /// <remarks>Do nothing if the quest is not tracked.</remarks>
        public void UntrackQuest(AQuestSO quest)
        {
            // Do nothing if the quest is not tracked
            if (!quest.IsTracked) return;

            // Disable tracking on the quest
            quest.IsTracked = false;
            CurrentTrackedQuest = null;

            // Notify listeners that the tracked quest has changed
            onTrackedQuestUpdated?.Invoke(null);
        }
    }
}