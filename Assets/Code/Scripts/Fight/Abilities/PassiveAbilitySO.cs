using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.FightConditions;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    ///     Represents a passive ability that can be applied from a fighter to possibly all the fighters during a fight.
    ///     You set a list of conditions. If all the conditions are met, the passive ability is applied to the given list of
    ///     targets.
    ///     If no conditions are set, the passive ability will be applied to the targets automatically when the fight begins.
    /// </summary>
    [CreateAssetMenu(fileName = "PassiveAbility", menuName = "ScriptableObjects/Fight/Abilities/PassiveAbility",
        order = 0)]
    public class PassiveAbilitySO : APassiveAbility
    {
        [SerializeReference] public AEffect[] effects;

        [SerializeReference]
        [Header("Passive ability specific configuration")]
        [Tooltip("The conditions that need to be met for the passive ability to be applied.")]
        public AFighterCondition[] activationConditions;

        [field: SerializeField]
        [field: Tooltip("To whom the effects should be applied.")]
        public ETarget[] Targets { get; private set; }

        [field: SerializeField]
        [field: Tooltip("The effects will be applied at every round if conditions are met.")]
        public bool IsRecurring { get; private set; }

        [field: SerializeField]
        [field: Tooltip("The effects will last for the entire fight if conditions are met once.")]
        public bool LastsForFight { get; private set; }

        public FighterBuffVisualsController visualsController;

        /// <summary>
        ///     Applies the passive ability to the given fighter. Effects application, visuals, sounds and events are handled here.
        /// </summary>
        /// <param name="initiator">The fighter that will receive the passive ability.</param>
        public void Apply(Fighter initiator, Dictionary<Fighter, bool> fighterTeams)
        {
            if (Targets.Length == 0)
            {
                Debug.LogError(
                    $"No targets are set for the passive ability {Name}. The passive ability can't be applied.");
                return;
            }

            if (effects.Length == 0)
            {
                Debug.LogError(
                    $"No effects are set for the passive ability {Name}. The passive ability can't be applied.");
                return;
            }

            Fighter[] targets = GetTargets(initiator, fighterTeams);
            foreach (Fighter target in targets)
                effects.ToList().ForEach(
                    effect => effect.ApplyEffect(
                        target,
                        false,
                        initiator,
                        false
                    )
                );

            if (visualsController == null)
            {
                Debug.LogWarning("VisualsController is not set. No visuals will be shown for the passive ability.");
                return;
            }

            visualsController.ShowApplicationVisuals(initiator);
            if (!visualsController.IsShowingRecurringVisuals) visualsController.ShowRecurringVisuals(initiator);

            initiator.onPassiveAbilityApplied?.Invoke(initiator, this);
        }

        public void Remove(Fighter initiator, Dictionary<Fighter, bool> fighterTeams)
        {
            Fighter[] targets = GetTargets(initiator, fighterTeams);
            foreach (Fighter target in targets) effects.ToList().ForEach(effect => effect.RestoreEffect(target));

            if (visualsController == null) return;

            visualsController.HideRecurringVisuals();

            initiator.onPassiveAbilityRemoved?.Invoke(initiator, this);
        }

        public bool CheckConditions(Fighter fighter, FightHexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams)
        {
            return activationConditions.Length == 0 ||
                   activationConditions.All(condition => condition.CheckCondition(fighter, fightGrid, fighterTeams));
        }

        private Fighter[] GetTargets(Fighter initiator, Dictionary<Fighter, bool> fighterTeams)
        {
            List<Fighter> targets = new();

            foreach (ETarget target in Targets)
                switch (target)
                {
                    case ETarget.SELF:
                        targets.Add(initiator);
                        break;
                    case ETarget.ALLIES:
                        targets.AddRange(
                            fighterTeams
                                .Where(fighterTeam => fighterTeam.Value == fighterTeams[initiator])
                                .Select(pair => pair.Key)
                        );
                        break;
                    case ETarget.OPONNENTS:
                        targets.AddRange(
                            fighterTeams
                                .Where(fighterTeam => fighterTeam.Value != fighterTeams[initiator])
                                .Select(pair => pair.Key)
                        );
                        break;
                }

            return targets.ToArray();
        }

        #region For the UI
        public override Dictionary<Sprite, string> GetStatsUIData()
        {
            throw new System.NotImplementedException();
        }

        public override List<string> GetPrimaryEffectsUIData()
        {
            throw new System.NotImplementedException();
        }

        public override List<string> GetSecondaryEffectsUIData()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}