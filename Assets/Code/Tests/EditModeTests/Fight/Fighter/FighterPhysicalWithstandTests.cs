using FrostfallSaga.Fight.Fighters;
using NUnit.Framework;
using UnityEngine;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterPhysicalWithstandTests
    {
        private Fighter fighter;

        [SetUp]
        public void Setup()
        {
            fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(fighter.gameObject);
            fighter = null;
        }

        [Test]
        public void PhysicalWithstand_PositiveAmountAdded_Test()
        {
            // Arrange
            var damageAmount = 10;
            var initialFighterHealth = 50;
            var fighterPhysicalResistance = 5;
            var expectedFighterHealth = initialFighterHealth - (damageAmount - fighterPhysicalResistance);

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().physicalResistance = fighterPhysicalResistance;

            // Act
            fighter.PhysicalWithstand(damageAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void PhysicalWithstand_HigherResistance_Test()
        {
            // Arrange
            var damageAmount = 10;
            var initialFighterHealth = 50;
            var fighterPhysicalResistance = 20;
            var expectedFighterHealth = initialFighterHealth;

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().physicalResistance = fighterPhysicalResistance;

            // Act
            fighter.PhysicalWithstand(damageAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void PhysicalWithstand_NullAmountAdded_Test()
        {
            // Arrange
            var damageAmount = 0;
            var initialFighterHealth = 50;
            var fighterPhysicalResistance = 5;
            var expectedFighterHealth = initialFighterHealth;

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().physicalResistance = fighterPhysicalResistance;

            // Act
            fighter.PhysicalWithstand(damageAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void PhysicalWithstand_NegativeAmountAdded_Test()
        {
            // Arrange
            var damageAmount = -10;
            var initialFighterHealth = 50;
            var fighterPhysicalResistance = 5;
            var expectedFighterHealth = initialFighterHealth;

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().physicalResistance = fighterPhysicalResistance;

            // Act
            fighter.PhysicalWithstand(damageAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void PhysicalWithstand_MinHealthCap_Test()
        {
            // Arrange
            var damageAmount = 1000;
            var initialFighterHealth = 50;
            var fighterPhysicalResistance = 5;
            var expectedFighterHealth = 0;

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().physicalResistance = fighterPhysicalResistance;

            // Act
            fighter.PhysicalWithstand(damageAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }
    }
}