using System;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Audio;


namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    /// Reperesents an active ability that can be used during a fight.
    /// </summary>
    [CreateAssetMenu(fileName = "ActiveAbility", menuName = "ScriptableObjects/Fight/Abilities/ActiveAbility", order = 0)]
    public class ActiveAbilitySO : ABaseAbility
    {
        [field: SerializeField] public Targeter Targeter { get; private set; }
        [field: SerializeField, Range(0, 99)] public int ActionPointsCost { get; private set; }
        [field: SerializeField, Range(0, 99)] public int GodFavorsPointsCost { get; private set; }
        [field: SerializeField] public bool Dodgable { get; private set; }
        [field: SerializeField] public bool Masterstrokable { get; private set; }
        [SerializeReference] public AEffect[] Effects;
        [SerializeReference] public AEffect[] MasterstrokeEffects = { };
        [SerializeReference] public AFightCellAlteration[] CellAlterations = { };
        [field: SerializeField] public AAbilityAnimationSO Animation { get; private set; }
        // Nouveau champ pour le son
        [field: SerializeField] public AudioClip AbilitySoundFX { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float SoundVolume { get; private set; } = 1f;

        [field: SerializeField, Range(0f, 3f)] public float DurationFadeOut { get; private set; } = 0.7f;

        public Action<ActiveAbilitySO> onActiveAbilityEnded;

        private Fighter _currentInitiator;

        /// <summary>
        /// Trigger the ability on the targeted cells.
        /// </summary>
        /// <param name="targetedCells">The cells that are targeted by the ability.</param>
        /// <param name="initiator">The fighter that initiated the ability.</param>
        public void Trigger(FightCell[] targetedCells, Fighter initiator)
        {
            // Jouer le son si configuré
            if (AbilitySoundFX != null)
            {
                // Vous devrez avoir un système audio dans votre jeu pour jouer les sons
                // Par exemple :
                AudioManager.instance.PlayFXSound(AbilitySoundFX, initiator.transform, SoundVolume, DurationFadeOut);
                // Ou utiliser votre propre système audio
            }
            else
            {
             Debug.Log($"There is no Ability Sound FX.");
            }
            
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
            onActiveAbilityEnded?.Invoke(this);
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
            {
                effect.ApplyEffect(
                    receiver: receiver,
                    isMasterstroke: isMasterstroke,
                    initiator: initiator,
                    adjustGodFavorsPoints: true
                );
            }

            if (isMasterstroke)
            {
                Debug.Log($"{initiator.name} masterstrokes the ability {Name}");
                foreach (AEffect effect in MasterstrokeEffects)
                {
                    effect.ApplyEffect(
                        receiver: receiver,
                        isMasterstroke: false,  // Masterstroke effects can't be masterstroked
                        initiator: initiator,
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