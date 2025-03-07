using System;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Fighters;
using NUnit.Framework;
using Object = UnityEngine.Object;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterMagicalWithstandTests
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
        public void MagicalWithstand_PositiveAmountAndResistanceToElement_Test()
        {
            // Arrange
            var damageAmount = 10;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            var initialFighterHealth = 50;
            var fighterMagicalResistance = 5;
            var expectedFighterHealth = initialFighterHealth - (damageAmount - fighterMagicalResistance);

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[damageMagicalElement] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_HigherResistanceToElement_Test()
        {
            // Arrange
            var damageAmount = 10;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            var initialFighterHealth = 50;
            var fighterMagicalResistance = 20;
            var expectedFighterHealth = initialFighterHealth;

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[damageMagicalElement] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_NullAmountAndResistanceToElement_Test()
        {
            // Arrange
            var damageAmount = 0;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            var initialFighterHealth = 50;
            var fighterMagicalResistance = 5;
            var expectedFighterHealth = initialFighterHealth;

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[damageMagicalElement] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_NegativeAmountAndResistanceToElement_Test()
        {
            // Arrange
            var damageAmount = -10;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            var initialFighterHealth = 50;
            var fighterMagicalResistance = 5;
            var expectedFighterHealth = initialFighterHealth;

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[damageMagicalElement] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_MinHealthCap_Test()
        {
            // Arrange
            var damageAmount = 10000;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            var initialFighterHealth = 50;
            var fighterMagicalResistance = 20;
            var expectedFighterHealth = 0;

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[damageMagicalElement] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_DifferentResistanceElement_Test()
        {
            // Arrange
            var damageAmount = 10;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            var initialFighterHealth = 50;
            var fighterMagicalResistance = 20;
            var expectedFighterHealth = initialFighterHealth - damageAmount;

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[EMagicalElement.FIRE] = 0;
            fighter.GetStatsForTests().magicalResistances[EMagicalElement.ICE] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_NotSpecifiedResistanceElement_Test()
        {
            // Arrange
            var damageAmount = 10;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            var initialFighterHealth = 50;
            var expectedFighterHealth = initialFighterHealth - damageAmount;

            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances.Remove(EMagicalElement.FIRE);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() =>
                fighter.MagicalWithstand(damageAmount, damageMagicalElement, false));
        }
    }
}