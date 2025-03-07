using System;
using System.Collections;
using FrostfallSaga.Core;
using FrostfallSaga.Core.Dungeons;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Kingdom.UI
{
    public class EnterDungeonPanelController : BaseUIController
    {
        private static readonly string PANEL_CONTAINER_UI_NAME = "ScreenPanelContainer";
        private static readonly string DUNGEON_PANEL_CONTAINER_UI_NAME = "DungeonPanelContainer";
        private static readonly string NAME_LABEL_UI_NAME = "DungeonNameLabel";
        private static readonly string PREVIEW_CONTAINER_UI_NAME = "DungeonPreview";
        private static readonly string DESCRIPTION_LABEL_UI_NAME = "DescriptionLabel";
        private static readonly string ENTER_DUNGEON_BUTTON_UI_NAME = "EnterButton";
        private static readonly string EXIT_DUNGEON_BUTTON_UI_NAME = "ExitButton";
        private static readonly string PANEL_HIDDEN_CLASSNAME = "panelContainerHidden";
        private static readonly string DUNGEON_PANEL_HIDDEN_CLASSNAME = "dungeonPanelContainerHidden";

        [field: SerializeField] public VisualTreeAsset EnterDungeonPanelTemplate { get; private set; }
        [SerializeField] private KingdomManager _kingdomManager;
        private DungeonBuildingConfigurationSO _currentDungeon;

        private TemplateContainer _dungeonGatePanel;

        public Action<DungeonBuildingConfigurationSO> onDungeonEnterClicked;

        private void Awake()
        {
            if (_kingdomManager == null)
            {
                Debug.LogError("KingdomManager is not set. Won't be able to display dungeon enter panel.");
                return;
            }

            _kingdomManager.OnInterestPointEncountered += OnDungeonBuildingEncountered;
        }

        private void OnDungeonBuildingEncountered(AInterestPointConfigurationSO interestPointConfiguration)
        {
            if (interestPointConfiguration is not DungeonBuildingConfigurationSO dungeonBuildingConfiguration) return;

            _currentDungeon = dungeonBuildingConfiguration;
            _dungeonGatePanel = EnterDungeonPanelTemplate.Instantiate();
            _dungeonGatePanel.Q<Button>(ENTER_DUNGEON_BUTTON_UI_NAME).clicked += OnEnterClicked;
            _dungeonGatePanel.Q<Button>(EXIT_DUNGEON_BUTTON_UI_NAME).clicked += OnExitClicked;
            SetupDungeonPanel(dungeonBuildingConfiguration);
            _uiDoc.rootVisualElement.Add(_dungeonGatePanel);

            StartCoroutine(DisplayPanel());
        }

        private IEnumerator DisplayPanel()
        {
            _dungeonGatePanel.Q<VisualElement>(PANEL_CONTAINER_UI_NAME).RemoveFromClassList(PANEL_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.1f);
            _dungeonGatePanel.Q<VisualElement>(DUNGEON_PANEL_CONTAINER_UI_NAME)
                .RemoveFromClassList(DUNGEON_PANEL_HIDDEN_CLASSNAME);
        }

        private IEnumerator HidePanel()
        {
            _dungeonGatePanel.Q<VisualElement>(DUNGEON_PANEL_CONTAINER_UI_NAME)
                .AddToClassList(DUNGEON_PANEL_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.1f);
            _dungeonGatePanel.Q<VisualElement>(PANEL_CONTAINER_UI_NAME).AddToClassList(PANEL_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.4f);
            _dungeonGatePanel.RemoveFromHierarchy();
        }

        private void SetupDungeonPanel(DungeonBuildingConfigurationSO dungeonBuildingConfiguration)
        {
            _dungeonGatePanel.style.flexGrow = 1;
            _dungeonGatePanel.Q<VisualElement>(PANEL_CONTAINER_UI_NAME).AddToClassList(PANEL_HIDDEN_CLASSNAME);
            _dungeonGatePanel.Q<VisualElement>(DUNGEON_PANEL_CONTAINER_UI_NAME)
                .AddToClassList(DUNGEON_PANEL_HIDDEN_CLASSNAME);
            _dungeonGatePanel.Q<VisualElement>(PREVIEW_CONTAINER_UI_NAME).style.backgroundImage =
                new StyleBackground(dungeonBuildingConfiguration.DungeonPreview);
            _dungeonGatePanel.Q<Label>(NAME_LABEL_UI_NAME).text = dungeonBuildingConfiguration.Name;
            _dungeonGatePanel.Q<Label>(DESCRIPTION_LABEL_UI_NAME).text = dungeonBuildingConfiguration.Description;
        }

        private void OnEnterClicked()
        {
            StartCoroutine(HidePanel());
            onDungeonEnterClicked?.Invoke(_currentDungeon);
        }

        private void OnExitClicked()
        {
            StartCoroutine(HidePanel());
            _currentDungeon = null;
        }
    }
}