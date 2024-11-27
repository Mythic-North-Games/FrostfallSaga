using FrostfallSaga.Fight.Fighters;
using NUnit.Framework;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterHealTests
    {
        [Test]
        public void Heal_PositiveAmountAdded_Test()
        {
            // Arrange
            int healAmount = 10;
            int initialFighterHealth = 50;
            int expectedFighterHealth = initialFighterHealth + healAmount;

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
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

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
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

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
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

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
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

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
            fighter.GetStatsForTests().health = initialFighterHealth;


            // Act
            fighter.Heal(healAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }
    }
}