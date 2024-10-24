using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterStatusAbilitiesTests
    {
        Fighter fighter;
        StatusesManager statusManager;
        BleedStatus bleedStatus;
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
            bleedStatus = ScriptableObject.CreateInstance<BleedStatus>();
            weaknessStatus = ScriptableObject.CreateInstance<WeaknessStatus>();
            paralysisStatus = ScriptableObject.CreateInstance<ParalysisStatus>();

            bleedStatus.SetIsRecurring(true);
            bleedStatus.SetDuration(3);
            weaknessStatus.SetIsRecurring(false);
            weaknessStatus.SetDuration(3);
            paralysisStatus.SetDuration(3);
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
            for (int i = 0; i < bleedStatus.Duration; i++)
            {
                statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);
            }

            // Assert
            Assert.AreEqual(initialHealth - (bleedStatus.Duration * expectedHealthReduction), fighter.GetHealth());
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
            {
                statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);
            }
            statusManager.ApplyStatus(bleedStatus);
            statusManager.UpdateStatuses(EStatusTriggerTime.StartOfTurn);


            // Assert
            Dictionary<AStatus, (bool isActive, int duration)> statusEffects = statusManager.GetStatuses();
            var (isActive, duration) = statusEffects[bleedStatus];
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