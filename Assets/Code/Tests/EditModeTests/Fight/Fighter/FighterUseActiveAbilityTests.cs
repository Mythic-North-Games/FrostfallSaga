using System;
using UnityEngine;
using NUnit.Framework;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Effects;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterUseActiveAbilityTests
    {
        private static readonly int FIRE_SPELL_DAMAGE_AMOUNT = 5;

        [Test]
        public void UseActiveAbility_OneReceiver_Test()
        {
            // Arrange
            HexGrid grid = FightTestsHelper.CreatePlainFightGrid();
            Fighter attacker = FightTestsHelper.CreateFighter();
            attacker.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, attacker, new Vector2Int(0, 0));
            Fighter receiver = FightTestsHelper.CreateFighter();
            receiver.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, receiver, new Vector2Int(0, 1));
            ActiveAbilityToAnimation activeAbilityToUse = attacker.FighterConfiguration.AvailableActiveAbilities[0];

            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - activeAbilityToUse.activeAbility.ActionPointsCost;
            int expectedReceiverHealth = (
                receiver.GetStatsForTests().health -
                FIRE_SPELL_DAMAGE_AMOUNT +
                receiver.GetStatsForTests().magicalResistances[EMagicalElement.FIRE]
            );

            Cell[] targetedCells = { receiver.cell };

            // Act
            attacker.UseActiveAbility(activeAbilityToUse, targetedCells);

            /// ASSERTS ///
            // Check actions points have been decreased
            Assert.AreEqual(expectedActionPoints, attacker.GetStatsForTests().actionPoints);

            // Check effects have been applied
            Assert.AreEqual(expectedReceiverHealth, receiver.GetStatsForTests().health);
        }

        [Test]
        public void UseActiveAbility_MultipleReceivers_Test()
        {
            // Arrange
            HexGrid grid = FightTestsHelper.CreatePlainFightGrid();
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
            receiver2.GetStatsForTests().magicalResistances[EMagicalElement.FIRE] = 0;

            ActiveAbilityToAnimation activeAbilityToUse = attacker.FighterConfiguration.AvailableActiveAbilities[0];

            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - activeAbilityToUse.activeAbility.ActionPointsCost;
            int expectedReceiverHealth = (
                receiver.GetStatsForTests().health -
                FIRE_SPELL_DAMAGE_AMOUNT +
                receiver.GetStatsForTests().magicalResistances[EMagicalElement.FIRE]
            );
            int expectedReceiverHealth2 = (
                receiver2.GetStatsForTests().health -
                FIRE_SPELL_DAMAGE_AMOUNT +
                receiver2.GetStatsForTests().magicalResistances[EMagicalElement.FIRE]
            );

            Cell[] targetedCells = {
                receiver.cell,
                receiver2.cell,
                grid.CellsByCoordinates[new(1, 1)],
            };

            // Act
            attacker.UseActiveAbility(activeAbilityToUse, targetedCells);

            /// ASSERTS ///
            // Check actions points have been decreased
            Assert.AreEqual(expectedActionPoints, attacker.GetStatsForTests().actionPoints);

            // Check effects have been applied
            Assert.AreEqual(expectedReceiverHealth, receiver.GetStatsForTests().health);
            Assert.AreEqual(expectedReceiverHealth2, receiver2.GetStatsForTests().health);
        }

        [Test]
        public void UseActiveAbility_NoFighterOnCells_Test()
        {
            // Arrange
            HexGrid grid = FightTestsHelper.CreatePlainFightGrid();
            Fighter attacker = FightTestsHelper.CreateFighter();
            attacker.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, attacker, new Vector2Int(0, 0));
            Fighter notTargetedFighter = FightTestsHelper.CreateFighter();
            notTargetedFighter.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, notTargetedFighter, new Vector2Int(0, 1));

            ActiveAbilityToAnimation activeAbilityToUse = attacker.FighterConfiguration.AvailableActiveAbilities[0];

            int expectedActionPoints = attacker.GetStatsForTests().actionPoints - activeAbilityToUse.activeAbility.ActionPointsCost;
            int expectedNotTargetedFighterHealth = notTargetedFighter.GetStatsForTests().health;

            Cell[] targetedCells = { grid.CellsByCoordinates[new(1, 1)] };

            // Act
            attacker.UseActiveAbility(activeAbilityToUse, targetedCells);

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
            HexGrid grid = FightTestsHelper.CreatePlainFightGrid();
            Fighter attacker = FightTestsHelper.CreateFighter();
            attacker.SetStatsForTests();
            FightTestsHelper.SetupFighterPositionOnGrid(grid, attacker, new Vector2Int(0, 0));
            ActiveAbilityToAnimation activeAbilityToUse = attacker.FighterConfiguration.AvailableActiveAbilities[0];
            Cell[] targetedCells = { };

            // Act
            Assert.Throws<ArgumentException>(() => attacker.UseActiveAbility(activeAbilityToUse, targetedCells));
        }

        [Test]
        public void UseDirectAttack_NotEnoughActionPoints_Test()
        {
            // Arrange
            HexGrid grid = FightTestsHelper.CreatePlainFightGrid();
            Fighter attacker = FightTestsHelper.CreateFighter();
            attacker.SetStatsForTests();
            attacker.GetStatsForTests().actionPoints = 1;
            FightTestsHelper.SetupFighterPositionOnGrid(grid, attacker, new Vector2Int(0, 0));
            ActiveAbilityToAnimation activeAbilityToUse = attacker.FighterConfiguration.AvailableActiveAbilities[0];
            Cell[] targetedCells = { grid.CellsByCoordinates[new(0, 1)] };

            // Act
            Assert.Throws<InvalidOperationException>(() => attacker.UseActiveAbility(activeAbilityToUse, targetedCells));
        }
    }
}
