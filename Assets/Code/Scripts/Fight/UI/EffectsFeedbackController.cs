using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Camera;
using FrostfallSaga.Utils.UI;

namespace FrostfallSaga.Fight.UI
{
    public class EffectsFeedbackController : BaseUIController
    {
        private static readonly string CONTAINER_UI_NAME = "EffectFeedbackContainer";
        private static readonly string EFFECT_LABEL_UI_NAME = "EffectFeedbackLabel";
        private static readonly string EFFECT_ICON_UI_NAME = "EffectFeedbackIcon";

        [SerializeField, Header("UI options")] private VisualTreeAsset _effectsUIPanel;
        [SerializeField] private float _displayDuration = 2f;
        [SerializeField] private Vector2Int _displayMarginFromFighter = new(1, 1);
        [SerializeField] private string _damageStyleClass;
        [SerializeField] private string _healStyleClass;
        [SerializeField] private string _buffAppliedStyleClass;
        [SerializeField] private string _debuffAppliedStyleClass;
        [SerializeField] private string _dodgeStyleClass;
        [SerializeField] private string _masterstrokeStyleClass;
        [SerializeField] private SElementToValue<EFighterMutableStat, Texture2D>[] _statIcons;
        [SerializeField, Header("Needed components")] private FightersGenerator _fightersGenerator;
        [SerializeField] private CameraController _fightCameraController;

        private Dictionary<Fighter, List<TemplateContainer>> _fighterEffectsPanel = new();

        private void OnFightersGenerated(Fighter[] allies, Fighter[] enemies)
        {
            foreach (Fighter fighter in allies.Concat(enemies))
            {
                _fighterEffectsPanel.Add(fighter, new());
                fighter.onDamageReceived += OnFighterReceivedDamages;
                fighter.onHealReceived += OnFighterReceivedHeal;
                fighter.onActionDodged += OnFighterDodged;
                fighter.onNonMagicalStatMutated += OnFighterNonMagicalStatMutated;
            }
        }

        private void OnFighterReceivedDamages(Fighter receiver, int damageAmount, bool isMasterstroke)
        {
            if (isMasterstroke)
            {
                TemplateContainer masterstrokePanel = SpawnEffectPanelForFighter(receiver);
                masterstrokePanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = "Critical!";
                masterstrokePanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_damageStyleClass);
                masterstrokePanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_masterstrokeStyleClass);
            }

            TemplateContainer damagesPanel = SpawnEffectPanelForFighter(receiver);
            damagesPanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = $"{damageAmount}{(isMasterstroke ? "!" : "")}";
            damagesPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_damageStyleClass);
            if (isMasterstroke) damagesPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_masterstrokeStyleClass);

            _fighterEffectsPanel[receiver].ForEach(panel => StartCoroutine(DisplayWaitAndRemove(receiver, panel)));
        }


        private void OnFighterReceivedHeal(Fighter receiver, int healAmount, bool isMasterstroke)
        {
            if (isMasterstroke)
            {
                TemplateContainer masterstrokePanel = SpawnEffectPanelForFighter(receiver);
                masterstrokePanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = "Masterstroke!";
                masterstrokePanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_healStyleClass);
                masterstrokePanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_masterstrokeStyleClass);
            }

            TemplateContainer effectsPanel = SpawnEffectPanelForFighter(receiver);
            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = $"{healAmount}{(isMasterstroke ? "!" : "")}";
            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_healStyleClass);
            if (isMasterstroke) effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_masterstrokeStyleClass);

            _fighterEffectsPanel[receiver].ForEach(panel => StartCoroutine(DisplayWaitAndRemove(receiver, panel)));
        }

        private void OnFighterDodged(Fighter dodger)
        {
            TemplateContainer effectsPanel = SpawnEffectPanelForFighter(dodger);
            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = "Dodged!";
            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_dodgeStyleClass);
            StartCoroutine(DisplayWaitAndRemove(dodger, effectsPanel));
        }

        private void OnFighterNonMagicalStatMutated(Fighter fighter, EFighterMutableStat statType, float mutationAmount)
        {

            Dictionary<EFighterMutableStat, Texture2D> statIcons =
                SElementToValue<EFighterMutableStat, Texture2D>.GetDictionaryFromArray(
                    _statIcons
                );
            bool isDebuff = mutationAmount < 0f;

            TemplateContainer effectsPanel = SpawnEffectPanelForFighter(fighter);
            effectsPanel.Q<VisualElement>(EFFECT_ICON_UI_NAME).style.backgroundImage = new(statIcons[statType]);
            effectsPanel.Q<VisualElement>(EFFECT_ICON_UI_NAME).style.flexGrow = (StyleFloat)0.75;
            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = $"{(isDebuff ? "" : "+")}{mutationAmount}";
            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(isDebuff ? _debuffAppliedStyleClass : _buffAppliedStyleClass);
            StartCoroutine(DisplayWaitAndRemove(fighter, effectsPanel));
        }

        private IEnumerator DisplayWaitAndRemove(Fighter holder, TemplateContainer effectsPanel)
        {
            DisplayPanel(effectsPanel);
            yield return new WaitForSeconds(_displayDuration);
            HidePanel(effectsPanel);
            _fighterEffectsPanel[holder].Remove(effectsPanel);
            yield return new WaitForSeconds(0.2f);
            effectsPanel.RemoveFromHierarchy();
        }

        private TemplateContainer SpawnEffectPanelForFighter(Fighter fighter)
        {
            // Intantiate the effect panel
            TemplateContainer effectsPanel = _effectsUIPanel.Instantiate();

            // Prepare the panel
            effectsPanel.name = $"{fighter.name}EffectFeedbackPanel{_fighterEffectsPanel[fighter].Count}";
            effectsPanel.Q<VisualElement>(EFFECT_ICON_UI_NAME).style.backgroundImage = null;

            effectsPanel.transform.position = GetRandomSpawnPositionAroundFighter(fighter);
            HidePanel(effectsPanel);

            // Add the panel to the UI
            _uiDoc.rootVisualElement.Add(effectsPanel);
            _fighterEffectsPanel[fighter].Add(effectsPanel);
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
            float fovScale = GetFovScalingFactor();

            // Apply FOV scaling to the margin
            float scaledMarginX = _displayMarginFromFighter.x * 2 * fovScale;
            float scaledMarginY = _displayMarginFromFighter.y * fovScale;

            // Calculate a random position around the center in screen space
            return new Vector2(
                Random.Range(fighterScreenPosition.x - scaledMarginX, fighterScreenPosition.x + scaledMarginX),
                Screen.height - Random.Range(fighterScreenPosition.y - scaledMarginY, fighterScreenPosition.y + scaledMarginY)
            );
        }

        private float GetFovScalingFactor()
        {
            // Scale proportionally to the base FOV
            return _fightCameraController.BaseFOV / Camera.main.fieldOfView;
        }

        #region Setup & teardown

        private void Awake()
        {
            _fightersGenerator.onFightersGenerated += OnFightersGenerated;
        }

        #endregion
    }
}