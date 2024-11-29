using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Effects;

namespace FrostfallSaga.Fight.Fighters
{
    public class DirectAttackManager
    {
        public Action onDirectAttackEnded;
        private readonly Fighter _controlledFighter;

        public DirectAttackManager(Fighter fighter)
        {
            _controlledFighter = fighter;
        }

        public void DirectAttack(List<FightCell> targetedCells)
        {
            // Trigger the direct attack (with or without animation)
            if (_controlledFighter.DirectAttackAnimation == null)
            {
                Debug.LogWarning($"No animation attached to direct attack for fighter {_controlledFighter.name}");
                targetedCells
                    .Where(cell => cell.HasFighter()).ToList()
                    .ForEach(cell =>
                        {
                            if (cell.Fighter.TryDodge())
                            {
                                cell.Fighter.onActionDodged?.Invoke(cell.Fighter, _controlledFighter);
                                return;
                            }
                            ApplyEffectsOnFighter(
                                _controlledFighter.Weapon.GetWeaponEffects(cell.Fighter.EntityID),
                                cell.Fighter,
                                _controlledFighter.TryMasterstroke()
                            );
                        }
                    );
                onDirectAttackEnded?.Invoke();
            }
            else
            {
                _controlledFighter.DirectAttackAnimation.onFighterTouched += OnDirectAttackTouchedFighter;
                _controlledFighter.DirectAttackAnimation.onAnimationEnded += OnDirectAttackAnimationEnded;
                _controlledFighter.DirectAttackAnimation.Execute(_controlledFighter, targetedCells.ToArray());
            }
        }

        private void OnDirectAttackTouchedFighter(Fighter touchedFighter)
        {
            if (touchedFighter.TryDodge())
            {
                touchedFighter.onActionDodged?.Invoke(touchedFighter, _controlledFighter);
                return;
            }
            ApplyEffectsOnFighter(
                _controlledFighter.Weapon.GetWeaponEffects(touchedFighter.EntityID), 
                touchedFighter,
                _controlledFighter.TryMasterstroke()
            );
        }

        private void OnDirectAttackAnimationEnded(Fighter initiator)
        {
            _controlledFighter.DirectAttackAnimation.onFighterTouched -= OnDirectAttackTouchedFighter;
            _controlledFighter.DirectAttackAnimation.onAnimationEnded -= OnDirectAttackAnimationEnded;
            onDirectAttackEnded?.Invoke();
        }

        /// <summary>
        /// Apply a list of effects to the targeted fighter.
        /// </summary>
        /// <param name="effectsToApply">The effects to apply.</param>
        /// <param name="target">The fighter to apply the effects to.</param>
        /// <param name="isMasterstroke">If the initiator made a masterstroke.</param>
        private void ApplyEffectsOnFighter(
            AEffect[] effectsToApply,
            Fighter target,
            bool isMasterstroke
        )
        {
            effectsToApply.ToList().ForEach(
                effect => effect.ApplyEffect(
                    receiver: target,
                    isMasterstroke: isMasterstroke,
                    initator: _controlledFighter,
                    adjustGodFavorsPoints: true
                )
            );
        }
    }
}