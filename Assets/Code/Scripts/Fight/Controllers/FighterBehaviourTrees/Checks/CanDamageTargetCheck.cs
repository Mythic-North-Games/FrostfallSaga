using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.Trees.BehaviourTree;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Checks
{
    /// <summary>
    ///     Check if the possessed fighter can damage any target between the possible targets and types given in parameters.
    /// </summary>
    public class CanDamageTargetCheck : FBTNode
    {
        private readonly List<ETarget> _possibleTargets;
        private readonly ETargetType _targetType;

        public CanDamageTargetCheck(
            Fighter possessedFighter,
            FightHexGrid fightGrid,
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
            List<Fighter> damagableTargets = GetDamagableFighters();
            if (damagableTargets.Count == 0) return NodeState.FAILURE;

            Fighter preferredTarget = _targetType.GetPreferedTargetInList(_possessedFighter, _fightGrid, damagableTargets);
            SetSharedData(TARGET_SHARED_DATA_KEY, preferredTarget);
            return NodeState.SUCCESS;
        }

        private List<Fighter> GetDamagableFighters()
        {
            List<Fighter> damagableTargets = new();
            bool _possessedFighterTeam = _fighterTeams[_possessedFighter];

            foreach (Fighter fighter in _fighterTeams.Keys.Where(fighter => fighter.GetHealth() > 0))
            {
                if (fighter == _possessedFighter && !_possibleTargets.Contains(ETarget.SELF)) continue;

                bool fighterIsAlly = _fighterTeams[fighter] == _possessedFighterTeam;
                if (fighterIsAlly && !_possibleTargets.Contains(ETarget.ALLIES)) continue;

                if (!fighterIsAlly && !_possibleTargets.Contains(ETarget.OPONNENTS)) continue;

                if (!CanDamageFighter(fighter)) continue;
                damagableTargets.Add(fighter);
            }

            return damagableTargets;
        }

        private bool CanDamageFighter(Fighter fighter)
        {
            return _possessedFighter.CanDirectAttack(_fightGrid, _fighterTeams, fighter) ||
                   _possessedFighter.CanUseAtLeastOneActiveAbility(_fightGrid, _fighterTeams, fighter);
        }
    }
}