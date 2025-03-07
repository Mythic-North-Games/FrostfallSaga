using System;
using UnityEngine;
using System.Collections.Generic;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.FightCells.Impediments
{
    public class TrapManager
    {
        private HashSet<Cell> VisibleTrapcells = new HashSet<Cell>();
        public int adjustedOffset = 3;

        public void UpdateTrapDetection(int range, Fighter fighter)
        {
            if (fighter.FighterConfiguration.CanDetectTraps == false) return;

            List<Cell> cells = CellsNeighbors.GetCellsInRange(range, fighter.cell);
            HashSet<Cell> trapsInRange = new HashSet<Cell>();

            foreach (FightCell cell in cells)
            {
                if (cell.TrapList.Count > 0)
                {
                    ShowTrap(cell);
                    cell.RevealTrap();

                    trapsInRange.Add(cell);
                }
            }

            foreach (FightCell visibleCell in new List<Cell>(VisibleTrapcells))
            {
                if (!trapsInRange.Contains(visibleCell))
                {
                    HideTrap(visibleCell);
                    visibleCell.HideTrap();
                    VisibleTrapcells.Remove(visibleCell);
                }
            }

            foreach (FightCell cell in trapsInRange)
            {
                if (!VisibleTrapcells.Contains(cell))
                {
                    VisibleTrapcells.Add(cell);
                    ShowTrap(cell);
                }
            }
        }

        public void ShowTrap(FightCell fightCell)
        {
            if (!fightCell.IsTrapVisible)
            {
                if (fightCell.TrapVisibleInstance == null)
                {
                    fightCell.TrapVisibleInstance = UnityEngine.Object.Instantiate(fightCell.TrapList[0].TrapIsVisiblePrefab,
                    fightCell.transform.position, Quaternion.identity, fightCell.transform);
                    Vector3 adjustedPosition = fightCell.TrapVisibleInstance.transform.position;
                    adjustedPosition.y += adjustedOffset;
                    fightCell.TrapVisibleInstance.transform.position = adjustedPosition;
                    fightCell.TrapVisibleInstance.transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    fightCell.TrapVisibleInstance.SetActive(true);
                }
            }
        }

        public void HideTrap(FightCell fightCell)
        {
            if (fightCell.IsTrapVisible)
            {
                if (fightCell.TrapVisibleInstance != null)
                {
                    fightCell.TrapVisibleInstance.SetActive(false);
                    fightCell.TrapVisibleInstance = null;
                }
            }
        }
    }
}
