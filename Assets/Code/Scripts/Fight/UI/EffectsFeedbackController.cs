using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;

namespace FrostfallSaga.Fight.UI
{
    public class EffectsFeedbackController : BaseUIController
    {
        private static readonly string CONTAINER_PATH = "EffectFeedbackContainer";
        private static readonly string EFFECT_LABEL_PATH = "EffectFeedbackLabel";
        private static readonly string EFFECT_ICON_PATH = "EffectFeedbackIcon";

        [SerializeField, Header("UI options")] private VisualTreeAsset _effectsUIPanel;
        [SerializeField] private float _displayDuration = 2f;
        [SerializeField] private string _damageStyleClass;
        [SerializeField] private string _healStyleClass;
        [SerializeField] private string _buffAppliedStyleClass;
        [SerializeField] private string _debuffAppliedStyleClass;
        [SerializeField] private string _statusRemovedStyleClass;
        [SerializeField] private string _dodgeStyleClass;
        [SerializeField] private string _masterstrokeStyleClass;
        [SerializeField] private SElementToValue<EFighterMutableStat, Texture2D>[] _statIcons;

        [SerializeField] private FightersGenerator _fightersGenerator;

        private Dictionary<Fighter, TemplateContainer> _fighterEffectsPanel = new();

        private void OnFightersGenerated(Fighter[] allies, Fighter[] enemies)
        {
            foreach (Fighter fighter in allies.Concat(enemies))
            {
                TemplateContainer effectsPanel = _effectsUIPanel.Instantiate();
                HidePanel(effectsPanel);
                _uiDoc.rootVisualElement.Add(effectsPanel);

                _fighterEffectsPanel[fighter] = effectsPanel;
                fighter.onDamageReceived += OnFighterReceivedDamages;
                fighter.onHealReceived += OnFighterReceivedHeal;
                fighter.onActionDodged += OnFighterDodged;
                fighter.onStatusApplied += OnStatusAppliedOnFighter;
                fighter.onStatusRemoved += OnStatusRemovedFromFighter;
                fighter.onStatMutationReceived += OnFighterStatMutationReceived;
            }
        }

