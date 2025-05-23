using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.DataStructures.BehaviourTree;
using UnityEngine;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Actions
{
    public class HealDefinedTargetAction : FBTNode
    {
        private readonly EAbilityUsagePreference abilityUsagePreference;

        public HealDefinedTargetAction(
            Fighter possessedFighter,
            FightHexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams,
            EAbilityUsagePreference abilityUsagePreference
        ) : base(possessedFighter, fightGrid, fighterTeams)
        {
            this.abilityUsagePreference = abilityUsagePreference;
        }

        public override NodeState Evaluate()
        {
            Fighter target = (Fighter)GetSharedData(FBTNode.TARGET_SHARED_DATA_KEY);

            if (target == null)
            {
                Debug.LogError("A target should be defined before using the HealDefinedTargetAction.");
                return NodeState.FAILURE;
            }

            // Filter the active abilities that can be used on the target
            List<ActiveAbilitySO> useableAbilities = _possessedFighter.ActiveAbilities.Where(
                activeAbility => _possessedFighter.CanUseActiveAbility(
                    _fightGrid, activeAbility, _fighterTeams, target
                )
            ).ToList();

            // Get the preferred active ability based on the ability usage preference
            ActiveAbilitySO preferedAbility = GetPreferredActiveAbility(useableAbilities, target);


            // Get heal action target cells
            FightCell[] targetCells = _possessedFighter.GetFirstTouchingCellSequence(
                preferedAbility.Targeter, target, _fightGrid, _fighterTeams
            );

            // Check if the target cells are valid (should not occure)
            if (targetCells == null)
            {
                throw new MalformedFBTException(
                    "Unable to find a valid target cell sequence. Should not occure. Please, verify the previous target check is valid."
                );
            }

            // INFO: We are setting the is running before executing the action because if no animation is played, the action will be finished instantly and the variable will still be true so infinite loop will occure.
            SetSharedData(FBTNode.ACTION_RUNNING_SHARED_DATA_KEY, true);

            // Do the action
            _possessedFighter.onActiveAbilityEnded +=
                (fighter, usedAbility) => OnPossessedFighterFinishedHealing(fighter);
            _possessedFighter.UseActiveAbility(
                preferedAbility,
                targetCells
            );
            return NodeState.SUCCESS;
        }

        private ActiveAbilitySO GetPreferredActiveAbility(List<ActiveAbilitySO> useableAbilities, Fighter target)
        {
            switch (abilityUsagePreference)
            {
                case EAbilityUsagePreference.RANDOM:
                    return Randomizer.GetRandomElementFromArray(useableAbilities.ToArray());

                case EAbilityUsagePreference.MAXIMIZE_EFFECT:
                    return useableAbilities.OrderByDescending(
                        ability => ability.GetHealPotential(_possessedFighter, target)
                    ).FirstOrDefault();

                case EAbilityUsagePreference.MINIMIZE_COST:
                    return useableAbilities.OrderBy(ability => ability.ActionPointsCost).First();

                default:
                    Debug.LogError($"Unknown ability usage preference: {abilityUsagePreference}");
                    return null;
            }
        }

        private void OnPossessedFighterFinishedHealing(Fighter possessedFighter)
        {
            SetSharedData(FBTNode.ACTION_RUNNING_SHARED_DATA_KEY, false);
        }
    }
}