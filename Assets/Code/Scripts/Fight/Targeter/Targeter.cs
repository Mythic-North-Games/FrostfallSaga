using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Fight.Targeters
{
    /// <summary>
    ///     Defines a targeter. Will be used to select cells for abilities and weapons.
    /// </summary>
    [Serializable]
    public class Targeter
    {
        /// <summary>
        ///     Targeter base definition
        /// </summary>
        [field: SerializeField]
        [field: Header("Definition")]
        [field: Tooltip("The available range from the intiator's cell available for targeting.")]
        public int OriginCellRange { get; private set; }

        [field: SerializeField]
        [field: Tooltip("The coordinates of the cells to target (ex: line would be [0,0], [0,1], [0,2]).")]
        public Vector2Int[] CellsSequence { get; private set; }

        [field: SerializeField]
        [field:
            Tooltip(
                "If true, the cell sequence will rotate towards fighter direction. It will also affect the obstacles checks.")]
        public bool StartFromInitiator { get; private set; }


        /// <summary>
        ///     Resolve conditions
        /// </summary>
        [field: SerializeField]
        [field: Header("Resolve conditions")]
        [field:
            Tooltip("If true, the targeter will only resolve if a fighter is present on one of the targeted cells.")]
        public bool FighterMandatory { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Can target all fighter types by default. Add options to restrain who can be targeted.")]
        public ETarget[] TargetsToExclude { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Can target all heights by default. Add options to restrain which heights can be targeted.")]
        [field: Range(-2, 2)]
        public int[] RelativeHeightsToExclude { get; private set; }


        /// <summary>
        ///     Obstacles that stops the targeter
        /// </summary>
        [field: SerializeField]
        [field: Header("Obstacles")]
        [field: Tooltip("If true, the targeter will resolve cells until stoped by an inaccessible cell.")]
        public bool StopedAtInacessibles { get; private set; }

        [field: SerializeField]
        [field:
            Tooltip(
                "The targeter will resolve cells until stoped by a cell with at least X higher height difference. 0 means deactivated.")]
        [field: Range(0, 2)]
        public int StopedAtXthHigherHeightDifference { get; private set; }

        [field: SerializeField]
        [field:
            Tooltip(
                "The targeter will resolve cells until stoped by a cell with at least X lower height difference. 0 means deactivated.")]
        [field: Range(0, 2)]
        public int StopedAtXthLowerHeightDifference { get; private set; }

        [field: SerializeField]
        [field: Tooltip("The targeter will resolve cells until stoped by the Xth fighter encountered.")]
        [field: Range(0, 6)]
        public int StopedAtXthEncounteredFighters { get; private set; }


        /// <summary>
        ///     Extract the cells corresponding to the targeter from the given context.
        /// </summary>
        /// <param name="fightGrid">The fight grid that the targeter will extract the cells from.</param>
        /// <param name="initiatorCell">The cell of the targeter's initiator.</param>
        /// <param name="originCell">The starting cell of the sequence.</param>
        /// <param name="fightersTeams">The fighters and their teams (true if ally, false if enemy).</param>
        /// <returns>The extracted cells in order if resolvable.</returns>
        /// <exception cref="TargeterUnresolvableException">If one of the targeter's condition is not respected.</exception>
        public FightCell[] Resolve(
            AHexGrid fightGrid,
            FightCell initiatorCell,
            FightCell originCell,
            Dictionary<Fighter, bool> fightersTeams,
            AFightCellAlteration[] cellAlterations = null
        )
        {
            int distanceToInitiator = CellsPathFinding.GetShorterPath(
                fightGrid,
                initiatorCell,
                originCell,
                true,
                true,
                true
            ).Length;
            if (distanceToInitiator > OriginCellRange)
                throw new TargeterUnresolvableException("Initiator not in range");

            FightCell[] targetedCells = GetCellsFromSequence(fightGrid, initiatorCell, originCell, cellAlterations);
            if (targetedCells.Length == 0)
                throw new TargeterUnresolvableException("An alteration is blocking the origin cell.");

            if (FighterMandatory) CheckAtLeastOneFighterPresent(targetedCells);
            CheckExcludedTargets(targetedCells, initiatorCell, fightersTeams);
            CheckExcludedRelativeHeights(targetedCells, initiatorCell, fightersTeams);
            return targetedCells;
        }

        public List<FightCell[]> GetAllResolvedCellsSequences(
            AHexGrid fightGrid,
            FightCell initiatorCell,
            Dictionary<Fighter, bool> fightersTeams,
            AFightCellAlteration[] cellAlterations = null
        )
        {
            List<FightCell[]> resolvedTargeterSequences = new();
            foreach (
                FightCell cellThatCanBeTargeted in GetAllCellsAvailableForTargeting(
                    fightGrid,
                    initiatorCell,
                    fightersTeams,
                    cellAlterations
                )
            )
                try
                {
                    resolvedTargeterSequences.Add(Resolve(fightGrid, initiatorCell, cellThatCanBeTargeted,
                        fightersTeams));
                }
                catch (TargeterUnresolvableException)
                {
                }

            return resolvedTargeterSequences;
        }

        /// <summary>
        ///     Get one random resolved targeter cell sequence for the given context if it does exist.
        /// </summary>
        /// <param name="fightGrid">The current fight grid.</param>
        /// <param name="initiatorCell">The cell where the targeter's initiator is located.</param>
        /// <param name="fightersTeams">The fighters and their teams (true if ally, false if enemy).</param>
        /// <returns>One random resolved targeter cell sequence for the given context if it does exist.</returns>
        /// <exception cref="TargeterUnresolvableException">If the targeter can't be resolved around the initiator.</exception>
        public FightCell[] GetRandomTargetCells(
            AHexGrid fightGrid,
            FightCell initiatorCell,
            Dictionary<Fighter, bool> fightersTeams
        )
        {
            return Randomizer.GetRandomElementFromArray(
                GetAllResolvedCellsSequences(fightGrid, initiatorCell, fightersTeams).ToArray());
        }

        /// <summary>
        ///     Extract the cells corresponding the targeter's sequence from the given context.
        /// </summary>
        /// <param name="fightGrid">The fight grid that the targeter will extract the cells from.</param>
        /// <param name="initiatorCell">The cell where the targeter's initiator is located.</param>
        /// <param name="originCell">The starting cell of the sequence.</param>
        /// <param name="cellAlterations">The cell alterations that can be applied to the cells.</param>
        /// <returns>The extracted cells in order, targeter's conditions are ignored.</returns>
        public FightCell[] GetCellsFromSequence(
            AHexGrid fightGrid,
            FightCell initiatorCell,
            FightCell originCell,
            AFightCellAlteration[] cellAlterations = null
        )
        {
            if (CellsSequence.Length == 1 && IsCellBlockedByAlterations(originCell, cellAlterations))
                return new FightCell[0];

            List<FightCell> targetedCells = new()
            {
                originCell
            };
            Vector2Int direction = Cell.GetHexDirection(initiatorCell, originCell);

            for (int i = 1; i < CellsSequence.Length; i++)
                if (StartFromInitiator)
                {
                    Vector2Int nextCellCoordinates = GetNextCellCoordinatesToDirection(
                        originCell,
                        CellsSequence[i],
                        direction
                    );
                    if (
                        fightGrid.Cells.TryGetValue(nextCellCoordinates, out Cell nextCell) &&
                        !IsCellBlockedByAlterations((FightCell)nextCell, cellAlterations)
                    )
                        targetedCells.Add((FightCell)nextCell);
                }
                else
                {
                    Vector2Int nextCellCoordinates = targetedCells.First().Coordinates + CellsSequence[i];
                    if (
                        fightGrid.Cells.TryGetValue(nextCellCoordinates, out Cell nextCell) &&
                        !IsCellBlockedByAlterations((FightCell)nextCell, cellAlterations)
                    )
                        targetedCells.Add((FightCell)nextCell);
                }

            if (cellAlterations != null)
            {
                if (cellAlterations.Any(alteration => !alteration.CanApplyWithFighter))
                    targetedCells = GetCellsUntilStoppedByTheXthFighter(targetedCells, 1);
                if (cellAlterations.Any(alteration => alteration is AddImpedimentAlteration))
                    targetedCells = GetCellsUntilStoppedByInaccessible(targetedCells);
            }

            if (StopedAtInacessibles) targetedCells = GetCellsUntilStoppedByInaccessible(targetedCells);
            if (StopedAtXthEncounteredFighters > 0)
                targetedCells = GetCellsUntilStoppedByTheXthFighter(targetedCells, StopedAtXthEncounteredFighters);
            if (StopedAtXthHigherHeightDifference > 0)
                targetedCells =
                    GetCellsUntilStoppedByTooHighCell(targetedCells, StartFromInitiator ? initiatorCell : originCell);
            if (StopedAtXthLowerHeightDifference > 0)
                targetedCells =
                    GetCellsUntilStoppedByTooLowCell(targetedCells, StartFromInitiator ? initiatorCell : originCell);

            return targetedCells.ToArray();
        }

        /// <summary>
        ///     Returns all the cells that are in range depending on the initiator position and the targeter.
        /// </summary>
        /// <param name="fightGrid">The fight grid to get the cells from.</param>
        /// <param name="initiatorCell">The cell the initiator occupies.</param>
        /// <param name="fightersTeams">The fighters and their teams (true if ally, false if enemy).</param>
        /// <param name="cellAlterations">The cell alterations that can be applied to the cells.</param>
        /// <returns>All the cells that are in range depending on the initiator position and the targeter</returns>
        public FightCell[] GetAllCellsAvailableForTargeting(
            AHexGrid fightGrid,
            FightCell initiatorCell,
            Dictionary<Fighter, bool> fightersTeams,
            AFightCellAlteration[] cellAlterations = null
        )
        {
            List<FightCell> availableCells = new();
            foreach (FightCell cell in fightGrid.GetCells())
                try
                {
                    if (
                        Resolve(
                            fightGrid, initiatorCell, cell, fightersTeams, cellAlterations
                        ).Length > 0 && (!StartFromInitiator || cell != initiatorCell)
                    )
                        availableCells.Add(cell);
                }
                catch (TargeterUnresolvableException)
                {
                }

            return availableCells.ToArray();
        }

        /// <summary>
        ///     Returns whether the given target cell is in the targeter range depending on the context.
        /// </summary>
        /// <param name="fightGrid">The grid where the initiator is located.</param>
        /// <param name="initiatorCell">The targeter initiator's cell.</param>
        /// <param name="targetCell">The cell you want to know if it's in range.</param>
        /// <param name="fightersTeams">The fighters and their teams (true if ally, false if enemy).</param>
        /// <param name="cellAlterations">The cell alterations that can be applied to the cells.</param>
        /// <returns>True if the target cell is in range, false otherwise.</returns>
        public bool IsCellTargetable(
            AHexGrid fightGrid,
            FightCell initiatorCell,
            FightCell targetCell,
            Dictionary<Fighter, bool> fightersTeams,
            AFightCellAlteration[] cellAlterations = null
        )
        {
            return GetAllCellsAvailableForTargeting(fightGrid, initiatorCell, fightersTeams, cellAlterations)
                .Contains(targetCell);
        }

        /// <summary>
        ///     Returns whether the targeter resolves at least for one cell in the available cells around the given initiator cell.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the initiator is located.</param>
        /// <param name="initiatorCell">The targeter initiator's cell.</param>
        /// *
        /// <param name="fightersTeams">The fighters and their teams (true if ally, false if enemy).</param>
        /// <param name="cellAlterations">The cell alterations that can be applied to the cells.</param>
        /// <returns>
        ///     True if the targeter resolves at least for one cell in the available cells around the given initiator cell,
        ///     false otherwise.
        /// </returns>
        public bool AtLeastOneCellResolvable(
            AHexGrid fightGrid,
            FightCell initiatorCell,
            Dictionary<Fighter, bool> fightersTeams,
            AFightCellAlteration[] cellAlterations = null
        )
        {
            foreach (
                FightCell cellThatCanBeTargeted in GetAllCellsAvailableForTargeting(
                    fightGrid,
                    initiatorCell,
                    fightersTeams,
                    cellAlterations
                )
            )
                try
                {
                    if (Resolve(fightGrid, initiatorCell, cellThatCanBeTargeted, fightersTeams).Length > 0) return true;
                }
                catch (TargeterUnresolvableException)
                {
                }

            return false;
        }

        private bool CheckAtLeastOneFighterPresent(FightCell[] targetedCells)
        {
            if (!targetedCells.Any(cell => cell.GetComponent<FightCell>().Fighter != null))
                throw new TargeterUnresolvableException("No fighter present in available targets.");
            return true;
        }

        private bool CheckExcludedTargets(
            FightCell[] targetedCells,
            FightCell initiatorCell,
            Dictionary<Fighter, bool> fightersTeams
        )
        {
            bool initiatorIsAlly = fightersTeams.First(fighterTeam => fighterTeam.Key.cell == initiatorCell).Value;
            foreach (ETarget target in TargetsToExclude)
                switch (target)
                {
                    case ETarget.SELF:
                        if (targetedCells.Contains(initiatorCell))
                            throw new TargeterUnresolvableException("Self exluded from available targets.");
                        break;
                    case ETarget.ALLIES:
                        List<FightCell> alliesCells = fightersTeams.Where(fighterTeam =>
                            fighterTeam.Value == initiatorIsAlly && fighterTeam.Key.cell != initiatorCell
                        ).Select(fighterTeam => fighterTeam.Key.cell).ToList();
                        if (targetedCells.Any(cell => alliesCells.Contains(cell)))
                            throw new TargeterUnresolvableException("Allies exluded from available targets.");
                        break;
                    case ETarget.OPONENTS:
                        List<FightCell> oponentsCells = fightersTeams.Where(fighterTeam =>
                            fighterTeam.Value != initiatorIsAlly
                        ).Select(fighterTeam => fighterTeam.Key.cell).ToList();
                        if (targetedCells.Any(cell => oponentsCells.Contains(cell)))
                            throw new TargeterUnresolvableException("Oponents exluded from available targets.");
                        break;
                }

            return true;
        }

        private bool CheckExcludedRelativeHeights(FightCell[] targetedCells, FightCell initiatorCell,
            Dictionary<Fighter, bool> fightersTeams)
        {
            foreach (int excludedHeight in RelativeHeightsToExclude)
                if (targetedCells.Any(cell =>
                    {
                        int relativeHeight = (int)cell.Height - (int)initiatorCell.Height;
                        return relativeHeight == excludedHeight;
                    }))
                    throw new TargeterUnresolvableException("Height exluded from available targets.");

            return true;
        }

        private List<FightCell> GetCellsUntilStoppedByInaccessible(List<FightCell> targetedCells)
        {
            int indexOfFirstInaccessible = targetedCells.FindIndex(cell => !cell.IsTerrainAccessible());
            if (indexOfFirstInaccessible != -1) targetedCells = targetedCells.Take(indexOfFirstInaccessible).ToList();
            return targetedCells;
        }

        private List<FightCell> GetCellsUntilStoppedByTheXthFighter(List<FightCell> targetedCells, int xthFighter)
        {
            int fightersEncountered = 0;
            for (int i = 0; i < targetedCells.Count; i++)
            {
                if (targetedCells[i].HasFighter()) fightersEncountered++;

                if (fightersEncountered == xthFighter + 1)
                {
                    targetedCells = targetedCells.Take(i).ToList();
                    break;
                }
            }

            return targetedCells;
        }

        private List<FightCell> GetCellsUntilStoppedByTooHighCell(List<FightCell> targetedCells, FightCell originCell)
        {
            FightCell currentCompareCell = originCell;
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

        private List<FightCell> GetCellsUntilStoppedByTooLowCell(List<FightCell> targetedCells, FightCell originCell)
        {
            FightCell currentCompareCell = originCell;
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
            FightCell originCell,
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

        private bool IsCellBlockedByAlterations(FightCell cell, AFightCellAlteration[] cellAlterations)
        {
            return cellAlterations != null && (
                cellAlterations.Any(alteration =>
                    alteration is SetHeightAlteration &&
                    ((SetHeightAlteration)alteration).TargetHeight == cell.Height) ||
                cell.GetAlterations().Any(
                    activeCellAlteration => cellAlterations.Any(
                        possibleCellAlteration =>
                            !activeCellAlteration.CanBeReplaced &&
                            activeCellAlteration.GetType() == possibleCellAlteration.GetType()
                    )
                )
            );
        }
    }
}