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
        public FighterClassSO fighterClassSO;
        public TargeterSO directAttackTargeter;
        public int directAttackActionPointsCost;
        public AEffectSO[] directAttackEffects;
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
            FighterClassSO fighterClassSO,
            TargeterSO directAttackTargeter,
            int directAttackActionPointsCost,
            AEffectSO[] directAttackEffects,
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
            this.fighterClassSO = fighterClassSO;
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