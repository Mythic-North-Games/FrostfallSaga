using System;
using UnityEngine;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;

namespace FrostfallSaga.Fight
{
    [Serializable]
    public class FighterSetup
    {
        public string name;
        public string sessionId;
        public Sprite icon;
        public FighterStats initialStats;
        public FighterClassSO fighterClass;
        public PersonalityTraitSO personalityTrait;
        public Targeter directAttackTargeter;
        public int directAttackActionPointsCost;
        [SerializeReference] public AEffect[] directAttackEffects = { };
        public AAbilityAnimationSO directAttackAnimation;
        public ActiveAbilityToAnimation[] activeAbilities;
        public string receiveDamageAnimationName;
        public string healSelfAnimationName;
        public string reduceStatAnimationName;
        public string increaseStatAnimationName;

        public FighterSetup(
            string name,
            string sessionId,
            Sprite fighterIcon,
            FighterStats initialStats,
            FighterClassSO fighterClass,
            PersonalityTraitSO personalityTrait,
            Targeter directAttackTargeter,
            int directAttackActionPointsCost,
            AEffect[] directAttackEffects,
            AAbilityAnimationSO directAttackAnimation,
            ActiveAbilityToAnimation[] activeAbilities,
            string receiveDamageAnimationName,
            string healSelfAnimationName,
            string reduceStatAnimationName,
            string increaseStatAnimationName
        )
        {
            this.name = name;
            this.sessionId = sessionId;
            this.icon = fighterIcon;
            this.initialStats = initialStats;
            this.fighterClass = fighterClass;
            this.personalityTrait = personalityTrait;
            this.directAttackTargeter = directAttackTargeter;
            this.directAttackActionPointsCost = directAttackActionPointsCost;
            this.directAttackEffects = directAttackEffects;
            this.directAttackAnimation = directAttackAnimation;
            this.activeAbilities = activeAbilities;
            this.receiveDamageAnimationName = receiveDamageAnimationName;
            this.healSelfAnimationName = healSelfAnimationName;
            this.reduceStatAnimationName = reduceStatAnimationName;
            this.increaseStatAnimationName = increaseStatAnimationName;
        }
    }
}