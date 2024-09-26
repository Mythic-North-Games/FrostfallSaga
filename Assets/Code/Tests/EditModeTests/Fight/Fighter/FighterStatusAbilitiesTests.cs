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

        [SetUp]
        public void Setup()
        {
            fighter = FightTestsHelper.CreateFighter();
            statusManager = fighter.GetComponent<StatusEffectManager>();
            Assert.IsNotNull(fighter, "Le fighter n'est pas initialisé");
            Assert.IsNotNull(statusManager, "Le StatusEffectManager n'est pas initialisé");
        }

        [Test]
        public void ApplyBleedingStatus_ShouldReduceHealthOverTime()
        {
            // arrange
            var bleedingStatus = new BleedingStatus();
            int initialHealth = fighter.GetHealth();
            int expectedHealthReduction = 5;

            // Act
            statusManager.ApplyEffect(bleedingStatus);
            statusManager.UpdateStatusEffects();

            // Assert
            Assert.AreEqual(initialHealth - expectedHealthReduction, fighter.GetHealth());
        }

        [Test]
        public void ApplyBleedingStatus_ShouldStopAfterDuration()
        {
            // arrange
            var bleedingStatus = new BleedingStatus();
            statusManager.ApplyEffect(bleedingStatus);
            int initialHealth = fighter.GetHealth();
            int expectedHealthReduction = 5;

            // Act
            for (int i = 0; i < bleedingStatus.Duration; i++)
            {
                statusManager.UpdateStatusEffects();
            }

            // Assert
            Assert.AreEqual(initialHealth - (bleedingStatus.Duration * expectedHealthReduction), fighter.GetHealth());
            Dictionary<StatusEffect, (bool isActive, int duration)> statusEffects = statusManager.getStatusEffects();
            var isActive = statusEffects.ContainsKey(bleedingStatus);
            Assert.IsFalse(isActive);
        }

        [Test]
        public void ApplyBleedingStatusTwice_ShouldNotStopAfterDurationOfTheFirst()
        {
            // arrange
            var bleedingStatus = new BleedingStatus();
            statusManager.ApplyEffect(bleedingStatus);


            // Act
            for (int i = 0; i < bleedingStatus.Duration - 1; i++)
            {
                statusManager.UpdateStatusEffects();
            }
            statusManager.ApplyEffect(bleedingStatus);
            statusManager.UpdateStatusEffects();


            // Assert
            Dictionary<StatusEffect, (bool isActive, int duration)> statusEffects = statusManager.getStatusEffects();
            var (isActive, duration) = statusEffects[bleedingStatus];
            Assert.IsTrue(isActive);
        }

        [Test]
        public void ApplyWeakeningStatus_ShouldReduceStrengthOnce()
        {
            // arrange
            var weaknessStatus = new WeaknessStatus();
            int initialStrength = fighter.GetStrength();
            int expectedStrengthReduction = 10;

            // Act
            statusManager.ApplyEffect(weaknessStatus);
            statusManager.UpdateStatusEffects();

            // Assert
            Assert.AreEqual(initialStrength - expectedStrengthReduction, fighter.GetStrength());
        }

        [Test]
        public void ApplyWeakeningStatus_StrengthShouldBeBackToNormalAfterDuration()
        {
            // arrange
            var weaknessStatus = new WeaknessStatus();
            int initialStrength = fighter.GetStrength();
            int expectedStrengthReduction = 10;

            // Act
            statusManager.ApplyEffect(weaknessStatus);
            for (int i = 0; i < weaknessStatus.Duration - 1; i++)
            {
                statusManager.UpdateStatusEffects();
            }

            // Assert
            Assert.AreEqual(initialStrength - expectedStrengthReduction, fighter.GetStrength());

            // Act 2
            statusManager.UpdateStatusEffects();

            // Assert 2
            Assert.AreEqual(initialStrength, fighter.GetStrength());
            Dictionary<StatusEffect, (bool isActive, int duration)> statusEffects = statusManager.getStatusEffects();
            var isActive = statusEffects.ContainsKey(weaknessStatus);
            Assert.IsFalse(isActive);
        }

        [Test]
        public void ApplyParalysisStatus_ShouldSkipFighterTurn()
        {
            // arrange
            var paralysisStatus = new ParalysisStatus();

            // Act
            statusManager.ApplyEffect(paralysisStatus);
            statusManager.UpdateStatusEffects();

            // Assert
            Assert.IsTrue(fighter.IsParalyzed);
        }


        [Test]
        public void ApplyParalysisStatus_ShouldUnparalyzedFighterAfterDuration()
        {
            // arrange
            var paralysisStatus = new ParalysisStatus();

            // Act
            statusManager.ApplyEffect(paralysisStatus);
            for (int i = 0; i < paralysisStatus.Duration - 1; i++)
            {
                statusManager.UpdateStatusEffects();
            }

            //Assert
            Assert.IsTrue(fighter.IsParalyzed);

            // Act 2 
            statusManager.UpdateStatusEffects();

            // Assert 2 
            Assert.IsFalse(fighter.IsParalyzed);
        }
    }
}