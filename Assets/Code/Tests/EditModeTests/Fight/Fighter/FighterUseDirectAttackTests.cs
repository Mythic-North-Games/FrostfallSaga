using System;
using FrostfallSaga.Fight;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterUseDirectAttackTests
    {
        private Fighter attacker;
        private FightHexGrid grid;
        private int maxAttackerDamages;
        private int minAttackerDamages;


        [SetUp]
        public void Setup()
        {
            // Set up grid
            grid = CommonTestsHelper.CreatePlainGridFightForTest();

            // Set up attacker
            attacker = FightTestsHelper.CreateFighter();
            attacker.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, attacker, new Vector2Int(0, 0));

            minAttackerDamages = attacker.Weapon.MinPhysicalDamages;
            maxAttackerDamages = attacker.Weapon.MaxPhysicalDamages;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(grid.gameObject);
            grid = null;

            Object.DestroyImmediate(attacker.gameObject);
            attacker = null;

            minAttackerDamages = 0;
            maxAttackerDamages = 0;
        }

        [Test]
        public void UseDirectAttack_OneReceiver_Test()
        {
            // Arrange
            Fighter receiver = FightTestsHelper.CreateFighter();
            receiver.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, receiver, new Vector2Int(0, 1));

            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - attacker.Weapon.UseActionPointsCost;
            int minExpectedReceiverHealth = Math.Max(0, receiver.GetStatsForTests().health - maxAttackerDamages);
            int maxExpectedReceiverHealth = Math.Max(0, receiver.GetStatsForTests().health - minAttackerDamages);

            FightCell[] targetedCells = { receiver.cell };

            // Act
            attacker.onDirectAttackEnded += attacker =>
            {
                /// ASSERTS ///
                // Check actions points have been decreased
                Assert.AreEqual(expectedActionPoints, attacker.GetStatsForTests().actionPoints);

                // Check effects have been applied
                Assert.GreaterOrEqual(receiver.GetStatsForTests().health, minExpectedReceiverHealth);
                Assert.LessOrEqual(receiver.GetStatsForTests().health, maxExpectedReceiverHealth);
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

            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - attacker.Weapon.UseActionPointsCost;

            int minExpectedReceiverHealth = Math.Max(0, receiver.GetStatsForTests().health - maxAttackerDamages);
            int maxExpectedReceiverHealth = Math.Max(0, receiver.GetStatsForTests().health - minAttackerDamages);

            int minExpectedReceiverHealth2 = Math.Max(0, receiver2.GetStatsForTests().health - maxAttackerDamages);
            int maxExpectedReceiverHealth2 = Math.Max(0, receiver2.GetStatsForTests().health - minAttackerDamages);

            FightCell[] targetedCells =
            {
                receiver.cell,
                receiver2.cell,
                (FightCell)grid.Cells[new Vector2Int(1, 1)]
            };

            // Act
            attacker.onDirectAttackEnded += attacker =>
            {
                /// ASSERTS ///
                Assert.AreEqual(expectedActionPoints, attacker.GetStatsForTests().actionPoints);

                // Check effects have been applied
                Assert.GreaterOrEqual(receiver.GetStatsForTests().health, minExpectedReceiverHealth);
                Assert.LessOrEqual(receiver.GetStatsForTests().health, maxExpectedReceiverHealth);

                Assert.GreaterOrEqual(receiver2.GetStatsForTests().health, minExpectedReceiverHealth2);
                Assert.LessOrEqual(receiver2.GetStatsForTests().health, maxExpectedReceiverHealth2);
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
            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - attacker.Weapon.UseActionPointsCost;
            int expectedNotTargetedFighterHealth = notTargetedFighter.GetStatsForTests().health;

            FightCell[] targetedCells = { (FightCell)grid.Cells[new Vector2Int(1, 1)] };

            // Act
            attacker.onDirectAttackEnded += attacker =>
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
            FightCell[] targetedCells = { (FightCell)grid.Cells[new Vector2Int(0, 1)] };

            // Act
            Assert.Throws<InvalidOperationException>(() => attacker.UseDirectAttack(targetedCells));
        }
    }
}