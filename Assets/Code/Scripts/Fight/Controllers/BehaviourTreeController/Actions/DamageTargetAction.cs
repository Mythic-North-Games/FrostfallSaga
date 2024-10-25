using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController.Actions
{
    public class DamageTargetAction : FBTNode
    {
        public DamageTargetAction(Fighter possessedFighter, HexGrid fightGrid) : base(Fighter possessedFighter, HexGrid fightGrid)
        { 

        }
    }
}
