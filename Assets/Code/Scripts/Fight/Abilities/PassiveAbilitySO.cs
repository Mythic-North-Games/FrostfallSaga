using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightConditions;

namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    /// Represents a passive ability that can be applied from a fighter to possibly all the fighters during a fight.
    /// You set a list of conditions. If all the conditions are met, the passive ability is applied to the given list of targets.
    /// If no conditions are set, the passive ability will be applied to the targets automatically when the fight begins.
    /// </summary>
    [CreateAssetMenu(fileName = "PassiveAbility", menuName = "ScriptableObjects/Fight/Abilities/PassiveAbility", order = 0)]
    public class PassiveAbilitySO : BaseAbilitySO
    {
        [
            SerializeReference,
            Header("Passive ability specific configuration"),
            Tooltip("The conditions that need to be met for the passive ability to be applied.")
        ]
        public AFighterCondition[] ActivationConditions;

        [field: SerializeField, Tooltip("To whom the effects should be applied.")] public ETarget[] Targets { get; private set; }

        [
            field: SerializeField,
            Tooltip("The effects will be applied at every round if conditions are met.")
        ]
        public bool IsRecurring { get; private set; }

        [
            field: SerializeField,
            Tooltip("The effects will last for the entire fight if conditions are met once.")
        ]
        public bool LastsForFight { get; private set; }

        public FighterBuffVisualsController VisualsController;

        /// <summary>
        /// Applies the passive ability to the given fighter. Effects application, visuals, sounds and events are handled here.
        /// </summary>
        /// <param name="initator">The fighter that will receive the passive ability.</param>
        public void Apply(Fighter initator, Dictionary<Fighter, bool> fighterTeams)
        {
            if (Targets.Length == 0)
            {
                Debug.LogError($"No targets are set for the passive ability {Name}. The passive ability can't be applied.");
                return;
            }
            if (Effects.Length == 0)
            {
                Debug.LogError($"No effects are set for the passive ability {Name}. The passive ability can't be applied.");
                return;
            }

            Fighter[] targets = GetTargets(initator, fighterTeams);
            foreach (Fighter target in targets)
            {
                Effects.ToList().ForEach(
                    effect => effect.ApplyEffect(
                        receiver: target,
                        isMasterstroke: false,
                        initator: initator,
                        adjustGodFavorsPoints: false
                    )
                );
            }

            if (VisualsController == null)
            {
                Debug.LogWarning("VisualsController is not set. No visuals will be shown for the passive ability.");
                return;
            }

            VisualsController.ShowApplicationVisuals(initator);
            if (!VisualsController.IsShowingRecurringVisuals)
            {
                VisualsController.ShowRecurringVisuals(initator);
            }

            initator.onPassiveAbilityApplied?.Invoke(initator, this);
        }

        public void Remove(Fighter initator, Dictionary<Fighter, bool> fighterTeams)
        {
            Fighter[] targets = GetTargets(initator, fighterTeams);
            foreach (Fighter target in targets)
            {
                Effects.ToList().ForEach(effect => effect.RestoreEffect(target));
            }

            if (VisualsController == null)
            {
                return;
            }

            VisualsController.HideRecurringVisuals();

            initator.onPassiveAbilityRemoved?.Invoke(initator, this);
        }

        public bool CheckConditions(Fighter fighter, HexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams)
        {
            return (
                ActivationConditions.Length == 0 ||
                ActivationConditions.All(condition => condition.CheckCondition(fighter, fightGrid, fighterTeams))
            );
        }

        private Fighter[] GetTargets(Fighter initator, Dictionary<Fighter, bool> fighterTeams)
        {
            List<Fighter> targets = new();

            foreach (ETarget target in Targets)
            {
                switch (target)
                {
                    case ETarget.SELF:
                        targets.Add(initator);
                        break;
                    case ETarget.ALLIES:
                        targets.AddRange(
                            fighterTeams
                            .Where(fighterTeam => fighterTeam.Value == fighterTeams[initator])
                            .Select(pair => pair.Key)
                        );
                        break;
                    case ETarget.OPONENTS:
                        targets.AddRange(
                            fighterTeams
                                .Where(fighterTeam => fighterTeam.Value != fighterTeams[initator])
                                .Select(pair => pair.Key)
                        );
                        break;
                    default:
                        break;
                }
            }

            return targets.ToArray();
        }
    }
}
