using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using NUnit.Framework;
using UnityEngine;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterStatusAbilitiesTests
    {
        private BleedStatus bleedStatus;
        private Fighter fighter;
        private ParalysisStatus paralysisStatus;
        private StatusesManager statusManager;
        private WeaknessStatus weaknessStatus;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            bleedStatus = new BleedStatus(
                false,
                3,
                false,
                true,
                EStatusTriggerTime.StartOfTurn,
                null,
                10
            );
            weaknessStatus = new WeaknessStatus(
                false,
                3,
                false,
                false,
                EStatusTriggerTime.StartOfTurn,
                null,
                10
            );
            paralysisStatus = new ParalysisStatus(
                false,
                3,
                false,
                false,
                EStatusTriggerTime.StartOfTurn,
                null
            );
        }

        [SetUp]
        public void Setup()
        {
            fighter = FightTestsHelper.CreateFighter();
            fighter.name = "Sofiane";
            fighter.GetStatsForTests().strength = 40;
            statusManager = fighter.StatusesManager;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(fighter.gameObject);
            fighter = null;
            statusManager = null;
        }

        [Test]
        public void ApplyBleedStatus_ShouldReduceHealthOverTime()
        {
            // arrange
            int initialHealth = fighter.GetHealth();
            int expectedHealthReduction = bleedStatus.BleedingDamage;

            // Act
            statusManager.ApplyStatus(bleedStatus);
            statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            // Assert
            Assert.AreEqual(initialHealth - expectedHealthReduction, fighter.GetHealth());
        }

        [Test]
        public void ApplyBleedStatus_ShouldNotReduceHealthOverTime()
        {
            // arrange
            int initialHealth = fighter.GetHealth();

            // Act
            statusManager.ApplyStatus(bleedStatus);
            statusManager.UpdateStatuses(EStatusTriggerTime.EndOfTurn);

            // Assert
            Assert.AreEqual(initialHealth, fighter.GetHealth());
        }

        [Test]
        public void ApplyBleedStatus_ShouldStopAfterDuration()
        {
            // arrange
            statusManager.ApplyStatus(bleedStatus);
            int initialHealth = fighter.GetHealth();
            int expectedHealthReduction = bleedStatus.BleedingDamage;

            // Act
            for (int i = 0; i < bleedStatus.Duration; i++) statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            // Assert
            Assert.AreEqual(initialHealth - bleedStatus.Duration * expectedHealthReduction, fighter.GetHealth());
            Dictionary<AStatus, (bool isActive, int duration)> statusEffects = statusManager.GetStatuses();
            bool isActive = statusEffects.ContainsKey(bleedStatus);
            Assert.IsFalse(isActive);
        }

        [Test]
        public void ApplyBleedStatusTwice_ShouldNotStopAfterDurationOfTheFirst()
        {
            // arrange
            statusManager.ApplyStatus(bleedStatus);


            // Act
            for (int i = 0; i < bleedStatus.Duration - 1; i++)
                statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);
            statusManager.ApplyStatus(bleedStatus);
            statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);


            // Assert
            Dictionary<AStatus, (bool isActive, int duration)> statusEffects = statusManager.GetStatuses();
            (bool isActive, int duration) = statusEffects[bleedStatus];
            Assert.IsTrue(isActive);
        }

        [Test]
        public void ApplyWeakeningStatus_ShouldReduceStrengthOnce()
        {
            // arrange
            int initialStrength = fighter.GetStrength();
            int expectedStrengthReduction = weaknessStatus.StrengthReduction;

            // Act
            statusManager.ApplyStatus(weaknessStatus);
            statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            // Assert
            Assert.AreEqual(initialStrength - expectedStrengthReduction, fighter.GetStrength());
        }

        [Test]
        public void ApplyWeakeningStatus_StrengthShouldBeBackToNormalAfterDuration()
        {
            // arrange
            int initialStrength = fighter.GetStrength();
            int expectedStrengthReduction = weaknessStatus.StrengthReduction;

            // Act
            statusManager.ApplyStatus(weaknessStatus);
            for (int i = 0; i < weaknessStatus.Duration - 1; i++)
                statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            // Assert
            Assert.AreEqual(initialStrength - expectedStrengthReduction, fighter.GetStrength());

            // Act 2
            statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            // Assert 2
            Assert.AreEqual(initialStrength, fighter.GetStrength());
            Dictionary<AStatus, (bool isActive, int duration)> statusEffects = statusManager.GetStatuses();
            bool isActive = statusEffects.ContainsKey(weaknessStatus);
            Assert.IsFalse(isActive);
        }

        [Test]
        public void ApplyParalysisStatus_ShouldSkipFighterTurn()
        {
            // Act
            statusManager.ApplyStatus(paralysisStatus);
            statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            // Assert
            Assert.IsTrue(fighter.IsParalyzed);
        }


        [Test]
        public void ApplyParalysisStatus_ShouldUnparalyzedFighterAfterDuration()
        {
            // Act
            statusManager.ApplyStatus(paralysisStatus);
            for (int i = 0; i < paralysisStatus.Duration - 1; i++)
                statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            //Assert
            Assert.IsTrue(fighter.IsParalyzed);

            // Act 2 
            statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            // Assert 2 
            Assert.IsFalse(fighter.IsParalyzed);
        }
    }
}