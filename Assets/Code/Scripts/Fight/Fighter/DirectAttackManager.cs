using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.FightCells;
using UnityEngine;

namespace FrostfallSaga.Fight.Fighters
{
    public class DirectAttackManager
    {
        private readonly Fighter _controlledFighter;
        private readonly List<Fighter> _touchedFighters = new();
        public Action onDirectAttackEnded;

        public DirectAttackManager(Fighter fighter)
        {
            _controlledFighter = fighter;
        }

        public void DirectAttack(List<FightCell> targetedCells)
        {
            // Trigger the direct attack (with or without animation)
            if (_controlledFighter.Weapon.AttackAnimation == null)
            {
                Debug.LogWarning($"No animation attached to direct attack for fighter {_controlledFighter.name}");
                targetedCells
                    .Where(cell => cell.HasFighter()).ToList()
                    .ForEach(cell =>
                        {
                            if (cell.Fighter.TryDodge())
                            {
                                cell.Fighter.onActionDodged?.Invoke(cell.Fighter);
                                return;
                            }

                            ApplyEffectsOnFighter(
                                _controlledFighter.Weapon.GetWeaponEffects(cell.Fighter.Race),
                                cell.Fighter,
                                _controlledFighter.TryMasterstroke()
                            );
                        }
                    );
                onDirectAttackEnded?.Invoke();
            }
            else
            {
                _controlledFighter.Weapon.AttackAnimation.onFighterTouched += OnDirectAttackTouchedFighter;
                _controlledFighter.Weapon.AttackAnimation.onAnimationEnded += OnDirectAttackAnimationEnded;
                _controlledFighter.Weapon.AttackAnimation.Execute(_controlledFighter, targetedCells.ToArray());
            }
        }

        private void OnDirectAttackTouchedFighter(Fighter touchedFighter)
        {
            if (_touchedFighters.Contains(touchedFighter)) return;
            _touchedFighters.Add(touchedFighter);

            if (touchedFighter.TryDodge())
            {
                touchedFighter.onActionDodged?.Invoke(touchedFighter);
                return;
            }

            ApplyEffectsOnFighter(
                _controlledFighter.Weapon.GetWeaponEffects(touchedFighter.Race),
                touchedFighter,
                _controlledFighter.TryMasterstroke()
            );
        }

        private void OnDirectAttackAnimationEnded(Fighter initiator)
        {
            _controlledFighter.Weapon.AttackAnimation.onFighterTouched -= OnDirectAttackTouchedFighter;
            _controlledFighter.Weapon.AttackAnimation.onAnimationEnded -= OnDirectAttackAnimationEnded;
            _touchedFighters.Clear();
            onDirectAttackEnded?.Invoke();
        }

        /// <summary>
        ///     Apply a list of effects to the targeted fighter.
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
                    target,
                    isMasterstroke,
                    _controlledFighter
                )
            );
        }
    }
}