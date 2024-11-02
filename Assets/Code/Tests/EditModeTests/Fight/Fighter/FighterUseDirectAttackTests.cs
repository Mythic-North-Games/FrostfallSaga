using System;
using NUnit.Framework;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.FightCells;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterUseDirectAttackTests
    {
        [Test]
        public void UseDirectAttack_OneReceiver_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(fightCell: true);
            Fighter attacker = FightTestsHelper.CreateFighter();
            attacker.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, attacker, new Vector2Int(0, 0));
            Fighter receiver = FightTestsHelper.CreateFighter();
            receiver.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, receiver, new Vector2Int(0, 1));

            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - attacker.DirectAttackActionPointsCost;

            int attackerDamages = ((PhysicalDamageEffectSO)attacker.DirectAttackEffects[0]).PhysicalDamageAmount;
            int expectedReceiverHealth = Math.Max(0, receiver.GetStatsForTests().health - attackerDamages);

            FightCell[] targetedCells = { receiver.cell };

            // Act
            attacker.UseDirectAttack(targetedCells);

            /// ASSERTS ///
            // Check actions points have been decreased
            Assert.AreEqual(expectedActionPoints, attacker.GetStatsForTests().actionPoints);

            // Check effects have been applied
            Assert.AreEqual(expectedReceiverHealth, receiver.GetStatsForTests().health);
        }

        [Test]
        public void UseDirectAttack_MultipleReceivers_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(fightCell: true);
            Fighter attacker = FightTestsHelper.CreateFighter();
            attacker.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, attacker, new Vector2Int(0, 0));

            Fighter receiver = FightTestsHelper.CreateFighter();
            receiver.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, receiver, new Vector2Int(0, 1));
            receiver.name = "Receiver 1";

            Fighter receiver2 = FightTestsHelper.CreateFighter();
            receiver2.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, receiver2, new Vector2Int(1, 0));
            receiver2.name = "Receiver 2";

            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - attacker.DirectAttackActionPointsCost;

            int attackerDamages = ((PhysicalDamageEffectSO)attacker.DirectAttackEffects[0]).PhysicalDamageAmount;
            int expectedReceiverHealth = Math.Max(0, receiver.GetStatsForTests().health - attackerDamages);
            int expectedReceiverHealth2 = Math.Max(0, receiver2.GetStatsForTests().health - attackerDamages);

            FightCell[] targetedCells = {
                receiver.cell,
                receiver2.cell,
                (FightCell)grid.CellsByCoordinates[new(1, 1)],
            };

            // Act
            attacker.UseDirectAttack(targetedCells);

            /// ASSERTS ///
            // Check actions points have been decreased
            Assert.AreEqual(expectedActionPoints, attacker.GetStatsForTests().actionPoints);

            // Check effects have been applied
            Assert.AreEqual(expectedReceiverHealth, receiver.GetStatsForTests().health);
            Assert.AreEqual(expectedReceiverHealth2, receiver2.GetStatsForTests().health);
        }

        [Test]
        public void UseDirectAttack_NoFighterOnCells_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(fightCell: true);
            Fighter attacker = FightTestsHelper.CreateFighter();
            attacker.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, attacker, new Vector2Int(0, 0));
            Fighter notTargetedFighter = FightTestsHelper.CreateFighter();
            notTargetedFighter.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, notTargetedFighter, new Vector2Int(0, 1));
            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - attacker.DirectAttackActionPointsCost;
            int expectedNotTargetedFighterHealth = notTargetedFighter.GetStatsForTests().health;

            FightCell[] targetedCells = { (FightCell)grid.CellsByCoordinates[new(1, 1)] };

            // Act
            attacker.UseDirectAttack(targetedCells);

            /// ASSERTS ///
            // Check actions points have been decreased
            Assert.AreEqual(expectedActionPoints, attacker.GetStatsForTests().actionPoints);

            // Check no effect have been applied to other fighter
            Assert.AreEqual(expectedNotTargetedFighterHealth, notTargetedFighter.GetStatsForTests().health);
        }

        [Test]
        public void UseDirectAttack_NoTargetCells_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(fightCell: true);
            Fighter attacker = FightTestsHelper.CreateFighter();
            attacker.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, attacker, new Vector2Int(0, 0));
            FightCell[] targetedCells = { };

            // Act
            Assert.Throws<ArgumentException>(() => attacker.UseDirectAttack(targetedCells));
        }

        [Test]
        public void UseDirectAttack_NotEnoughActionPoints_Test()
        {
            // Arrange
            HexGrid grid = CommonTestsHelper.CreatePlainGridForTest(fightCell: true);
            Fighter attacker = FightTestsHelper.CreateFighter();
            attacker.SetStatsForTests();
            attacker.GetStatsForTests().actionPoints = 1;
            FightTestsHelper.SetupFighterPositionOnGrid(grid, attacker, new Vector2Int(0, 0));
            FightCell[] targetedCells = { (FightCell)grid.CellsByCoordinates[new(0, 1)] };

            // Act
            Assert.Throws<InvalidOperationException>(() => attacker.UseDirectAttack(targetedCells));
        }
    }
}