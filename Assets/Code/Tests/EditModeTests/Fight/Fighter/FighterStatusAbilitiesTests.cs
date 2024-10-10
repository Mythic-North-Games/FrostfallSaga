using NUnit.Framework;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.StatusEffects;
using System.Collections.Generic;

namespace FrostfallSaga.EditModeTests.FightTests.FighterTests
{
    public class FighterStatusAbilitiesTests
    {
        Fighter fighter;
        StatusEffectManager statusManager;
        BleedingStatus bleedingStatus;
        WeaknessStatus weaknessStatus;
        ParalysisStatus paralysisStatus;


        [SetUp]
        public void Setup()
        {
            fighter = FightTestsHelper.CreateFighter();
            fighter.name = "Sofiane";
            fighter.GetStatsForTests().strength = 40;
            statusManager = fighter.statusEffectManager;
            Assert.IsNotNull(fighter, "Le fighter n'est pas initialisé");
            Assert.IsNotNull(statusManager, "Le StatusEffectManager n'est pas initialisé");
            bleedingStatus = ScriptableObject.CreateInstance<BleedingStatus>(); ;
            weaknessStatus = ScriptableObject.CreateInstance<WeaknessStatus>(); ;
            paralysisStatus = ScriptableObject.CreateInstance<ParalysisStatus>(); ;

            weaknessStatus.IsRecurring = false;
            paralysisStatus.IsRecurring = false;

        }

        [Test]
        public void ApplyBleedingStatus_ShouldReduceHealthOverTime()
        {
            // arrange
            int initialHealth = fighter.GetHealth();
            int expectedHealthReduction = bleedingStatus.BleedingReduction;

            // Act
            statusManager.ApplyEffect(bleedingStatus);
            statusManager.UpdateStatusEffects(EffectTriggerTime.StartOfCombat);

            // Assert
            Assert.AreEqual(initialHealth - expectedHealthReduction, fighter.GetHealth());
        }
        
        [Test]
          public void ApplyBleedingStatus_ShouldNotReduceHealthOverTime()
        {
            // arrange
            int initialHealth = fighter.GetHealth();
            int expectedHealthReduction = bleedingStatus.BleedingReduction;

            // Act
            statusManager.ApplyEffect(bleedingStatus);
            statusManager.UpdateStatusEffects(EffectTriggerTime.EndOfCombat);

            // Assert
            Assert.AreEqual(initialHealth, fighter.GetHealth());
        }

        [Test]
        public void ApplyBleedingStatus_ShouldStopAfterDuration()
        {
            // arrange
            statusManager.ApplyEffect(bleedingStatus);
            int initialHealth = fighter.GetHealth();
            int expectedHealthReduction = bleedingStatus.BleedingReduction;

            // Act
            for (int i = 0; i < bleedingStatus.Duration; i++)
            {
                statusManager.UpdateStatusEffects(EffectTriggerTime.StartOfCombat);
            }

            // Assert
            Assert.AreEqual(initialHealth - (bleedingStatus.Duration * expectedHealthReduction), fighter.GetHealth());
            Dictionary<StatusEffect, (bool isActive, int duration)> statusEffects = statusManager.getStatusEffects();
            bool isActive = statusEffects.ContainsKey(bleedingStatus);
            Assert.IsFalse(isActive);
        }

        [Test]
        public void ApplyBleedingStatusTwice_ShouldNotStopAfterDurationOfTheFirst()
        {
            // arrange
            statusManager.ApplyEffect(bleedingStatus);


            // Act
            for (int i = 0; i < bleedingStatus.Duration - 1; i++)
            {
                statusManager.UpdateStatusEffects(EffectTriggerTime.StartOfCombat);
            }
            statusManager.ApplyEffect(bleedingStatus);
            statusManager.UpdateStatusEffects(EffectTriggerTime.StartOfCombat);


            // Assert
            Dictionary<StatusEffect, (bool isActive, int duration)> statusEffects = statusManager.getStatusEffects();
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
            statusManager.ApplyEffect(weaknessStatus);
            statusManager.UpdateStatusEffects(EffectTriggerTime.StartOfCombat);

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
            statusManager.ApplyEffect(weaknessStatus);
            for (int i = 0; i < weaknessStatus.Duration - 1; i++)
            {
                statusManager.UpdateStatusEffects(EffectTriggerTime.StartOfCombat);
            }

            // Assert
            Assert.AreEqual(initialStrength - expectedStrengthReduction, fighter.GetStrength());

            // Act 2
            statusManager.UpdateStatusEffects(EffectTriggerTime.StartOfCombat);

            // Assert 2
            Assert.AreEqual(initialStrength, fighter.GetStrength());
            Dictionary<StatusEffect, (bool isActive, int duration)> statusEffects = statusManager.getStatusEffects();
            bool isActive = statusEffects.ContainsKey(weaknessStatus);
            Assert.IsFalse(isActive);
        }

        [Test]
        public void ApplyParalysisStatus_ShouldSkipFighterTurn()
        {

            // Act
            statusManager.ApplyEffect(paralysisStatus);
            statusManager.UpdateStatusEffects(EffectTriggerTime.StartOfCombat);

            // Assert
            Assert.IsTrue(fighter.IsParalyzed);
        }


        [Test]
        public void ApplyParalysisStatus_ShouldUnparalyzedFighterAfterDuration()
        {

            // Act
            statusManager.ApplyEffect(paralysisStatus);
            for (int i = 0; i < paralysisStatus.Duration - 1; i++)
            {
                statusManager.UpdateStatusEffects(EffectTriggerTime.StartOfCombat);
            }

            //Assert
            Assert.IsTrue(fighter.IsParalyzed);

            // Act 2 
            statusManager.UpdateStatusEffects(EffectTriggerTime.StartOfCombat);

            // Assert 2 
            Assert.IsFalse(fighter.IsParalyzed);
        }
    }
}