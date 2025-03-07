using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Trees.BehaviourTree;

namespace FrostfallSaga.Fight.Assets.Code.Scripts.Fight.Controllers.FighterBehaviourTrees.Checks
{
    public class CanHealTargetCheck : FBTNode
    {
        public static string TARGET_SHARED_DATA_KEY = "healTarget";
        private readonly List<ETarget> _possibleTargets;
        private readonly ETargetType _targetType;

        public CanHealTargetCheck(
            Fighter possessedFighter,
            AHexGrid fightGrid,
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
            if (healableTargets.Count == 0) return NodeState.FAILURE;
            SetSharedData(TARGET_SHARED_DATA_KEY, GetPreferredTarget(healableTargets));
            return NodeState.SUCCESS;
        }

        private List<Fighter> GetHealableTargets()
        {
            List<Fighter> healableTargets = new();
            bool _posssessedFighterTeam = _fighterTeams[_possessedFighter];

            foreach (Fighter fighter in _fighterTeams.Keys.Where(fighter => fighter.GetHealth() > 0))
            {
                if (fighter == _possessedFighter && !_possibleTargets.Contains(ETarget.SELF)) continue;

                bool fighterIsAlly = _fighterTeams[fighter] == _posssessedFighterTeam;

                if (fighterIsAlly && _possibleTargets.Contains(ETarget.ALLIES))
                    if (fighter.GetHealth() < fighter.GetMaxHealth())
                        if (CanHealFighter(fighter))
                            healableTargets.Add(fighter);
            }

            return healableTargets;
        }

        private bool CanHealFighter(Fighter fighter)
        {
            ListOfTypes<AEffect> effects = new();
            effects.Add<HealEffect>();
            return _possessedFighter.CanUseAtLeastOneActiveAbility(_fightGrid, _fighterTeams, fighter,
                effects); // Change for healing and not activeabilty
        }

        private Fighter GetPreferredTarget(List<Fighter> healableTargets)
        {
            return _targetType switch
            {
                ETargetType.RANDOM => Randomizer.GetRandomElementFromArray(healableTargets.ToArray()),
                ETargetType.WEAKEST => healableTargets.OrderBy(fighter => fighter.GetHealth()).First(),
                ETargetType.STRONGEST => healableTargets.OrderByDescending(fighter => fighter.GetHealth()).First(),
                ETargetType.CLOSEST => healableTargets.OrderBy(
                    fighter => CellsPathFinding.GetShorterPath(
                        _fightGrid,
                        _possessedFighter.cell,
                        fighter.cell,
                        true,
                        true,
                        true
                    ).Length
                ).First(),
                _ => null
            };
        }
    }
}