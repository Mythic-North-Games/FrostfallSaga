using System;
using System.Linq;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Targeters;
using UnityEngine;

namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    ///     Reperesents an active ability that can be used during a fight.
    /// </summary>
    [CreateAssetMenu(fileName = "ActiveAbility", menuName = "ScriptableObjects/Fight/Abilities/ActiveAbility", order = 0)]
    public class ActiveAbilitySO : ABaseAbility
    {
        [field: SerializeField] public Targeter Targeter { get; private set; }

        [field: SerializeField]
        [field: Range(0, 99)]
        public int ActionPointsCost { get; private set; }

        [field: SerializeField]
        [field: Range(0, 99)]
        public int GodFavorsPointsCost { get; private set; }

        [field: SerializeField] public bool Dodgable { get; private set; }
        [field: SerializeField] public bool Masterstrokable { get; private set; }
        [SerializeReference] public AEffect[] Effects;
        [SerializeReference] public AEffect[] MasterstrokeEffects = { };
        [SerializeReference] public AFightCellAlteration[] CellAlterations = { };
        [field: SerializeField] public AAbilityAnimationSO Animation { get; private set; }

        private Fighter _currentInitiator;

        public Action<ActiveAbilitySO> OnActiveAbilityEnded;

        /// <summary>
        /// Trigger the ability on the targeted cells.
        /// </summary>
        /// <param name="targetedCells">The cells that are targeted by the ability.</param>
        /// <param name="initiator">The fighter that initiated the ability.</param>
        public void Trigger(FightCell[] targetedCells, Fighter initiator)
        {
            if (Animation == null)
            {
                Debug.LogWarning($"No animation attached to active ability {Name}.");
                targetedCells.ToList()
                    .Where(cell => cell.HasFighter()).ToList()
                    .ForEach(cell =>
                        {
                            if (cell.HasFighter()) ApplyAbilityToFighter(cell.Fighter, initiator);
                            ApplyAlterationsToCell(cell);
                        }
                    );
                OnActiveAbilityEnded?.Invoke(this);
            }
            else
            {
                _currentInitiator = initiator;
                Animation.onFighterTouched += OnActiveAbilityTouchedFighter;
                Animation.onCellTouched += OnActiveAbilityTouchedCell;
                Animation.onAnimationEnded += OnActiveAbilityAnimationEnded;
                Animation.Execute(initiator, targetedCells);
            }
        }

        /// <summary>
        /// Compute the potential damages that the ability can do to the target.
        /// </summary>
        /// <param name="initiator">The initiator of the ability.</param>
        /// <param name="target">The target that will receive the ability effects.</param>
        /// <returns>The potential damages that the ability can do in this configuration to the target.</returns>
        public int GetDamagesPotential(Fighter initiator, Fighter target)
        {
            return Effects.Sum(
                effect => effect.GetPotentialEffectDamages(initiator, target, Masterstrokable)
            );
        }

        /// <summary>
        /// Compute the potential heal that the ability can do to the target.
        /// </summary>
        /// <param name="initiator">The initiator of the ability.</param>
        /// <param name="target">The target that will receive the ability effects.</param>
        /// <returns>The potential heal that the ability can do in this configuration to the target.</returns>
        public int GetHealPotential(Fighter initiator, Fighter target)
        {
            return Effects.Sum(
                effect => effect.GetPotentialEffectHeal(initiator, target, Masterstrokable)
            );
        }

        #region Ability application management

        private void OnActiveAbilityTouchedFighter(Fighter fighter)
        {
            ApplyAbilityToFighter(fighter, _currentInitiator);
        }

        private void OnActiveAbilityTouchedCell(FightCell cell)
        {
            ApplyAlterationsToCell(cell);
        }

        private void OnActiveAbilityAnimationEnded(Fighter initiator)
        {
            Animation.onFighterTouched -= OnActiveAbilityTouchedFighter;
            Animation.onCellTouched -= OnActiveAbilityTouchedCell;
            Animation.onAnimationEnded -= OnActiveAbilityAnimationEnded;
            OnActiveAbilityEnded?.Invoke(this);
        }

        private void ApplyAbilityToFighter(Fighter receiver, Fighter initiator)
        {
            if (Dodgable && receiver.TryDodge())
            {
                receiver.onActionDodged?.Invoke(initiator);
                Debug.Log($"{receiver.name} dodged the ability {Name}");
                return;
            }

            bool isMasterstroke = Masterstrokable && initiator.TryMasterstroke();
            foreach (AEffect effect in Effects)
                effect.ApplyEffect(
                    receiver,
                    isMasterstroke,
                    initiator
                );

            if (isMasterstroke)
            {
                Debug.Log($"{initiator.name} masterstrokes the ability {Name}");
                foreach (AEffect effect in MasterstrokeEffects)
                    effect.ApplyEffect(
                        receiver,
                        false, // Masterstroke effects can't be masterstroked
                        initiator,
                        false
                    );
            }
        }

        private void ApplyAlterationsToCell(FightCell cell)
        {
            foreach (AFightCellAlteration alteration in CellAlterations) alteration.Apply(cell);
        }
    }

    #endregion
}