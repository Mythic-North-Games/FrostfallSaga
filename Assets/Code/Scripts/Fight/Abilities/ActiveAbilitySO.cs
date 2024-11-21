using System;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;
using FrostfallSaga.Fight.FightCells;


namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    /// Reperesents an active ability that can be used during a fight.
    /// </summary>
    [CreateAssetMenu(fileName = "ActiveAbility", menuName = "ScriptableObjects/Fight/Abilities/ActiveAbility", order = 0)]
    public class ActiveAbilitySO : BaseAbilitySO
    {
        [field: SerializeField] public Targeter Targeter { get; private set; }
        [field: SerializeField, Range(0, 99)] public int ActionPointsCost { get; private set; }
        [field: SerializeField, Range(0, 99)] public int GodFavorsPointsCost { get; private set; }
        [field: SerializeField] public bool Dodgable { get; private set; }
        [field: SerializeField] public bool Masterstrokable { get; private set; }
        [SerializeReference] public AEffect[] MasterstrokeEffects = { };
        [SerializeReference] public AFightCellAlteration[] CellAlterations = { };
        [field: SerializeField] public AAbilityAnimationSO Animation { get; private set; }

        public Action<ActiveAbilitySO> onActiveAbilityEnded;

        private Fighter _currentInitiator;

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
                onActiveAbilityEnded?.Invoke(this);
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

        public int GetDamagesPotential(Fighter initiator, Fighter target)
        {
            return Effects.Sum(
                effect => effect.GetPotentialEffectDamages(initiator, target, Masterstrokable)
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
            onActiveAbilityEnded?.Invoke(this);
        }

        private void ApplyAbilityToFighter(Fighter receiver, Fighter initiator)
        {
            if (Dodgable && receiver.TryDodge())
            {
                receiver.onActionDodged?.Invoke(initiator, receiver);
                Debug.Log($"{receiver.name} dodged the ability {Name}");
                return;
            }

            bool isMasterstroke = Masterstrokable && initiator.TryMasterstroke();
            foreach (AEffect effect in Effects)
            {
                effect.ApplyEffect(
                    receiver: receiver,
                    isMasterstroke: isMasterstroke,
                    initator: initiator,
                    adjustGodFavorsPoints: true
                );
            }

            if (isMasterstroke)
            {
                foreach (AEffect effect in MasterstrokeEffects)
                {
                    effect.ApplyEffect(
                        receiver: receiver,
                        isMasterstroke: false,  // Masterstroke effects can't be masterstroked
                        initator: initiator,
                        adjustGodFavorsPoints: false
                    );
                }
            }
        }

        private void ApplyAlterationsToCell(FightCell cell)
        {
            foreach (AFightCellAlteration alteration in CellAlterations)
            {
                alteration.Apply(cell);
            }
        }
    }

    #endregion
}