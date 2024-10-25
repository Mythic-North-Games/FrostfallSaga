using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;
using System.Collections.Generic;
using System.Linq;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController.Actions
{
    public class DamageTargetAction : FBTNode
    {
        private readonly EDamagePreference damagePreference;
        public DamageTargetAction(Fighter possessedFighter, HexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams, EDamagePreference damagePreference) : base(possessedFighter, fightGrid, fighterTeams)
        {
            this.damagePreference = damagePreference;
        }

        public override NodeState Evaluate()
        {
            
            Fighter target = (Fighter)GetSharedData("damageTarget");
            if (target == null) return NodeState.FAILURE;
            List<ActiveAbilitySO> useableAbilities = _possessedFighter.GetActiveAbilities().Where(actibeAbilities => _possessedFighter.CanUseActiveAbility(_fightGrid, actibeAbilities, _fighterTeams, target)).ToList();
            ActiveAbilitySO prefereAbility;

            

            switch (damagePreference)
            {
                case EDamagePreference.RANDOM:
                    prefereAbility = Randomizer.GetRandomElementFromArray(useableAbilities.ToArray());
                    break;

                case EDamagePreference.MAXIMIZE_DAMAGE:
                    prefereAbility = useableAbilities.OrderByDescending(ability => GetPotentialsDamageOfAbilities(ability)).First();

                    if (GetPotentialsDamageOfAbilities(prefereAbility) > GetPotentialsDamageOfDirectAttack(_possessedFighter.DirectAttackEffects.ToList()))
                    {
                        _possessedFighter.UseActiveAbility(_possessedFighter.ActiveAbilitiesToAnimation.Where(ability => ability.activeAbility.Equals(prefereAbility)).First(), _possessedFighter.GetFirstTouchingCellSequence(prefereAbility.Targeter, _possessedFighter, _fightGrid, _fighterTeams));
                    } 
                    else
                    {
                        _possessedFighter.UseDirectAttack(_possessedFighter.GetFirstTouchingCellSequence(_possessedFighter.DirectAttackTargeter, target, _fightGrid, _fighterTeams));
                    }
                    break;
                
                case EDamagePreference.MINIMIZE_COST:
                    prefereAbility = useableAbilities.OrderBy(ability => ability.ActionPointsCost).First();
                    break;
            }
            return NodeState.SUCCESS;
        }

        private int GetPotentialsDamageOfAbilities(ActiveAbilitySO activeAbility)
        {
            return activeAbility.Effects.Sum(effect => effect.GetEffectDamages());
        }

        private int GetPotentialsDamageOfDirectAttack(List<AEffectSO> effects)
        {
            return effects.Sum(effect => effect.GetEffectDamages());
        }
    }
}
