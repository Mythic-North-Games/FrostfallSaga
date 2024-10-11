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
        public TargeterSO directAttackTargeter;
        public int directAttackActionPointsCost;
        public AEffectSO[] directAttackEffects;
        public AAbilityAnimationSO directAttackAnimation;
        public ActiveAbilityToAnimation[] activeAbilities;
        public string receiveDamageAnimationStateName;
        public string healSelfAnimationStateName;
        public FighterClassSO fighterClassSO;

        public FighterSetup(
            string name,
            string sessionId,
            Sprite fighterIcon,
            FighterStats initialStats,
            TargeterSO directAttackTargeter,
            int directAttackActionPointsCost,
            AEffectSO[] directAttackEffects,
            AAbilityAnimationSO directAttackAnimation,
            ActiveAbilityToAnimation[] activeAbilities,
            string receiveDamageAnimationStateName,
            string healSelfAnimationStateName,
            FighterClassSO fighterClassSO
        ) {
            this.name = name;
            this.sessionId = sessionId;
            this.icon = fighterIcon;
            this.initialStats = initialStats;
            this.directAttackTargeter = directAttackTargeter;
            this.directAttackActionPointsCost = directAttackActionPointsCost;
            this.directAttackEffects = directAttackEffects;
            this.directAttackAnimation = directAttackAnimation;
            this.activeAbilities = activeAbilities;
            this.receiveDamageAnimationStateName = receiveDamageAnimationStateName;
            this.healSelfAnimationStateName = healSelfAnimationStateName;
            this.fighterClassSO = fighterClassSO;
        }
    }
}