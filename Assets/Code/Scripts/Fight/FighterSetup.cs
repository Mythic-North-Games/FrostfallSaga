using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Targeters;

namespace FrostfallSaga.Fight
{
    public class FighterSetup
    {
        public string name;
        public FighterStats initialStats;
        public TargeterSO directAttackTargeter;
        public int directAttackActionPointsCost;
        public AEffectSO[] directAttackEffects;
        public string directAttackAnimationStateName;
        public ActiveAbilityToAnimation[] activeAbilities;
        public string receiveDamageAnimationStateName;
        public string healSelfAnimationStateName;

        public FighterSetup()
        {
        }

        public FighterSetup(
            string name,
            FighterStats initialStats,
            TargeterSO directAttackTargeter,
            int directAttackActionPointsCost,
            AEffectSO[] directAttackEffects,
            string directAttackAnimationStateName,
            ActiveAbilityToAnimation[] activeAbilities,
            string receiveDamageAnimationStateName,
            string healSelfAnimationStateName
        )
        {
            this.name = name;
            this.initialStats = initialStats;
            this.directAttackTargeter = directAttackTargeter;
            this.directAttackActionPointsCost = directAttackActionPointsCost;
            this.directAttackEffects = directAttackEffects;
            this.directAttackAnimationStateName = directAttackAnimationStateName;
            this.activeAbilities = activeAbilities;
            this.receiveDamageAnimationStateName = receiveDamageAnimationStateName;
            this.healSelfAnimationStateName = healSelfAnimationStateName;
        }
    }
}