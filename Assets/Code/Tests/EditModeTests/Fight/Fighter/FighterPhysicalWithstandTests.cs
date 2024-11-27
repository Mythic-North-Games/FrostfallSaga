using FrostfallSaga.Fight.Fighters;
using NUnit.Framework;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterPhysicalWithstandTests
    {
        [Test]
        public void PhysicalWithstand_PositiveAmountAdded_Test()
        {
            // Arrange
            int damageAmount = 10;
            int initialFighterHealth = 50;
            int fighterPhysicalResistance = 5;
            int expectedFighterHealth = initialFighterHealth - (damageAmount - fighterPhysicalResistance);

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
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
            int damageAmount = 10;
            int initialFighterHealth = 50;
            int fighterPhysicalResistance = 20;
            int expectedFighterHealth = initialFighterHealth;

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
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
            int damageAmount = 0;
            int initialFighterHealth = 50;
            int fighterPhysicalResistance = 5;
            int expectedFighterHealth = initialFighterHealth;

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
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
            int damageAmount = -10;
            int initialFighterHealth = 50;
            int fighterPhysicalResistance = 5;
            int expectedFighterHealth = initialFighterHealth;

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
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
            int damageAmount = 1000;
            int initialFighterHealth = 50;
            int fighterPhysicalResistance = 5;
            int expectedFighterHealth = 0;

            Fighter fighter = FightTestsHelper.CreateFighter();
            fighter.SetStatsForTests();
            fighter.GetStatsForTests().health = initialFighterHealth;
            fighter.GetStatsForTests().physicalResistance = fighterPhysicalResistance;

            // Act
            fighter.PhysicalWithstand(damageAmount, false);

            // Assert
            Assert.AreEqual(expectedFighterHealth, fighter.GetStatsForTests().health);
        }
    }
}