        private void OnFighterReceivedDamages(Fighter receiver, int damageAmount, bool isMasterstroke)
        {
            if (!_fighterEffectsPanel.ContainsKey(receiver))
            {
                Debug.LogError($"Fighter {receiver} is not in the list of fighters with effects panels.");
                return;
            }

            TemplateContainer effectsPanel = _fighterEffectsPanel[receiver];
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).text = $"{damageAmount}{(isMasterstroke ? "!" : "")}";
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).AddToClassList(_damageStyleClass);
            if (isMasterstroke) effectsPanel.Q<Label>(EFFECT_LABEL_PATH).AddToClassList(_masterstrokeStyleClass);
            StartCoroutine(DisplayAndWait(effectsPanel));
        }


        private void OnFighterReceivedHeal(Fighter receiver, int healAmount, bool isMasterstroke)
        {
            if (!_fighterEffectsPanel.ContainsKey(receiver))
            {
                Debug.LogError($"Fighter {receiver} is not in the list of fighters with effects panels.");
                return;
            }

            TemplateContainer effectsPanel = _fighterEffectsPanel[receiver];
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).text = $"{healAmount}{(isMasterstroke ? "!" : "")}";
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).AddToClassList(_healStyleClass);
            if (isMasterstroke) effectsPanel.Q<Label>(EFFECT_LABEL_PATH).AddToClassList(_masterstrokeStyleClass);
            StartCoroutine(DisplayAndWait(effectsPanel));
        }

        private void OnFighterDodged(Fighter dodger)
        {
            if (!_fighterEffectsPanel.ContainsKey(dodger))
            {
                Debug.LogError($"Fighter {dodger} is not in the list of fighters with effects panels.");
                return;
            }

            TemplateContainer effectsPanel = _fighterEffectsPanel[dodger];
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).text = "Dodged!";
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).AddToClassList(_dodgeStyleClass);
            StartCoroutine(DisplayAndWait(effectsPanel));
        }

        private void OnStatusAppliedOnFighter(Fighter fighter, AStatus receivedStatus)
        {
            if (!_fighterEffectsPanel.ContainsKey(fighter))
            {
                Debug.LogError($"Fighter {fighter} is not in the list of fighters with effects panels.");
                return;
            }

            TemplateContainer effectsPanel = _fighterEffectsPanel[fighter];
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).text = $"+{receivedStatus.Name}";
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).AddToClassList(
                receivedStatus.StatusType.IsBuff() ? _buffAppliedStyleClass : _debuffAppliedStyleClass
            );
            effectsPanel.Q<VisualElement>(EFFECT_ICON_PATH).style.backgroundImage = new(receivedStatus.Icon);
            StartCoroutine(DisplayAndWait(effectsPanel));
        }

        private void OnStatusRemovedFromFighter(Fighter fighter, AStatus removedStatus)
        {
            if (!_fighterEffectsPanel.ContainsKey(fighter))
            {
                Debug.LogError($"Fighter {fighter} is not in the list of fighters with effects panels.");
                return;
            }

            TemplateContainer effectsPanel = _fighterEffectsPanel[fighter];
            effectsPanel.Q<VisualElement>(EFFECT_ICON_PATH).style.backgroundImage = new(removedStatus.Icon);
            effectsPanel.Q<VisualElement>(EFFECT_ICON_PATH).style.flexGrow = (StyleFloat)0.75;
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).text = $"-{removedStatus.Name}";
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).AddToClassList(_statusRemovedStyleClass);
            StartCoroutine(DisplayAndWait(effectsPanel));
        }

        private void OnFighterStatMutationReceived(Fighter fighter, EFighterMutableStat statType, float mutationAmount)
        {
            if (!_fighterEffectsPanel.ContainsKey(fighter))
            {
                Debug.LogError($"Fighter {fighter} is not in the list of fighters with effects panels.");
                return;
            }

            Dictionary<EFighterMutableStat, Texture2D> statIcons =
                SElementToValue<EFighterMutableStat, Texture2D>.GetDictionaryFromArray(
                    _statIcons
                );

            bool isDebuff = mutationAmount < 0f;
            TemplateContainer effectsPanel = _fighterEffectsPanel[fighter];
            effectsPanel.Q<VisualElement>(EFFECT_ICON_PATH).style.backgroundImage = new(statIcons[statType]);
            effectsPanel.Q<VisualElement>(EFFECT_ICON_PATH).style.flexGrow = (StyleFloat)0.75;
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).text = $"{(isDebuff ? "-" : "+")}{mutationAmount}";
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).AddToClassList(isDebuff ? _debuffAppliedStyleClass : _buffAppliedStyleClass);
            StartCoroutine(DisplayAndWait(effectsPanel));
        }

        private IEnumerator DisplayAndWait(TemplateContainer effectsPanel)
        {
            DisplayPanel(effectsPanel);
            yield return new WaitForSeconds(_displayDuration);
            HidePanel(effectsPanel);
        }

        private void DisplayPanel(TemplateContainer effectsPanel)
        {
            effectsPanel.Q<VisualElement>(CONTAINER_PATH).RemoveFromClassList("effectFeedbackContainerHidden");
        }

        private void HidePanel(TemplateContainer effectsPanel)
        {
            effectsPanel.Q<VisualElement>(EFFECT_ICON_PATH).style.backgroundImage = null;
            effectsPanel.Q<VisualElement>(EFFECT_ICON_PATH).style.flexGrow = 0;
            effectsPanel.Q<VisualElement>(CONTAINER_PATH).AddToClassList("effectFeedbackContainerHidden");
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).ClearClassList();
            effectsPanel.Q<Label>(EFFECT_LABEL_PATH).AddToClassList("effectLabelDefault");
        }

        private void Update()
        {
            foreach (Fighter fighter in _fighterEffectsPanel.Keys)
            {
                Vector3 fighterScreenPosition = Camera.main.WorldToScreenPoint(fighter.transform.position);
                TemplateContainer effectsPanel = _fighterEffectsPanel[fighter];
                effectsPanel.style.left = fighterScreenPosition.x;
                effectsPanel.style.top = Screen.height - fighterScreenPosition.y - 50;
            }
        }

        #region Setup & teardown

        private void Awake()
        {
            _fightersGenerator.onFightersGenerated += OnFightersGenerated;
        }

        #endregion
    }
}