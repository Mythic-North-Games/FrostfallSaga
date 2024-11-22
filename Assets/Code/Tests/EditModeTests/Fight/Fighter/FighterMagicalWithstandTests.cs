using System;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using NUnit.Framework;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterMagicalWithstandTests
    {
        [Test]
        public void MagicalWithstand_PositiveAmountAndResistanceToElement_Test()
        {
            // Arrange
            int damageAmount = 10;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            int initialFighterHealth = 50;
            int fighterMagicalResistance = 5;
            int expectedFighterHealth = initialFighterHealth - (damageAmount - fighterMagicalResistance);

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[damageMagicalElement] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_HigherResistanceToElement_Test()
        {
            // Arrange
            int damageAmount = 10;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            int initialFighterHealth = 50;
            int fighterMagicalResistance = 20;
            int expectedFighterHealth = initialFighterHealth;

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[damageMagicalElement] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_NullAmountAndResistanceToElement_Test()
        {
            // Arrange
            int damageAmount = 0;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            int initialFighterHealth = 50;
            int fighterMagicalResistance = 5;
            int expectedFighterHealth = initialFighterHealth;

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[damageMagicalElement] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_NegativeAmountAndResistanceToElement_Test()
        {
            // Arrange
            int damageAmount = -10;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            int initialFighterHealth = 50;
            int fighterMagicalResistance = 5;
            int expectedFighterHealth = initialFighterHealth;

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[damageMagicalElement] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_MinHealthCap_Test()
        {
            // Arrange
            int damageAmount = 10000;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            int initialFighterHealth = 50;
            int fighterMagicalResistance = 20;
            int expectedFighterHealth = 0;

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[damageMagicalElement] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_DifferentResistanceElement_Test()
        {
            // Arrange
            int damageAmount = 10;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            int initialFighterHealth = 50;
            int fighterMagicalResistance = 20;
            int expectedFighterHealth = initialFighterHealth - damageAmount;

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances[EMagicalElement.FIRE] = 0;
            fighter.GetStatsForTests().magicalResistances[EMagicalElement.WATER] = fighterMagicalResistance;

            // Act
            fighter.MagicalWithstand(damageAmount, damageMagicalElement);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }

        [Test]
        public void MagicalWithstand_NotSpecifiedResistanceElement_Test()
        {
            // Arrange
            int damageAmount = 10;
            EMagicalElement damageMagicalElement = EMagicalElement.FIRE;
            int initialFighterHealth = 50;
            int expectedFighterHealth = initialFighterHealth - damageAmount;

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().magicalResistances.Remove(EMagicalElement.FIRE);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => fighter.MagicalWithstand(damageAmount, damageMagicalElement));
        }
    }
}