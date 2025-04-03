using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.UI.FightEndMenu;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight
{
    public class FightEndingManager : MonoBehaviour
    {
        [SerializeField] private FightManager _fightManager;
        [SerializeField, Header("End menu UI")] private UIDocument _fightEndMenuUIDocument;
        [SerializeField] private VisualTreeAsset _fighterStateContainerTemplate;
        [SerializeField] private VisualTreeAsset _itemRewardContainerTemplate;
        [SerializeField] private VisualTreeAsset _statContainerTemplate;
        [SerializeField] private Color _itemRewardStatsColor = new(0.8f, 0.8f, 0.8f, 1f);
        [SerializeField] private float _itemRewardLongHoverDuration = 0.5f;
        [SerializeField] private string _fightWonText = "Enemies defeated";
        [SerializeField] private string _fightLostText = "You have been defeated";

        private FightEndMenuController _fightEndMenuController;
        private GameStateManager _gameStateManager;

        private void HandleFightEnding(Fighter[] allies, Fighter[] enemies)
        {
            // First, generate and save post-fight data
            GenerateAndSavePostFightData(allies, enemies);

            // Check if allies have won
            bool alliesWon = AlliesHaveWon(allies);

            // Handle rewards
            int stycasReward = 0;
            List<ItemSO> itemsReward = new();
            if (alliesWon)
            {
                stycasReward = FightRewardGenerator.GenerateStycasReward(enemies);
                itemsReward = FightRewardGenerator.GenerateItemsReward(enemies);

                // Add loot to hero team inventory
                HeroTeam heroTeam = HeroTeam.Instance;
                heroTeam.AddStycas(stycasReward);
                heroTeam.DistributeItems(itemsReward.ToArray());
            }

            // Configure and display the fight end menu
            _fightEndMenuController.Setup(allies, enemies, alliesWon, stycasReward, itemsReward);
            _fightEndMenuController.SetVisible(true);
        }

        private void OnPlayerContinues()
        {
            // Transition to the next scene
            EScenesName sceneToTransitionTo = _gameStateManager.GetCurrentFightOrigin().ToEScenesName();
            _gameStateManager.CleanPreFightData();
            Debug.Log($"Transitioning to {sceneToTransitionTo}...");
            SceneTransitioner.FadeInToScene(sceneToTransitionTo.ToSceneString());
        }

        private bool AlliesHaveWon(Fighter[] allies)
        {
            foreach (Fighter ally in allies)
                if (ally.GetHealth() > 0)
                    return true;

            return false;
        }

        private void GenerateAndSavePostFightData(Fighter[] allies, Fighter[] enemies)
        {
            GameStateManager gameStateManager = GameStateManager.Instance;

            if (gameStateManager.GetCurrentFightOrigin() == EFightOrigin.DUNGEON)
            {
                Debug.Log("Saving dungeon progress...");

                bool alliesHaveWon = allies.All(ally => ally.GetHealth() > 0);
                gameStateManager.SaveDungeonProgress(alliesHaveWon);

                Debug.Log("Dungeon progress saved!");
            }

            if (enemies[0].EntitySessionId == null || enemies[0].EntitySessionId.Length == 0)
            {
                Debug.Log("No session ID on fighters. No post fight data saved.");
                gameStateManager.CleanPostFightData();
                return;
            }

            gameStateManager.SavePostFightData(enemies);
            Debug.Log("Post fight data saved!");
        }

        #region Setup

        private void Awake()
        {
            // Check if components are assigned
            if (_fightManager == null) _fightManager = FindObjectOfType<FightManager>();
            if (_fightManager == null)
            {
                Debug.LogError("FightManager is not assigned in the inspector.");
                return;
            }

            if (_fightEndMenuUIDocument == null)
            {
                Debug.LogError("FightEndMenu UIDocument is not assigned in the inspector.");
                return;
            }

            if (_fighterStateContainerTemplate == null)
            {
                Debug.LogError("FighterStateContainer template is not assigned in the inspector.");
                return;
            }

            if (_itemRewardContainerTemplate == null)
            {
                Debug.LogError("ItemRewardContainer template is not assigned in the inspector.");
                return;
            }

            _fightEndMenuController = new FightEndMenuController(
                _fightEndMenuUIDocument.rootVisualElement,
                _fighterStateContainerTemplate,
                _itemRewardContainerTemplate,
                _statContainerTemplate,
                _itemRewardStatsColor,
                _itemRewardLongHoverDuration,
                _fightWonText,
                _fightLostText
            );
            _fightEndMenuController.SetVisible(false);
            _fightEndMenuController.onContinueClicked += OnPlayerContinues;

            _gameStateManager = GameStateManager.Instance;

            _fightManager.onFightEnded += HandleFightEnding;
        }

        #endregion
    }
}