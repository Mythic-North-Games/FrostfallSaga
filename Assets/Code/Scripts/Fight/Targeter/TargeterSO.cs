using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Fight.Targeters
{
    /// <summary>
    /// Defines a targeter. Will be used to select cells for abilities and weapons.
    /// </summary>
    [CreateAssetMenu(fileName = "Targeter", menuName = "ScriptableObjects/Fight/Targeter", order = 0)]
    public class TargeterSO : ScriptableObject
    {
        [field: SerializeField] public int OriginCellRange { get; private set; }
        [field: SerializeField] public Vector2Int[] CellsSequence { get; private set; }
        [field: SerializeField] public bool FighterMandatory { get; private set; }

        private static readonly System.Random _randomizer = new();

        /// <summary>
        /// Extract the cells corresponding to the targeter from the given context.
        /// </summary>
        /// <param name="fightGrid">The fight grid that the targeter will extract the cells from.</param>
        /// <param name="originCell">The starting cell of the sequence.</param>
        /// <param name="initiatorCell">The cell of the targeter's initiator.</param>
        /// <returns>The extracted cells in order if resolvable.</returns>
        /// <exception cref="TargeterUnresolvableException">If one of the targeter's condition is not respected.</exception>
        public Cell[] Resolve(HexGrid fightGrid, Cell originCell, Cell initiatorCell)
        {
            if (CellsPathFinding.GetShorterPath(fightGrid, initiatorCell, originCell).Length > OriginCellRange)
            {
                throw new TargeterUnresolvableException("Initiator not in range");
            }

            Cell[] targetedCells = GetCellsFromSequence(fightGrid, originCell);

            if (FighterMandatory && targetedCells.All(cell => cell.GetComponentInChildren<CellFightBehaviour>().Fighter == null))
            {
                throw new TargeterUnresolvableException("A fighter is mandatory to resolve the targeter");
            }

            return targetedCells;
        }

        /// <summary>
        /// Get one random resolved targeter cell sequence for the given context if it does exist.
        /// </summary>
        /// <param name="fightGrid">The current fight grid.</param>
        /// <param name="initiatorCell">The cell where the targeter's initiator is located.</param>
        /// <returns>One random resolved targeter cell sequence for the given context if it does exist.</returns>
        /// <exception cref="TargeterUnresolvableException">If the targeter can't be resolved around the initiator.</exception>
        public Cell[] GetRandomTargetCells(HexGrid fightGrid, Cell initiatorCell)
        {
            List<Cell[]> resolvedTargeterSequences = new();
            foreach (Cell cellThatCanBeTargeted in GetAllCellsAvailableForTargeting(fightGrid, initiatorCell))
            {
                try
                {
                    resolvedTargeterSequences.Add(Resolve(fightGrid, cellThatCanBeTargeted, initiatorCell));
                }
                catch (TargeterUnresolvableException)
                {
                    continue;
                }
            }

            if (resolvedTargeterSequences.Count == 0)
            {
                throw new TargeterUnresolvableException("Targeter unresolvable around initiator.");
            }

            return resolvedTargeterSequences[_randomizer.Next(0, resolvedTargeterSequences.Count)];
        }

        /// <summary>
        /// Extract the cells corresponding the targeter's sequence from the given context.
        /// </summary>
        /// <param name="fightGrid">The fight grid that the targeter will extract the cells from.</param>
        /// <param name="originCell">The starting cell of the sequence.</param>
        /// <returns>The extracted cells in order, targeter's conditions are ignored.</returns>
        public Cell[] GetCellsFromSequence(HexGrid fightGrid, Cell originCell)
        {
            List<Cell> targetedCells = new()
            {
                originCell
            };

            for (int i = 1; i < CellsSequence.Length; i++)
            {
                Vector2Int nextCellCoordinates = targetedCells.First().Coordinates + CellsSequence[i];
                if (fightGrid.CellsByCoordinates.TryGetValue(nextCellCoordinates, out Cell nextCell))
                {
                    targetedCells.Add(nextCell);
                }
            }

            return targetedCells.ToArray();
        }

        /// <summary>
        /// Returns all the cells that are in range depending on the initiator position and the targeter.
        /// </summary>
        /// <param name="fightGrid">The fight grid to get the cells from.</param>
        /// <param name="initiatorCell">The cell the initiator occupies.</param>
        /// <returns>All the cells that are in range depending on the initiator position and the targeter</returns>
        public Cell[] GetAllCellsAvailableForTargeting(HexGrid fightGrid, Cell initiatorCell)
        {
            List<Cell> availableCells = new();
            foreach (Cell cell in fightGrid.GetCells().Where(cell => cell != initiatorCell))
            {
                if (CellsPathFinding.GetShorterPath(fightGrid, initiatorCell, cell).Length <= OriginCellRange)
                {
                    availableCells.Add(cell);
                }
            }
            return availableCells.ToArray();
        }

        /// <summary>
        /// Returns whether the given target cell is in the targeter range depending on the context.
        /// </summary>
        /// <param name="fightGrid">The grid where the initiator is located.</param>
        /// <param name="initiatorCell">The targeter initiator's cell.</param>
        /// <param name="targetCell">The cell you want to know if it's in range.</param>
        /// <returns>True if the target cell is in range, false otherwise.</returns>
        public bool IsCellInRange(HexGrid fightGrid, Cell initiatorCell, Cell targetCell)
        {
            return GetAllCellsAvailableForTargeting(fightGrid, initiatorCell).Contains(targetCell);
        }

        /// <summary>
        /// Returns whether the targeter resolves at least for one cell in the available cells around the given initiator cell.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the initiator is located.</param>
        /// <param name="initiatorCell">The targeter initiator's cell.</param>
        /// <returns>True if the targeter resolves at least for one cell in the available cells around the given initiator cell, false otherwise.</returns>
        public bool AtLeastOneCellResolvable(HexGrid fightGrid, Cell initiatorCell)
        {
            foreach (Cell cellThatCanBeTargeted in GetAllCellsAvailableForTargeting(fightGrid, initiatorCell))
            {
                try
                {
                    if (Resolve(fightGrid, cellThatCanBeTargeted, initiatorCell).Length > 0)
                    {
                        return true;
                    }
                }
                catch (TargeterUnresolvableException)
                {
                    continue;
                }
            }
            return false;
        }
    }
}