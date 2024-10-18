using NUnit.Framework;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using System.Collections.Generic;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterStatusAbilitiesTests
    {
        Fighter fighter;
        StatusesManager statusManager;
        BleedingStatus bleedingStatus;
        WeaknessStatus weaknessStatus;
        ParalysisStatus paralysisStatus;


        [SetUp]
        public void Setup()
        {
            fighter = FightTestsHelper.CreateFighter();
            fighter.name = "Sofiane";
            fighter.GetStatsForTests().strength = 40;
            statusManager = fighter.StatusesManager;
            Assert.IsNotNull(fighter, "Le fighter n'est pas initialisé");
            Assert.IsNotNull(statusManager, "Le StatusEffectManager n'est pas initialisé");
            bleedingStatus = ScriptableObject.CreateInstance<BleedingStatus>();
            weaknessStatus = ScriptableObject.CreateInstance<WeaknessStatus>();
            paralysisStatus = ScriptableObject.CreateInstance<ParalysisStatus>();

            bleedingStatus.SetIsRecurring(true);
            bleedingStatus.SetDuration(3);
            weaknessStatus.SetIsRecurring(false);
            weaknessStatus.SetDuration(3);
            paralysisStatus.SetDuration(3);
        }

        [Test]
        public void ApplyBleedingStatus_ShouldReduceHealthOverTime()
        {
            // arrange
            int initialHealth = fighter.GetHealth();
            int expectedHealthReduction = bleedingStatus.BleedingDamage;

            // Act
            statusManager.ApplyStatus(bleedingStatus);
            statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            // Assert
            Assert.AreEqual(initialHealth - expectedHealthReduction, fighter.GetHealth());
        }
        
        [Test]
          public void ApplyBleedingStatus_ShouldNotReduceHealthOverTime()
        {
            // arrange
            int initialHealth = fighter.GetHealth();

            // Act
            statusManager.ApplyStatus(bleedingStatus);
            statusManager.UpdateStatuses(EStatusTriggerTime.EndOfTurn);

            // Assert
            Assert.AreEqual(initialHealth, fighter.GetHealth());
        }

        [Test]
        public void ApplyBleedingStatus_ShouldStopAfterDuration()
        {
            // arrange
            statusManager.ApplyStatus(bleedingStatus);
            int initialHealth = fighter.GetHealth();
            int expectedHealthReduction = bleedingStatus.BleedingDamage;

            // Act
            for (int i = 0; i < bleedingStatus.Duration; i++)
            {
                statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);
            }

            // Assert
            Assert.AreEqual(initialHealth - (bleedingStatus.Duration * expectedHealthReduction), fighter.GetHealth());
            Dictionary<AStatus, (bool isActive, int duration)> statusEffects = statusManager.GetStatusEffects();
            bool isActive = statusEffects.ContainsKey(bleedingStatus);
            Assert.IsFalse(isActive);
        }

        [Test]
        public void ApplyBleedingStatusTwice_ShouldNotStopAfterDurationOfTheFirst()
        {
            // arrange
            statusManager.ApplyStatus(bleedingStatus);


            // Act
            for (int i = 0; i < bleedingStatus.Duration - 1; i++)
            {
                statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);
            }
            statusManager.ApplyStatus(bleedingStatus);
            statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);


            // Assert
            Dictionary<AStatus, (bool isActive, int duration)> statusEffects = statusManager.GetStatusEffects();
            var (isActive, duration) = statusEffects[bleedingStatus];
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
            {
                statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);
            }

            // Assert
            Assert.AreEqual(initialStrength - expectedStrengthReduction, fighter.GetStrength());

            // Act 2
            statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            // Assert 2
            Assert.AreEqual(initialStrength, fighter.GetStrength());
            Dictionary<AStatus, (bool isActive, int duration)> statusEffects = statusManager.GetStatusEffects();
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
            {
                statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);
            }

            //Assert
            Assert.IsTrue(fighter.IsParalyzed);

            // Act 2 
            statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);

            // Assert 2 
            Assert.IsFalse(fighter.IsParalyzed);
        }
    }
}