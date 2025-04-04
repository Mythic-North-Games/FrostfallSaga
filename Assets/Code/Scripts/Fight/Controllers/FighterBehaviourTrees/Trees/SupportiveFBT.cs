using System.Collections.Generic;
using FrostfallSaga.Utils.Trees.BehaviourTree;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Actions;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Checks;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Trees
{
    public class SupportiveFBT : FighterBehaviourTree
    {
        public SupportiveFBT(
            Fighter possessedFighter,
            FightHexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams
        ) : base(possessedFighter, fightGrid, fighterTeams)
        {
        }

        /// <summary>
        /// First, check if the fighter can heal the weakest ally. If so, heal it.
        /// If not, check if the fighter can move. If so, move to the defined target.
        /// Second, check if the fighter can heal himself. If so, heal it.
        /// Third, check if the fighter can damage the weakest enemy around. If so, damage it.
        /// If not, check if the fighter can move. If so, move to the weakest enemy.
        /// If not, end the turn.
        /// </summary>
        protected override Node SetupTree()
        {
            return new Selector(
                new List<Node>
                {
                    BuildHealWeakestAllySequence(),
                    BuildHealSelfAllySequence(),
                    BuildDamageSequence(),
                    BuildMoveSequence(),
                    new EndTurnAction(_possessedFighter, _fightGrid, _fighterTeams)
                }
            );
        }

        private Sequence BuildHealWeakestAllySequence()
        {
            return new Sequence(
                new List<Node>
                {
                    new CanUseAbilityWithEffectCheck<HealEffect>(
                        _possessedFighter,
                        _fightGrid,
                        _fighterTeams
                    ),
                    new GetHealableTargetCheck(
                        _possessedFighter,
                        _fightGrid,
                        _fighterTeams,
                        new() { ETarget.ALLIES },
                        ETargetType.WEAKEST
                    ),
                    new Sequence(
                        new List<Node>
                        {
                            new IsDefinedTargetCloseForEffectCheck<HealEffect>(
                                _possessedFighter,
                                _fightGrid,
                                _fighterTeams
                            ),
                            new HealDefinedTargetAction(
                                _possessedFighter,
                                _fightGrid,
                                _fighterTeams,
                                EAbilityUsagePreference.MAXIMIZE_EFFECT
                            )
                        }
                    )
                }
            );
        }

        private Sequence BuildHealSelfAllySequence()
        {
            return new Sequence(
                new List<Node>
                {
                    new CanUseAbilityWithEffectCheck<HealEffect>(
                        _possessedFighter,
                        _fightGrid,
                        _fighterTeams
                    ),
                    new GetHealableTargetCheck(
                        _possessedFighter,
                        _fightGrid,
                        _fighterTeams,
                        new() { ETarget.SELF },
                        ETargetType.RANDOM  // Irrelevant because there is only one target
                    ),
                    new HealDefinedTargetAction(
                        _possessedFighter,
                        _fightGrid,
                        _fighterTeams,
                        EAbilityUsagePreference.MAXIMIZE_EFFECT
                    )
                }
            );
        }

        /// <summary>
        /// First, check if one of the given targets can be damaged.
        /// If there is one, damage it.
        /// </summary>
        private Sequence BuildDamageSequence()
        {
            return new Sequence(
                new List<Node>
                {
                    new CanDamageTargetCheck(
                        _possessedFighter,
                        _fightGrid,
                        _fighterTeams,
                        new() { ETarget.OPONNENTS },
                        ETargetType.WEAKEST
                    ),
                    new DamageDefinedTargetAction(
                        _possessedFighter,
                        _fightGrid,
                        _fighterTeams,
                        EAbilityUsagePreference.MAXIMIZE_EFFECT
                    )
                }
            );
        }

        /// <summary>
        /// First, check if fighter can move.
        /// If he can, move towards the closest target.
        /// </summary>
        private Sequence BuildMoveSequence()
        {
            return new Sequence(
                new List<Node>
                {
                    new CanMoveCheck(_possessedFighter, _fightGrid, _fighterTeams),
                    new MoveToClosestTargetAction(
                        _possessedFighter,
                        _fightGrid,
                        _fighterTeams,
                        new() { ETarget.OPONNENTS }
                    )
                }
            );
        }
    }
}