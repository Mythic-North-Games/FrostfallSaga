using System;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.GameItems;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;

namespace FrostfallSaga.Fight
{
    [Serializable]
    public class FighterSetup
    {
        public GameObject fighterPrefab;
        public string name;
        public string sessionId;
        public EntityID entityID;
        public Sprite icon;
        public Sprite diamondIcon;
        public FighterStats initialStats;
        public FighterClassSO fighterClass;
        public PersonalityTraitSO personalityTrait;
        public Inventory inventory;
        public AAbilityAnimationSO directAttackAnimation;
        public ActiveAbilitySO[] activeAbilities;
        public PassiveAbilitySO[] passiveAbilities;
        public string receiveDamageAnimationName;
        public string healSelfAnimationName;
        public string reduceStatAnimationName;
        public string increaseStatAnimationName;

        public FighterSetup(
            GameObject fighterPrefab,
            string name,
            string sessionId,
            EntityID entityID,
            Sprite icon,
            Sprite diamondIcon,
            FighterStats initialStats,
            FighterClassSO fighterClass,
            PersonalityTraitSO personalityTrait,
            Inventory inventory,
            AAbilityAnimationSO directAttackAnimation,
            ActiveAbilitySO[] activeAbilities,
            PassiveAbilitySO[] passiveAbilities,
            string receiveDamageAnimationName,
            string healSelfAnimationName,
            string reduceStatAnimationName,
            string increaseStatAnimationName
        )
        {
            this.fighterPrefab = fighterPrefab;
            this.name = name;
            this.sessionId = sessionId;
            this.entityID = entityID;
            this.icon = icon;
            this.diamondIcon = diamondIcon;
            this.initialStats = initialStats;
            this.fighterClass = fighterClass;
            this.personalityTrait = personalityTrait;
            this.inventory = inventory;
            this.directAttackAnimation = directAttackAnimation;
            this.activeAbilities = activeAbilities;
            this.passiveAbilities = passiveAbilities;
            this.receiveDamageAnimationName = receiveDamageAnimationName;
            this.healSelfAnimationName = healSelfAnimationName;
            this.reduceStatAnimationName = reduceStatAnimationName;
            this.increaseStatAnimationName = increaseStatAnimationName;
        }
    }
}