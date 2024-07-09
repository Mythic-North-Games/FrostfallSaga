using System;
using UnityEngine;
using FrostfallSaga.Fight.Abilities;

namespace FrostfallSaga.Fight.UI
{
    public class FighterActionPanel : MonoBehaviour
    {
        public Action onDirectAttackClicked;
        public Action<ActiveAbilitySO> onActiveAbilityClicked;
        public Action onEndTurnClicked;
        
        [SerializeField] private ActiveAbilitySO ability;

        public void TriggerDirectAttackEvent()
        {
            onDirectAttackClicked?.Invoke();
        }

        public void TriggerActiveAbilityEvent()
        {
            onActiveAbilityClicked?.Invoke(ability);
        }

        public void TriggerEndTurnEvent()
        {
            onEndTurnClicked?.Invoke();
        }
    }
}