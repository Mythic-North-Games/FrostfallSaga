using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Core;
using FrostfallSaga.Grid;
using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Checks;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Actions
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
            Fighter target = (Fighter)GetSharedData(CanDamageTargetCheck.TARGET_SHARED_DATA_KEY);
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
                    float directAttackChance = 1f / (useableAbilitiesToAnimation.Count + 1);
                    useActiveAbility = !Randomizer.GetBooleanOnChance(directAttackChance) || !canUseDirectAttack;
                    break;

                case EDamagePreference.MAXIMIZE_DAMAGE:
                    preferedAbilityToAnimation = useableAbilitiesToAnimation.OrderByDescending(
                        abilityToAnimation => abilityToAnimation.activeAbility.GetDamagesPotential(_possessedFighter, target)
                    ).First();
                    useActiveAbility = (
                        preferedAbilityToAnimation.activeAbility.GetDamagesPotential(_possessedFighter, target) >
                        GetPotentialsDamageOfDirectAttack(_possessedFighter.DirectAttackEffects.ToList(), target)
                    ) || !canUseDirectAttack;
                    break;

                case EDamagePreference.MINIMIZE_COST:
                    preferedAbilityToAnimation = useableAbilitiesToAnimation.OrderBy(
                        abilityToAnimation => abilityToAnimation.activeAbility.ActionPointsCost
                    ).First();
                    useActiveAbility = (
                        preferedAbilityToAnimation.activeAbility.ActionPointsCost < _possessedFighter.DirectAttackActionPointsCost
                    ) || !canUseDirectAttack;
                    break;
            }

            // Get damage action target cells
            TargeterSO damageActionTargeter = useActiveAbility ?
                preferedAbilityToAnimation.activeAbility.Targeter :
                _possessedFighter.DirectAttackTargeter;
            Cell[] targetCells = _possessedFighter.GetFirstTouchingCellSequence(damageActionTargeter, target, _fightGrid, _fighterTeams);

            // Check if the target cells are valid (should not occure)
            if (targetCells == null)
            {
                throw new MalformedFBTException(
                    "Unable to find a valid target cell sequence. Should not occure. Please, verify the previous target check is valid."
                );
            }

            // Do the action
            if (useActiveAbility)
            {
                _possessedFighter.onFighterActiveAbilityEnded += OnPossessedFighterFinishedDamageAction;
                _possessedFighter.UseActiveAbility(
                    preferedAbilityToAnimation,
                    targetCells
                );
            }
            else
            {
                _possessedFighter.onFighterDirectAttackEnded += OnPossessedFighterFinishedDamageAction;
                _possessedFighter.UseDirectAttack(targetCells);
            }
            SetSharedData(FBTNode.ACTION_RUNNING_SHARED_DATA_KEY, true);
            return NodeState.SUCCESS;
        }

        private int GetPotentialsDamageOfDirectAttack(List<AEffectSO> effects, Fighter target)
        {
            return effects.Sum(
                effect => effect.GetPotentialEffectDamages(_possessedFighter, target, effect.Masterstrokable)
            );
        }

        private void OnPossessedFighterFinishedDamageAction(Fighter possessedFighter)
        {
            SetSharedData(FBTNode.ACTION_RUNNING_SHARED_DATA_KEY, false);
            possessedFighter.onFighterActiveAbilityEnded -= OnPossessedFighterFinishedDamageAction;
            possessedFighter.onFighterDirectAttackEnded -= OnPossessedFighterFinishedDamageAction;
        }
    }
}
