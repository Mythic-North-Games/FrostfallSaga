using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Targeters
{
    /// <summary>
    /// Defines a targeter. Will be used to select cells for abilities and weapons.
    /// </summary>
    [CreateAssetMenu(fileName = "Targeter", menuName = "ScriptableObjects/Fight/Targeter", order = 0)]
    public class TargeterSO : ScriptableObject
    {
        /// <summary>
        /// Targeter base definition
        /// </summary>
        [field: SerializeField, Header("Definition"), Tooltip("The available range from the intiator's cell available for targeting.")]
        public int OriginCellRange { get; private set; }

        [field: SerializeField, Tooltip("The coordinates of the cells to target (ex: line would be [0,0], [0,1], [0,2]).")]
        public Vector2Int[] CellsSequence { get; private set; }

        [field: SerializeField, Tooltip("If true, the cell sequence will rotate towards fighter direction. It will also affect the obstacles checks.")]
        public bool StartFromInitiator { get; private set; }


        /// <summary>
        /// Resolve conditions
        /// </summary>
        [field: SerializeField, Header("Resolve conditions"), Tooltip("If true, the targeter will only resolve if a fighter is present on one of the targeted cells.")]
        public bool FighterMandatory { get; private set; }

        [field: SerializeField, Tooltip("Can target all fighter types by default. Add options to restrain who can be targeted.")]
        public ETarget[] TargetsToExclude { get; private set; }

        [field: SerializeField, Tooltip("Can target all heights by default. Add options to restrain which heights can be targeted."), Range(-2, 2)]
        public int[] RelativeHeightsToExclude { get; private set; }


        /// <summary>
        /// Obstacles that stops the targeter
        /// </summary>
        [field: SerializeField, Header("Obstacles"), Tooltip("If true, the targeter will resolve cells until stoped by an inaccessible cell.")]
        public bool StopedAtInacessibles { get; private set; }

        [field: SerializeField, Tooltip("The targeter will resolve cells until stoped by a cell with at least X higher height difference. 0 means deactivated."), Range(0, 2)]
        public int StopedAtXthHigherHeightDifference { get; private set; }

        [field: SerializeField, Tooltip("The targeter will resolve cells until stoped by a cell with at least X lower height difference. 0 means deactivated."), Range(0, 2)]
        public int StopedAtXthLowerHeightDifference { get; private set; }

        [field: SerializeField, Tooltip("The targeter will resolve cells until stoped by the Xth fighter encountered."), Range(0, 6)]
        public int StopedAtXthEncounteredFighters { get; private set; }


        /// <summary>
        /// Extract the cells corresponding to the targeter from the given context.
        /// </summary>
        /// <param name="fightGrid">The fight grid that the targeter will extract the cells from.</param>
        /// <param name="initiatorCell">The cell of the targeter's initiator.</param>
        /// <param name="originCell">The starting cell of the sequence.</param>
        /// <param name="fightersTeams">The fighters and their teams (true if ally, false if enemy).</param>
        /// <returns>The extracted cells in order if resolvable.</returns>
        /// <exception cref="TargeterUnresolvableException">If one of the targeter's condition is not respected.</exception>
        public Cell[] Resolve(HexGrid fightGrid, Cell initiatorCell, Cell originCell, Dictionary<Fighter, bool> fightersTeams)
        {
            int distanceToInitiator = CellsPathFinding.GetShorterPath(
                fightGrid,
                initiatorCell,
                originCell,
                includeInaccessibleNeighbors: true,
                includeHeightInaccessibleNeighbors: true
            ).Length;
            if (distanceToInitiator > OriginCellRange)
            {
                throw new TargeterUnresolvableException("Initiator not in range");
            }

            Cell[] targetedCells = GetCellsFromSequence(fightGrid, initiatorCell, originCell);
            if (FighterMandatory)
            {
                CheckAtLeastOneFighterPresent(targetedCells);
            }
            CheckExcludedTargets(targetedCells, initiatorCell, fightersTeams);
            CheckExcludedRelativeHeights(targetedCells, initiatorCell, fightersTeams);
            return targetedCells;
        }

        /// <summary>
        /// Get one random resolved targeter cell sequence for the given context if it does exist.
        /// </summary>
        /// <param name="fightGrid">The current fight grid.</param>
        /// <param name="initiatorCell">The cell where the targeter's initiator is located.</param>
        /// <param name="fightersTeams">The fighters and their teams (true if ally, false if enemy).</param>
        /// <returns>One random resolved targeter cell sequence for the given context if it does exist.</returns>
        /// <exception cref="TargeterUnresolvableException">If the targeter can't be resolved around the initiator.</exception>
        public Cell[] GetRandomTargetCells(HexGrid fightGrid, Cell initiatorCell, Dictionary<Fighter, bool> fightersTeams)
        {
            List<Cell[]> resolvedTargeterSequences = new();
            foreach (Cell cellThatCanBeTargeted in GetAllCellsAvailableForTargeting(fightGrid, initiatorCell, fightersTeams))
            {
                try
                {
                    resolvedTargeterSequences.Add(Resolve(fightGrid, initiatorCell, cellThatCanBeTargeted, fightersTeams));
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

            return Randomizer.GetRandomElementFromArray(resolvedTargeterSequences.ToArray());
        }

        /// <summary>
        /// Extract the cells corresponding the targeter's sequence from the given context.
        /// </summary>
        /// <param name="fightGrid">The fight grid that the targeter will extract the cells from.</param>
        /// <param name="initiatorCell">The cell where the targeter's initiator is located.</param>
        /// <param name="originCell">The starting cell of the sequence.</param>
        /// <returns>The extracted cells in order, targeter's conditions are ignored.</returns>
        public Cell[] GetCellsFromSequence(HexGrid fightGrid, Cell initiatorCell, Cell originCell)
        {
            List<Cell> targetedCells = new()
            {
                originCell
            };
            Vector2Int direction = Cell.GetHexDirection(initiatorCell, originCell);

            for (int i = 1; i < CellsSequence.Length; i++)
            {
                if (StartFromInitiator)
                {
                    Vector2Int nextCellCoordinates = GetNextCellCoordinatesToDirection(
                        originCell,
                        CellsSequence[i],
                        direction
                    );
                    if (fightGrid.CellsByCoordinates.TryGetValue(nextCellCoordinates, out Cell nextCell))
                    {
                        targetedCells.Add(nextCell);
                    }
                }
                else
                {
                    Vector2Int nextCellCoordinates = targetedCells.First().Coordinates + CellsSequence[i];
                    if (fightGrid.CellsByCoordinates.TryGetValue(nextCellCoordinates, out Cell nextCell))
                    {
                        targetedCells.Add(nextCell);
                    }
                }
            }

            if (StopedAtInacessibles)
            {
                targetedCells = GetCellsUntilStoppedByInaccessible(targetedCells);
            }
            if (StopedAtXthEncounteredFighters > 0)
            {
                targetedCells = GetCellsUntilStoppedByTheXthFighter(targetedCells);
            }
            if (StopedAtXthHigherHeightDifference > 0)
            {
                targetedCells = GetCellsUntilStoppedByTooHighCell(targetedCells, StartFromInitiator ? initiatorCell : originCell);
            }
            if (StopedAtXthLowerHeightDifference > 0)
            {
                targetedCells = GetCellsUntilStoppedByTooLowCell(targetedCells, StartFromInitiator ? initiatorCell : originCell);
            }

            return targetedCells.ToArray();
        }

        /// <summary>
        /// Returns all the cells that are in range depending on the initiator position and the targeter.
        /// </summary>
        /// <param name="fightGrid">The fight grid to get the cells from.</param>
        /// <param name="initiatorCell">The cell the initiator occupies.</param>
        /// <param name="fightersTeams">The fighters and their teams (true if ally, false if enemy).</param>
        /// <returns>All the cells that are in range depending on the initiator position and the targeter</returns>
        public Cell[] GetAllCellsAvailableForTargeting(HexGrid fightGrid, Cell initiatorCell, Dictionary<Fighter, bool> fightersTeams)
        {
            List<Cell> availableCells = new();
            foreach (Cell cell in fightGrid.GetCells())
            {
                try
                {
                    if (Resolve(fightGrid, initiatorCell, cell, fightersTeams).Length > 0 && (!StartFromInitiator || cell != initiatorCell))
                    {
                        availableCells.Add(cell);
                    }
                }
                catch (TargeterUnresolvableException)
                {
                    continue;
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
        /// <param name="fightersTeams">The fighters and their teams (true if ally, false if enemy).</param>
        /// <returns>True if the target cell is in range, false otherwise.</returns>
        public bool IsCellTargetable(HexGrid fightGrid, Cell initiatorCell, Cell targetCell, Dictionary<Fighter, bool> fightersTeams)
        {
            return GetAllCellsAvailableForTargeting(fightGrid, initiatorCell, fightersTeams).Contains(targetCell);
        }

        /// <summary>
        /// Returns whether the targeter resolves at least for one cell in the available cells around the given initiator cell.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the initiator is located.</param>
        /// <param name="initiatorCell">The targeter initiator's cell.</param>*
        /// <param name="fightersTeams">The fighters and their teams (true if ally, false if enemy).</param>
        /// <returns>True if the targeter resolves at least for one cell in the available cells around the given initiator cell, false otherwise.</returns>
        public bool AtLeastOneCellResolvable(HexGrid fightGrid, Cell initiatorCell, Dictionary<Fighter, bool> fightersTeams)
        {
            foreach (Cell cellThatCanBeTargeted in GetAllCellsAvailableForTargeting(fightGrid, initiatorCell, fightersTeams))
            {
                try
                {
                    if (Resolve(fightGrid, initiatorCell, cellThatCanBeTargeted, fightersTeams).Length > 0)
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

        private bool CheckAtLeastOneFighterPresent(Cell[] targetedCells)
        {
            if (!targetedCells.Any(cell => cell.GetComponent<CellFightBehaviour>().Fighter != null))
            {
                throw new TargeterUnresolvableException("No fighter present in available targets.");
            }
            return true;
        }

        private bool CheckExcludedTargets(Cell[] targetedCells, Cell initiatorCell, Dictionary<Fighter, bool> fightersTeams)
        {
            bool initiatorIsAlly = fightersTeams.First(fighterTeam => fighterTeam.Key.cell == initiatorCell).Value;
            foreach (ETarget target in TargetsToExclude)
            {
                switch (target)
                {
                    case ETarget.SELF:
                        if (targetedCells.Contains(initiatorCell))
                        {
                            throw new TargeterUnresolvableException("Self exluded from available targets.");
                        }
                        break;
                    case ETarget.ALLIES:
                        List<Cell> alliesCells = fightersTeams.Where(fighterTeam =>
                            fighterTeam.Value == initiatorIsAlly && fighterTeam.Key.cell != initiatorCell
                        ).Select(fighterTeam => fighterTeam.Key.cell).ToList();
                        if (targetedCells.Any(cell => alliesCells.Contains(cell)))
                        {
                            throw new TargeterUnresolvableException("Allies exluded from available targets.");
                        }
                        break;
                    case ETarget.OPONENTS:
                        List<Cell> oponentsCells = fightersTeams.Where(fighterTeam =>
                            fighterTeam.Value != initiatorIsAlly
                        ).Select(fighterTeam => fighterTeam.Key.cell).ToList();
                        if (targetedCells.Any(cell => oponentsCells.Contains(cell)))
                        {
                            throw new TargeterUnresolvableException("Oponents exluded from available targets.");
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        private bool CheckExcludedRelativeHeights(Cell[] targetedCells, Cell initiatorCell, Dictionary<Fighter, bool> fightersTeams)
        {
            foreach (int excludedHeight in RelativeHeightsToExclude)
            {
                if (targetedCells.Any(cell =>
                {
                    int relativeHeight = (int)cell.Height - (int)initiatorCell.Height;
                    return relativeHeight == excludedHeight;
                }))
                {
                    throw new TargeterUnresolvableException("Height exluded from available targets.");
                }
            }
            return true;
        }

        private List<Cell> GetCellsUntilStoppedByInaccessible(List<Cell> targetedCells)
        {
            int indexOfFirstInaccessible = targetedCells.FindIndex(cell => !cell.IsAccessible);
            if (indexOfFirstInaccessible != -1)
            {
                targetedCells = targetedCells.Take(indexOfFirstInaccessible).ToList();
            }
            return targetedCells;
        }

        private List<Cell> GetCellsUntilStoppedByTheXthFighter(List<Cell> targetedCells)
        {
            int fightersEncountered = 0;
            for (int i = 0; i < targetedCells.Count; i++)
            {
                if (fightersEncountered == StopedAtXthEncounteredFighters)
                {
                    targetedCells = targetedCells.Take(i).ToList();
                    break;
                }

                if (fightersEncountered < StopedAtXthEncounteredFighters && targetedCells[i].GetComponent<CellFightBehaviour>().Fighter != null)
                {
                    fightersEncountered++;
                }
            }
            return targetedCells;
        }

        private List<Cell> GetCellsUntilStoppedByTooHighCell(List<Cell> targetedCells, Cell originCell)
        {
            Cell currentCompareCell = originCell;
            for (int i = 0; i < targetedCells.Count; i++)
            {
                if ((int)targetedCells[i].Height >= (int)currentCompareCell.Height &&
                    (int)targetedCells[i].Height - (int)currentCompareCell.Height >= StopedAtXthHigherHeightDifference)
                {
                    targetedCells = targetedCells.Take(i).ToList();
                    break;
                }
                currentCompareCell = targetedCells[i];
            }
            return targetedCells;
        }

        private List<Cell> GetCellsUntilStoppedByTooLowCell(List<Cell> targetedCells, Cell originCell)
        {
            Cell currentCompareCell = originCell;
            for (int i = 0; i < targetedCells.Count; i++)
            {
                if ((int)targetedCells[i].Height <= (int)currentCompareCell.Height &&
                    (int)currentCompareCell.Height - (int)targetedCells[i].Height >= StopedAtXthLowerHeightDifference)
                {
                    targetedCells = targetedCells.Take(i).ToList();
                    break;
                }
                currentCompareCell = targetedCells[i];
            }
            return targetedCells;
        }

        private Vector2Int GetNextCellCoordinatesToDirection(
            Cell originCell,
            Vector2Int baseNextCellCoordinates, // This represents the cell sequence delta (like [0,1], etc.)
            Vector2Int direction // This represents the direction between the initiator and the origin cell
        )
        {
            // Convert origin and baseNextCell from offset (x,y) to axial (q, r)
            Vector2Int axialOrigin = originCell.AxialCoordinates;
            Vector2Int axialBase = HexMetrics.OffsetToAxial(baseNextCellCoordinates);

            // Apply rotation to the axial base vector depending on the direction.
            // You can decide how many rotations to apply based on the direction vector (for example, if direction is [0, 1], rotate 60 degrees).
            Vector2Int rotatedAxial = HexMetrics.RotateAxialVector(axialBase, direction);

            // Now, calculate the new coordinates in axial system
            Vector2Int nextAxial = axialOrigin + rotatedAxial;

            // Convert the new axial coordinates back to offset (x,y) coordinates
            return HexMetrics.AxialToOffset(nextAxial);
        }
    }
}
