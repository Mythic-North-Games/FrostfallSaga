using System;
using UnityEngine;
using System.Linq;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;
using FrostfallSaga.Fight.Targeters;
using System.Collections.Generic;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;
using FrostfallSaga.Grid.Cells;
namespace FrostfallSaga.Fight.FightCells.Impediments
{
    /// <summary>
    /// Represents a trap that can be placed on a fight cell.
    /// Traps apply a list of effects when a fighter steps on them at the specified trigger times.
    /// Traps effects can't be dodged or masterstroked.
    /// </summary>
    [CreateAssetMenu(fileName = "Trap", menuName = "ScriptableObjects/Fight/Impediments/Trap", order = 0)]
    public class TrapSO : AImpedimentSO
    {
        [field: SerializeField] public Targeter Targeter { get; private set; }
        [field: SerializeField] public ImpedimentAnimationSO Animation { get; private set; }
        [field: SerializeField, Header("Trap definition")] public ETrapTriggerTime[] TriggerTimes { get; private set; }
        [SerializeReference] public AEffect[] OtherCellsEffects = { };
        [SerializeReference] public AEffect[] OnCellEffects = { };
        [SerializeReference] public AFightCellAlteration[] CellAlterations = { };
        [SerializeField] public ETrapType TrapType  { get; private set; }

        public Action onTrapTriggered;
        [field: SerializeField] public GameObject TrapIsVisiblePrefab { get; private set; }

        public void
         Trigger(Fighter receiver, FightCell targetedCell)
        {
            FightCell[] trapTargetCells = Targeter.GetCellsFromSequence(targetedCell.ParentGrid, targetedCell, targetedCell);

            foreach (AEffect effect in OnCellEffects)
            {
                effect.ApplyEffect(
                targetedCell.GetFighter(),
                isMasterstroke: false,
                initiator: null,
                adjustGodFavorsPoints: true
    );
            }
            foreach (AEffect effect in OtherCellsEffects)
            {
                trapTargetCells.ToList()
                  .Where(cell => cell.HasFighter()).ToList()
                  .ForEach(cell =>
                      {
                          if (cell.HasFighter())
                          {
                              effect.ApplyEffect(
                                cell.GetFighter(),
                                isMasterstroke: false,
                                initiator: null,
                                adjustGodFavorsPoints: true
                    );
                          }
                          ;
                      }
                  );
            }
            foreach (AFightCellAlteration cellAteration in CellAlterations)
            {
                targetedCell.AlterationsManager.AddNewAlteration(cellAteration);
            }
            TriggerAnimation(targetedCell, receiver);
            onTrapTriggered?.Invoke();
        }

        public void TriggerAnimation(FightCell targetedCell, Fighter fighter)
        {
            FightCell[] AnimationTargetCells = new[] { targetedCell };

            if (Animation == null)
            {
                Debug.LogError($"No animation attached to the trap {Name}.");
            }
            else
            {
                Animation.Execute(fighter, AnimationTargetCells);
            }
        }

    }
}