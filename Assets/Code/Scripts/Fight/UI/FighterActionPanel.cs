using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.UI
{
    /// TODO: This class has only been used to test the player controller.
    /// TODO: It can be re-used or extended to serve as the real fighter action panel script or
    /// TODO: can be deleted if another one is created. In this case, the player controller should
    /// TODO: also be updated to listen to the action panel events.
    public class FighterActionPanel : MonoBehaviour
    {
        public Action onDirectAttackClicked;
        public Action<ActiveAbilityToAnimation> onActiveAbilityClicked;
        public Action onEndTurnClicked;

        [SerializeField] private ActiveAbilityToAnimation _ability;

        public void TriggerDirectAttackEvent()
        {
            onDirectAttackClicked?.Invoke();
        }

        public void TriggerActiveAbilityEvent()
        {
            onActiveAbilityClicked?.Invoke(_ability);
        }

        public void TriggerEndTurnEvent()
        {
            onEndTurnClicked?.Invoke();
        }
    }
}