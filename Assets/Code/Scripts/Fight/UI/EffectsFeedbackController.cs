using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Camera;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class EffectsFeedbackController : BaseUIController
    {
        private static readonly string CONTAINER_UI_NAME = "EffectFeedbackContainer";
        private static readonly string EFFECT_LABEL_UI_NAME = "EffectFeedbackLabel";

        [SerializeField] [Header("UI options")]
        private VisualTreeAsset _effectsUIPanel;

        [SerializeField] private float _displayDuration = 2f;
        [SerializeField] private Vector2Int _displayMarginFromFighter = new(1, 1);
        [SerializeField] private string _damageStyleClass;
        [SerializeField] private string _healStyleClass;
        [SerializeField] private string _dodgeStyleClass;
        [SerializeField] private string _masterstrokeStyleClass;
        [SerializeField] private SElementToValue<EFighterMutableStat, Texture2D>[] _statIcons;

        [SerializeField] [Header("Needed components")]
        private FightLoader _fightLoader;

        [SerializeField] private CameraController _fightCameraController;

        private readonly Dictionary<Fighter, List<TemplateContainer>> _fighterEffectsPanel = new();
        private readonly Dictionary<TemplateContainer, GameObject> _panelsAnchors = new();
        private readonly Dictionary<TemplateContainer, WorldUIPositioner> _positioners = new();

        #region Setup & teardown

        private void Awake()
        {
            _fightLoader.onFightLoaded += OnFightLoaded;
        }

        #endregion

        private void OnFightLoaded(Fighter[] allies, Fighter[] enemies)
        {
            foreach (Fighter fighter in allies.Concat(enemies))
            {
                _fighterEffectsPanel.Add(fighter, new List<TemplateContainer>());
                fighter.onDamageReceived += OnFighterReceivedDamages;
                fighter.onHealReceived += OnFighterReceivedHeal;
                fighter.onActionDodged += OnFighterDodged;
            }
        }

        private void OnFighterReceivedDamages(Fighter receiver, int damageAmount, bool isMasterstroke)
        {
            TemplateContainer effectsPanel = SpawnEffectPanelForFighter(receiver);
            if (isMasterstroke)
            {
                effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = $"Critical! {damageAmount}";
                effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_masterstrokeStyleClass);
            }
            else
            {
                effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = $"{damageAmount}";
            }

            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_damageStyleClass);

            _fighterEffectsPanel[receiver].ForEach(panel => StartCoroutine(DisplayWaitAndRemove(receiver, panel)));
        }


        private void OnFighterReceivedHeal(Fighter receiver, int healAmount, bool isMasterstroke)
        {
            TemplateContainer effectsPanel = SpawnEffectPanelForFighter(receiver);
            if (isMasterstroke)
            {
                effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = $"Critical! +{healAmount}";
                effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_masterstrokeStyleClass);
            }
            else
            {
                effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = $"+{healAmount}";
            }

            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_healStyleClass);

            _fighterEffectsPanel[receiver].ForEach(panel => StartCoroutine(DisplayWaitAndRemove(receiver, panel)));
        }

        private void OnFighterDodged(Fighter dodger)
        {
            TemplateContainer effectsPanel = SpawnEffectPanelForFighter(dodger);
            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = "Dodged!";
            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_dodgeStyleClass);
            StartCoroutine(DisplayWaitAndRemove(dodger, effectsPanel));
        }

        private IEnumerator DisplayWaitAndRemove(Fighter holder, TemplateContainer effectsPanel)
        {
            DisplayPanel(effectsPanel);
            yield return new WaitForSeconds(_displayDuration);
            HidePanel(effectsPanel);
            Destroy(_positioners[effectsPanel]);
            Destroy(_panelsAnchors[effectsPanel]);
            _fighterEffectsPanel[holder].Remove(effectsPanel);
            yield return new WaitForSeconds(0.2f);
            effectsPanel.RemoveFromHierarchy();
        }

        private TemplateContainer SpawnEffectPanelForFighter(Fighter fighter)
        {
            // Prepare panel anchor
            GameObject anchor = new()
            {
                name = $"{fighter.name}EffectFeedbackAnchor{_fighterEffectsPanel[fighter].Count}"
            };
            anchor.transform.SetParent(fighter.transform);
            anchor.transform.position = GetRandomSpawnPositionAroundFighter(fighter);

            // Prepare the panel
            TemplateContainer effectsPanel = _effectsUIPanel.Instantiate();
            effectsPanel.name = $"{fighter.name}EffectFeedbackPanel{_fighterEffectsPanel[fighter].Count}";
            HidePanel(effectsPanel);

            // Create a WorldUIPositioner to manage the panel's position
            WorldUIPositioner positioner = gameObject.AddComponent<WorldUIPositioner>();
            positioner.Setup(_uiDoc, effectsPanel, anchor.transform);

            // Track the panel for this fighter
            _positioners.Add(effectsPanel, positioner);
            _panelsAnchors.Add(effectsPanel, anchor);
            _fighterEffectsPanel[fighter].Add(effectsPanel);

            // Add the panel to the UI
            _uiDoc.rootVisualElement.Add(effectsPanel);

            return effectsPanel;
        }

        private void DisplayPanel(TemplateContainer effectsPanel)
        {
            effectsPanel.Q<VisualElement>(CONTAINER_UI_NAME).RemoveFromClassList("effectFeedbackContainerHidden");
        }

        private void HidePanel(TemplateContainer effectsPanel)
        {
            effectsPanel.Q<VisualElement>(CONTAINER_UI_NAME).AddToClassList("effectFeedbackContainerHidden");
        }

        private Vector2 GetRandomSpawnPositionAroundFighter(Fighter fighter)
        {
            // Convert the world-space center to screen space
            Vector3 fighterScreenPosition = Camera.main.WorldToScreenPoint(fighter.transform.position);

            // Get the camera's FOV scaling factor
            var fovScale = GetFovScalingFactor();

            // Apply FOV scaling to the margin
            var scaledMarginX = _displayMarginFromFighter.x * 2 * fovScale;
            var scaledMarginY = _displayMarginFromFighter.y * fovScale;

            // Calculate a random position around the center in screen space
            return Camera.main.ScreenToWorldPoint(
                new Vector3(
                    Random.Range(fighterScreenPosition.x - scaledMarginX, fighterScreenPosition.x + scaledMarginX),
                    Screen.height - Random.Range(fighterScreenPosition.y - scaledMarginY,
                        fighterScreenPosition.y + scaledMarginY),
                    fighterScreenPosition.z
                )
            );
        }

        private float GetFovScalingFactor()
        {
            // Scale proportionally to the base FOV
            return _fightCameraController.BaseFOV / Camera.main.fieldOfView;
        }
    }
}