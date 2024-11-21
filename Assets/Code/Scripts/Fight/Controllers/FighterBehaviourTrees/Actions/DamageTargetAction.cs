using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Core;
using FrostfallSaga.Grid;
using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Checks;
using FrostfallSaga.Fight.Abilities;

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
            List<ActiveAbilitySO> useableAbilities = _possessedFighter.ActiveAbilities.Where(
                activeAbility => _possessedFighter.CanUseActiveAbility(
                    _fightGrid, activeAbility, _fighterTeams, target
                )
            ).ToList();

            ActiveAbilitySO preferedAbility = null;
            bool useActiveAbility = false;
            bool canUseDirectAttack = _possessedFighter.CanDirectAttack(_fightGrid, _fighterTeams, target);

            // Choose the ability to use based on the damage preference and decide if it's better to use the prefered ability or the direct attack
            switch (damagePreference)
            {
                case EDamagePreference.RANDOM:
                    preferedAbility = Randomizer.GetRandomElementFromArray(useableAbilities.ToArray());
                    float directAttackChance = 1f / (useableAbilities.Count + 1);
                    useActiveAbility = !Randomizer.GetBooleanOnChance(directAttackChance) || !canUseDirectAttack;
                    break;

                case EDamagePreference.MAXIMIZE_DAMAGE:
                    preferedAbility = useableAbilities.OrderByDescending(
                        ability => ability.GetDamagesPotential(_possessedFighter, target)
                    ).First();
                    useActiveAbility = (
                        preferedAbility.GetDamagesPotential(_possessedFighter, target) >
                        GetPotentialsDamageOfDirectAttack(_possessedFighter.DirectAttackEffects.ToList(), target)
                    ) || !canUseDirectAttack;
                    break;

                case EDamagePreference.MINIMIZE_COST:
                    preferedAbility = useableAbilities.OrderBy(ability => ability.ActionPointsCost).First();
                    useActiveAbility = preferedAbility.ActionPointsCost < _possessedFighter.DirectAttackActionPointsCost || !canUseDirectAttack;
                    break;
            }

            // Get damage action target cells
            Targeter damageActionTargeter = useActiveAbility ? preferedAbility.Targeter : _possessedFighter.DirectAttackTargeter;
            FightCell[] targetCells = _possessedFighter.GetFirstTouchingCellSequence(
                damageActionTargeter, target, _fightGrid, _fighterTeams
            );

            // Check if the target cells are valid (should not occure)
            if (targetCells == null)
            {
                throw new MalformedFBTException(
                    "Unable to find a valid target cell sequence. Should not occure. Please, verify the previous target check is valid."
                );
            }

            // INFO: We are setting it before executing the action because if no animation is played, the action will be finished instantly and the variable will still be true so infinite loop will occure.
            SetSharedData(FBTNode.ACTION_RUNNING_SHARED_DATA_KEY, true);

            // Do the action
            if (useActiveAbility)
            {
                _possessedFighter.onActiveAbilityEnded += OnPossessedFighterFinishedDamageAction;
                _possessedFighter.UseActiveAbility(
                    preferedAbility,
                    targetCells
                );
            }
            else
            {
                _possessedFighter.onDirectAttackEnded += OnPossessedFighterFinishedDamageAction;
                _possessedFighter.UseDirectAttack(targetCells);
            }
            return NodeState.SUCCESS;
        }

        private int GetPotentialsDamageOfDirectAttack(List<AEffect> effects, Fighter target)
        {
            return effects.Sum(
                effect => effect.GetPotentialEffectDamages(_possessedFighter, target, true)
            );
        }

        private void OnPossessedFighterFinishedDamageAction(Fighter possessedFighter)
        {
            SetSharedData(FBTNode.ACTION_RUNNING_SHARED_DATA_KEY, false);
            possessedFighter.onActiveAbilityEnded -= OnPossessedFighterFinishedDamageAction;
            possessedFighter.onDirectAttackEnded -= OnPossessedFighterFinishedDamageAction;
        }
    }
}
