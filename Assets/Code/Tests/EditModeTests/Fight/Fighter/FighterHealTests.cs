using FrostfallSaga.Fight.Fighters;
using NUnit.Framework;
using UnityEngine;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterHealTests
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
        public void Heal_PositiveAmountAdded_Test()
        {
            // Arrange
            var healAmount = 10;
            var initialFighterHealth = 50;
            var expectedFighterHealth = initialFighterHealth + healAmount;

            fighter.GetStatsForTests().health = initialFighterHealth;

            // Act
            fighter.Heal(healAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void Heal_NullAmountAdded_Test()
        {
            // Arrange
            var healAmount = 0;
            var initialFighterHealth = 50;
            var expectedFighterHealth = initialFighterHealth + healAmount;

            fighter.GetStatsForTests().health = initialFighterHealth;

            // Act
            fighter.Heal(healAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void Heal_NegativeAmountAdded_Test()
        {
            // Arrange
            var healAmount = -10;
            var initialFighterHealth = 50;
            var expectedFighterHealth = initialFighterHealth + healAmount;

            fighter.GetStatsForTests().health = initialFighterHealth;

            // Act
            fighter.Heal(healAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void Heal_MaxHealthCap_Test()
        {
            // Arrange
            var healAmount = 1000;
            var initialFighterHealth = 50;

            fighter.GetStatsForTests().health = initialFighterHealth;

            var expectedFighterHealth = fighter.GetStatsForTests().maxHealth;

            // Act
            fighter.Heal(healAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void Heal_MinHealthCap_Test()
        {
            // Arrange
            var healAmount = -1000;
            var initialFighterHealth = 50;
            var expectedFighterHealth = 0;

            fighter.GetStatsForTests().health = initialFighterHealth;


            // Act
            fighter.Heal(healAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }
    }
}