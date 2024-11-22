using System;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;
using NUnit.Framework;
using UnityEngine;
using FrostfallSaga.Fight.FightCells;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterUseDirectAttackTests
    {
        HexGrid grid;
        Fighter attacker;
        int attackerDamages;

        [SetUp]
        public void Setup()
        {
            // Set up grid
            grid = CommonTestsHelper.CreatePlainGridForTest(fightCell: true);

            // Set up attacker
            attacker = FightTestsHelper.CreateFighter();
            attacker.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, attacker, new Vector2Int(0, 0));

            attackerDamages = ((PhysicalDamageEffect)attacker.DirectAttackEffects[0]).PhysicalDamageAmount;
        }

        [Test]
        public void UseDirectAttack_OneReceiver_Test()
        {
            // Arrange
            Fighter receiver = FightTestsHelper.CreateFighter();
            receiver.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, receiver, new Vector2Int(0, 1));

            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - attacker.DirectAttackActionPointsCost;
            int expectedReceiverHealth = Math.Max(0, receiver.GetStatsForTests().health - attackerDamages);

            FightCell[] targetedCells = { receiver.cell };

            // Act
            attacker.onDirectAttackEnded += (Fighter attacker) =>
            {
                /// ASSERTS ///
                // Check actions points have been decreased
                Assert.AreEqual(expectedActionPoints, attacker.GetStatsForTests().actionPoints);

                // Check effects have been applied
                Assert.AreEqual(expectedReceiverHealth, receiver.GetStatsForTests().health);
            };
            attacker.UseDirectAttack(targetedCells);
        }

        [Test]
        public void UseDirectAttack_MultipleReceivers_Test()
        {
            // Arrange
            Fighter receiver = FightTestsHelper.CreateFighter();
            receiver.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, receiver, new Vector2Int(0, 1));
            receiver.name = "Receiver 1";

            Fighter receiver2 = FightTestsHelper.CreateFighter();
            receiver2.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, receiver2, new Vector2Int(1, 0));
            receiver2.name = "Receiver 2";

            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - attacker.DirectAttackActionPointsCost;
            int expectedReceiverHealth = Math.Max(0, receiver.GetStatsForTests().health - attackerDamages);
            int expectedReceiverHealth2 = Math.Max(0, receiver2.GetStatsForTests().health - attackerDamages);

            FightCell[] targetedCells = {
                receiver.cell,
                receiver2.cell,
                (FightCell)grid.CellsByCoordinates[new(1, 1)],
            };

            // Act
            attacker.onDirectAttackEnded += (Fighter attacker) =>
            {
                /// ASSERTS ///
                Assert.AreEqual(expectedActionPoints, attacker.GetStatsForTests().actionPoints);
                Assert.AreEqual(expectedReceiverHealth, receiver.GetStatsForTests().health);
                Assert.AreEqual(expectedReceiverHealth2, receiver2.GetStatsForTests().health);
            };
            attacker.UseDirectAttack(targetedCells);
        }

        [Test]
        public void UseDirectAttack_NoFighterOnCells_Test()
        {
            // Arrange
            Fighter notTargetedFighter = FightTestsHelper.CreateFighter();
            notTargetedFighter.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, notTargetedFighter, new Vector2Int(0, 1));
            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - attacker.DirectAttackActionPointsCost;
            int expectedNotTargetedFighterHealth = notTargetedFighter.GetStatsForTests().health;

            FightCell[] targetedCells = { (FightCell)grid.CellsByCoordinates[new(1, 1)] };

            // Act
            attacker.onDirectAttackEnded += (Fighter attacker) =>
            {
                /// ASSERTS ///
                // Check actions points have been decreased
                Assert.AreEqual(expectedActionPoints, attacker.GetStatsForTests().actionPoints);

                // Check no effect have been applied to other fighter
                Assert.AreEqual(expectedNotTargetedFighterHealth, notTargetedFighter.GetStatsForTests().health);
            };
            attacker.UseDirectAttack(targetedCells);
        }

        [Test]
        public void UseDirectAttack_NoTargetCells_Test()
        {
            // Arrange
            FightCell[] targetedCells = { };

            // Act
            Assert.Throws<ArgumentException>(() => attacker.UseDirectAttack(targetedCells));
        }

        [Test]
        public void UseDirectAttack_NotEnoughActionPoints_Test()
        {
            // Arrange
            attacker.GetStatsForTests().actionPoints = 1;
            FightCell[] targetedCells = { (FightCell)grid.CellsByCoordinates[new(0, 1)] };

            // Act
            Assert.Throws<InvalidOperationException>(() => attacker.UseDirectAttack(targetedCells));
        }
    }
}