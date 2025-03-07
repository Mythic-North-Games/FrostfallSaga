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
            int healAmount = 10;
            int initialFighterHealth = 50;
            int expectedFighterHealth = initialFighterHealth + healAmount;

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
            int healAmount = 0;
            int initialFighterHealth = 50;
            int expectedFighterHealth = initialFighterHealth + healAmount;

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
            int healAmount = -10;
            int initialFighterHealth = 50;
            int expectedFighterHealth = initialFighterHealth + healAmount;

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
            int healAmount = 1000;
            int initialFighterHealth = 50;

            fighter.GetStatsForTests().health = initialFighterHealth;

            int expectedFighterHealth = fighter.GetStatsForTests().maxHealth;

            // Act
            fighter.Heal(healAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void Heal_MinHealthCap_Test()
        {
            // Arrange
            int healAmount = -1000;
            int initialFighterHealth = 50;
            int expectedFighterHealth = 0;

            fighter.GetStatsForTests().health = initialFighterHealth;


            // Act
            fighter.Heal(healAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }
    }
}