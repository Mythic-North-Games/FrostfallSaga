using System.Collections.Generic;
using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Actions;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Checks;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Trees
{
    public class AggressiveFBT : FighterBehaviourTree
    {
        public AggressiveFBT(
            Fighter possessedFighter,
            HexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams
        ) : base(possessedFighter, fightGrid, fighterTeams)
        {
        }

        /// <summary>
        /// First, check if the fighter can damage the strongest target. If so, damage it.
        /// If not, check if the fighter can move. If so, move to the closest target.
        /// If not, end the turn.
        /// </summary>
        protected override Node SetupTree()
        {
            List<ETarget> targets = new() { ETarget.OPONENTS };

            return new Selector(
                new List<Node>
                {
                    BuildDamageSequence(targets),
                    BuildMoveSequence(targets),
                    new EndTurnAction(_possessedFighter, _fightGrid, _fighterTeams)
                }
            );
        }

        /// <summary>
        /// First, check if one of the given targets can be damaged.
        /// If there is one, damage it.
        /// </summary>
        /// <param name="targets">The possible targets to try to damage</param>
        private Sequence BuildDamageSequence(List<ETarget> targets)
        {
            return new Sequence(
                new List<Node>
                {
                    new CanDamageTargetCheck(
                        _possessedFighter,
                        _fightGrid,
                        _fighterTeams,
                        targets,
                        ETargetType.STRONGEST
                    ),
                    new DamageTargetAction(
                        _possessedFighter,
                        _fightGrid,
                        _fighterTeams,
                        EDamagePreference.MAXIMIZE_DAMAGE
                    )
                }
            );
        }

        /// <summary>
        /// First, check if fighter can move.
        /// If he can, move towards the closest target.
        /// </summary>
        /// <param name="targets">The possible targets to try to move to.</param>
        private Sequence BuildMoveSequence(List<ETarget> targets)
        {
            return new Sequence(
                new List<Node>
                {
                    new CanMoveCheck(_possessedFighter, _fightGrid, _fighterTeams),
                    new MoveToClosestTargetAction(
                        _possessedFighter,
                        _fightGrid,
                        _fighterTeams,
                        targets
                    )
                }
            );
        }
    }
}