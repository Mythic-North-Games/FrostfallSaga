using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Grid;
using System.Collections.Generic;
using System.Linq;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController.Actions
{
    public class DamageTargetAction : FBTNode
    {
        private readonly EDamagePreference damagePreference;

        public DamageTargetAction(
            Fighter possessedFighter,
            HexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams,
            EDamagePreference damagePreference
        ) : base(possessedFighter, fightGrid, fighterTeams)
        {
            this.damagePreference = damagePreference;
        }

        public override NodeState Evaluate()
        {
            Fighter target = (Fighter)GetSharedData("damageTarget");
            if (target == null)
            {
                return NodeState.FAILURE;
            }

            // Filter the active abilities that can be used on the target
            List<ActiveAbilityToAnimation> useableAbilitiesToAnimation = _possessedFighter.ActiveAbilitiesToAnimation.Where(
                activeAbilityToAnimation => _possessedFighter.CanUseActiveAbility(
                    _fightGrid, activeAbilityToAnimation.activeAbility, _fighterTeams, target
                )
            ).ToList();

            ActiveAbilityToAnimation preferedAbilityToAnimation = null;
            bool useActiveAbility = false;
            bool canUseDirectAttack = _possessedFighter.CanDirectAttack(_fightGrid, _fighterTeams, target);

            // Choose the ability to use based on the damage preference and decide if it's better to use the prefered ability or the direct attack
            switch (damagePreference)
            {
                case EDamagePreference.RANDOM:
                    preferedAbilityToAnimation = Randomizer.GetRandomElementFromArray(useableAbilitiesToAnimation.ToArray());
                    float directAttackChance = 1f / useableAbilitiesToAnimation.Count + 1;
                    useActiveAbility = !(canUseDirectAttack && Randomizer.GetBooleanOnChance(directAttackChance));
                    break;

                case EDamagePreference.MAXIMIZE_DAMAGE:
                    preferedAbilityToAnimation = useableAbilitiesToAnimation.OrderByDescending(
                        abilityToAnimation => GetPotentialsDamageOfAbilities(abilityToAnimation.activeAbility, target)
                    ).First();
                    useActiveAbility = (
                        GetPotentialsDamageOfAbilities(preferedAbilityToAnimation.activeAbility, target) >
                        GetPotentialsDamageOfDirectAttack(_possessedFighter.DirectAttackEffects.ToList(), target)
                    );
                    break;

                case EDamagePreference.MINIMIZE_COST:
                    preferedAbilityToAnimation = useableAbilitiesToAnimation.OrderBy(
                        abilityToAnimation => abilityToAnimation.activeAbility.ActionPointsCost
                    ).First();
                    useActiveAbility = preferedAbilityToAnimation.activeAbility.ActionPointsCost < _possessedFighter.DirectAttackActionPointsCost;
                    break;
            }

            // Do the action
            if (useActiveAbility && preferedAbilityToAnimation != null)
            {
                ;
                TargeterSO activeAbilityTargeter = preferedAbilityToAnimation.activeAbility.Targeter;
                _possessedFighter.UseActiveAbility(
                    preferedAbilityToAnimation,
                    _possessedFighter.GetFirstTouchingCellSequence(activeAbilityTargeter, target, _fightGrid, _fighterTeams)
                );
            }
            else
            {
                TargeterSO directAttackTargeter = _possessedFighter.DirectAttackTargeter;
                _possessedFighter.UseDirectAttack(
                    _possessedFighter.GetFirstTouchingCellSequence(directAttackTargeter, target, _fightGrid, _fighterTeams)
                );
            }
            return NodeState.SUCCESS;
        }

        private int GetPotentialsDamageOfAbilities(ActiveAbilitySO activeAbility, Fighter target)
        {
            return activeAbility.Effects.Sum(
                effect => effect.GetPotentialEffectDamages(_possessedFighter, target, effect.Masterstrokable)
            );
        }

        private int GetPotentialsDamageOfDirectAttack(List<AEffectSO> effects, Fighter target)
        {
            return effects.Sum(
                effect => effect.GetPotentialEffectDamages(_possessedFighter, target, effect.Masterstrokable)
            );
        }
    }
}
