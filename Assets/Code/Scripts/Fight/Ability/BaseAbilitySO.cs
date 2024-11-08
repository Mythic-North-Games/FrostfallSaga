using UnityEngine;
using System.Linq;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;


namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    /// Abilities common fields.
    /// </summary>
    public abstract class BaseAbilitySO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite IconSprite { get; private set; }
        [SerializeReference] public AEffect[] Effects = { };

        public int GetDamagesPotential(Fighter initiator, Fighter target)
        {
            return Effects.Sum(
                effect => effect.GetPotentialEffectDamages(initiator, target, effect.Masterstrokable)
            );
        }
    }
}