using UnityEngine.UIElements;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.UI
{
    /// <summary>
    /// Controller for the fighter progress bars in the UI.
    /// </summary>
    public class FighterProgressBarsController
    {
        private static readonly string HP_LABEL_UI_NAME = "LifeBarLabel";
        private static readonly string HP_PROGRESS_UI_NAME = "LifeBarProgress";
        private static readonly string AP_LABEL_UI_NAME = "ActionBarLabel";
        private static readonly string AP_PROGRESS_UI_NAME = "ActionBarProgress";
        private static readonly string MP_LABEL_UI_NAME = "MoveBarLabel";
        private static readonly string MP_PROGRESS_UI_NAME = "MoveBarProgress";

        private readonly VisualElement _root;

        public FighterProgressBarsController(VisualElement root)
        {
            _root = root;
        }

        public void UpdateAllBars(Fighter fighter)
        {
            UpdateHealthBar(fighter);
            UpdateActionBar(fighter);
            UpdateMoveBar(fighter);
        }

        public void UpdateHealthBar(Fighter fighter)
        {
            Label fighterHpLabel = _root.Q<Label>(HP_LABEL_UI_NAME);
            VisualElement fighterHpProgress = _root.Q<VisualElement>(HP_PROGRESS_UI_NAME);
            fighterHpLabel.text = $"{fighter.GetHealth()}/{fighter.GetMaxHealth()}";
            fighterHpProgress.style.width = new Length(
                (float)fighter.GetHealth() / fighter.GetMaxHealth() * 100,
                LengthUnit.Percent
            );
        }

        public void UpdateActionBar(Fighter fighter)
        {
            Label fighterApLabel = _root.Q<Label>(AP_LABEL_UI_NAME);
            VisualElement fighterApProgress = _root.Q<VisualElement>(AP_PROGRESS_UI_NAME);
            fighterApLabel.text = $"{fighter.GetActionPoints()}/{fighter.GetMaxActionPoints()}";
            fighterApProgress.style.width = new Length(
                (float)fighter.GetActionPoints() / fighter.GetMaxActionPoints() * 100,
                LengthUnit.Percent
            );
        }

        public void UpdateMoveBar(Fighter fighter)
        {
            Label fighterMpLabel = _root.Q<Label>(MP_LABEL_UI_NAME);
            VisualElement fighterMpProgress = _root.Q<VisualElement>(MP_PROGRESS_UI_NAME);
            fighterMpLabel.text = $"{fighter.GetMovePoints()}/{fighter.GetMaxMovePoints()}";
            fighterMpProgress.style.width = new Length(
                (float)fighter.GetMovePoints() / fighter.GetMaxMovePoints() * 100,
                LengthUnit.Percent
            );
        }
    }
}