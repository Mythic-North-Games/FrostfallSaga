using System;
using System.Collections.Generic;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;
using System.Linq;

namespace FrostfallSaga.Fight.Controllers
{
    public class RandomController : AFighterController
    {
        private static readonly Random _randomizer = new();
        public int maxActionsCount = 3;

        public override void PlayTurn(Fighter fighterToPlay, Dictionary<Fighter, string> fighterTeams, HexGrid fightGrid)
        {
            int numberOfActionsToDo = _randomizer.Next(1, maxActionsCount);
            int numberOfActionsDone = 0;

            // while (numberOfActionsDone < numberOfActionsToDo && fighterToPlay.CanStillAct())
            // {
            //    switch (GetRandomDoableAction(fighterToPlay, fightGrid))
            //    {
            //         case FighterAction.MOVE:
            //            break;
            //        case FighterAction.DIRECT_ATTACK:
            //            break;
            //        case FighterAction.ACTIVE_ABILITY:
            //            break;
            //        default:
            //            break;
            //    }
            // }

            // OnFighterTurnEnded?.Invoke(fighterToPlay);
        }

        // private FighterAction GetRandomDoableAction(Fighter fighterThatWillAct, HexGrid fightGrid)
        // {
        //     List<FighterAction> doableActions = new();
        //     if (fighterThatWillAct.GetMovePoints() > 0 && FightCellNeighbors.GetNeighbors(fightGrid, fighterThatWillAct.cell).Length > 0)
        //     {
        //         doableActions.Add(FighterAction.MOVE);
        //     }

        //     if (fighterThatWillAct.FighterConfiguration.DirectAttackActionPointsCost <= fighterThatWillAct.GetActionPoints())
        //     {
        //         doableActions.Add(FighterAction.DIRECT_ATTACK);
        //     }

        //     if (
        //         fighterThatWillAct.FighterConfiguration.AvailableActiveAbilities.First(
        //             activeAbilityToAnimation => activeAbilityToAnimation.activeAbility.ActionPointsCost <= fighterThatWillAct.GetActionPoints()
        //         ) != null
        //     )
        //     {
        //         doableActions.Add(FighterAction.ACTIVE_ABILITY);
        //     }

        //     return doableActions[_randomizer.Next(0, doableActions.Count)];
        // }

        private enum FighterAction
        {
            MOVE = 0,
            DIRECT_ATTACK = 1,
            ACTIVE_ABILITY = 2,
        }
    }
}