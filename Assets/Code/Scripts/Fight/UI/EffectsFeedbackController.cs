using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.UI;
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

        [SerializeField]
        [Header("UI options")]
        private VisualTreeAsset _effectsUIPanel;

        [SerializeField] private float _displayDuration = 2f;
        [SerializeField] private Vector3 _displayMarginFromFighter = new(0.5f, 1f);
        [SerializeField] private string _damageStyleClass;
        [SerializeField] private string _healStyleClass;
        [SerializeField] private string _dodgeStyleClass;
        [SerializeField] private string _masterstrokeStyleClass;
        [SerializeField] private SElementToValue<EFighterMutableStat, Texture2D>[] _statIcons;

        [SerializeField]
        [Header("Needed components")]
        private FightLoader _fightLoader;

        [SerializeField] private CameraController _fightCameraController;

        private readonly Dictionary<Fighter, List<VisualElement>> _fighterEffectsPanel = new();
        private readonly Dictionary<VisualElement, GameObject> _panelsAnchors = new();
        private readonly Dictionary<VisualElement, WorldUIPositioner> _positioners = new();

        #region Setup & teardown

        private void Awake()
        {
            _fightLoader.OnFightLoaded += OnFightLoaded;
        }

        #endregion

        private void OnFightLoaded(Fighter[] allies, Fighter[] enemies)
        {
            foreach (Fighter fighter in allies.Concat(enemies))
            {
                _fighterEffectsPanel.Add(fighter, new List<VisualElement>());
                fighter.onDamageReceived += OnFighterReceivedDamages;
                fighter.onHealReceived += OnFighterReceivedHeal;
                fighter.onActionDodged += OnFighterDodged;
            }
        }

        private void OnFighterReceivedDamages(Fighter receiver, int damageAmount, bool isMasterstroke)
        {
            VisualElement effectsPanel = SpawnEffectPanelForFighter(receiver);
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
            VisualElement effectsPanel = SpawnEffectPanelForFighter(receiver);
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
            VisualElement effectsPanel = SpawnEffectPanelForFighter(dodger);
            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).text = "Dodged!";
            effectsPanel.Q<Label>(EFFECT_LABEL_UI_NAME).AddToClassList(_dodgeStyleClass);
            StartCoroutine(DisplayWaitAndRemove(dodger, effectsPanel));
        }

        private IEnumerator DisplayWaitAndRemove(Fighter holder, VisualElement effectsPanel)
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

        private VisualElement SpawnEffectPanelForFighter(Fighter fighter)
        {
            // Prepare panel anchor
            GameObject anchor = new()
            {
                name = $"{fighter.name}EffectFeedbackAnchor{_fighterEffectsPanel[fighter].Count}"
            };
            anchor.transform.parent = fighter.transform;
            anchor.transform.SetLocalPositionAndRotation(
                GetNonOverlappingOffsetPosition(fighter),
                Quaternion.identity
            );

            // Prepare the panel
            VisualElement effectsPanel = _effectsUIPanel.Instantiate();
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

        private void DisplayPanel(VisualElement effectsPanel)
        {
            effectsPanel.Q<VisualElement>(CONTAINER_UI_NAME).RemoveFromClassList("effectFeedbackContainerHidden");
        }

        private void HidePanel(VisualElement effectsPanel)
        {
            effectsPanel.Q<VisualElement>(CONTAINER_UI_NAME).AddToClassList("effectFeedbackContainerHidden");
        }

        private Vector3 GetNonOverlappingOffsetPosition(Fighter fighter, float minDistance = 1f, int maxAttempts = 10)
        {
            Vector3 newPos = Vector3.zero;
            bool isValid = false;

            for (int attempt = 0; attempt < maxAttempts && !isValid; attempt++)
            {
                float randomX = Random.Range(-_displayMarginFromFighter.x, _displayMarginFromFighter.x);
                float randomY = Random.Range(-_displayMarginFromFighter.y, _displayMarginFromFighter.y);
                newPos = new Vector3(randomX, randomY + 1f, 0f);

                isValid = true; // assume it's valid unless overlap found

                foreach (var existingPanel in _fighterEffectsPanel[fighter])
                {
                    if (_panelsAnchors.TryGetValue(existingPanel, out GameObject anchorGO))
                    {
                        Vector3 existingPos = anchorGO.transform.localPosition;
                        if (Vector3.Distance(existingPos, newPos) < minDistance)
                        {
                            isValid = false;
                            break;
                        }
                    }
                }
            }

            return newPos;
        }
    }
}