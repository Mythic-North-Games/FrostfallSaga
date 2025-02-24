using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees;
using FrostfallSaga.Utils.Trees.BehaviourTree;

namespace FrostfallSaga.Fight.Assets.Code.Scripts.Fight.Controllers.FighterBehaviourTrees.Checks
{
    public class GetHealableTargetCheck : FBTNode
    {
        private readonly List<ETarget> _possibleTargets;
        private readonly ETargetType _targetType;

        public GetHealableTargetCheck(
            Fighter possessedFighter,
            HexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams,
            List<ETarget> possibleTargets,
            ETargetType targetType
            ) : base(possessedFighter, fightGrid, fighterTeams)
        {
            _possibleTargets = possibleTargets;
            _targetType = targetType;
        }

        public override NodeState Evaluate()
        {
            List<Fighter> healableTargets = GetHealableTargets();
            if (healableTargets.Count == 0)
            {
                return NodeState.FAILURE;
            }

            Fighter preferredTarget = _targetType.GetPreferedTargetInList(_possessedFighter, _fightGrid, healableTargets);
            SetSharedData(TARGET_SHARED_DATA_KEY, preferredTarget);
            return NodeState.SUCCESS;
        }

        private List<Fighter> GetHealableTargets()
        {
            List<Fighter> healableTargets = new();
            bool _posssessedFighterTeam = _fighterTeams[_possessedFighter];

            Fighter[] damagedFighters = _fighterTeams.Keys.Where(fighter => fighter.IsDamaged()).ToArray();
            foreach (Fighter fighter in damagedFighters)
            {
                // Check if self included in possible targets
                if (!_possibleTargets.Contains(ETarget.SELF) && fighter == _possessedFighter)
                {
                    continue;
                }

                bool fighterIsAlly = _fighterTeams[fighter] == _posssessedFighterTeam;

                // Check if allies included in possible targets
                if (!_possibleTargets.Contains(ETarget.ALLIES) && fighterIsAlly)
                {
                    continue;
                }

                // Check if enemies included in possible targets
                if (!_possibleTargets.Contains(ETarget.OPONNENTS) && !fighterIsAlly)
                {
                    continue;
                }

                healableTargets.Add(fighter);
            }

            return healableTargets;
        }
    }
}
