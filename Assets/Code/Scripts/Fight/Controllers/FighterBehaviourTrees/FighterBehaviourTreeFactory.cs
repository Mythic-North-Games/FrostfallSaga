using System.Collections.Generic;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Trees;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees
{
    public static class FighterBehaviourTreeFactory
    {
        /// <summary>
        /// Creates a new fighter behaviour tree based on the given ID.
        /// </summary>
        /// <param name="id">The ID of the behaviour tree to create.</param>
        /// <param name="fighter">The fighter to control.</param>
        /// <param name="fightGrid">The fight grid.</param>
        /// <param name="fighterTeams">All the fighters of the fight and their corresponding team.</param>
        /// <returns>The created behaviour tree.</returns>
        public static FighterBehaviourTree CreateBehaviourTree(EFighterBehaviourTreeID id, Fighter fighter, HexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams)
        {
            return id switch
            {
                EFighterBehaviourTreeID.Aggressive => new AggressiveFBT(fighter, fightGrid, fighterTeams),
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}