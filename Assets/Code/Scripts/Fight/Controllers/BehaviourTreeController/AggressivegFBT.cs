using System.Collections.Generic;
using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Fight.Controllers.BehaviourTreeController.Actions;
using FrostfallSaga.Fight.Controllers.BehaviourTreeController.Checks;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController
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

        protected override Node SetupTree()
        {
            return new Selector(
                new List<Node>
                {
                    new Sequence(
                        new List<Node>
                        {
                            new CanDamageTargetCheck(_possessedFighter, _fightGrid, _fighterTeams, new List<ETarget> {ETarget.OPONENTS}, ETargetType.STRONGEST),
                            new DamageTargetAction(_possessedFighter, _fightGrid, _fighterTeams, EDamagePreference.MAXIMIZE_DAMAGE)
                        }
                    ),
                    new Sequence(
                        new List<Node>
                        {
                            new CanMoveCheck(_possessedFighter, _fightGrid, _fighterTeams),
                            new MoveToClosestTargetAction(_possessedFighter, _fightGrid, _fighterTeams, new List<ETarget> {ETarget.OPONENTS})
                        }
                    ),
                    new EndTurnAction(_possessedFighter, _fightGrid, _fighterTeams)
                }
            );
        }
    }
}