using UnityEngine;

namespace Code.Scripts.Effect
{
    public class DamageEffect : AEffect
    {
        [SerializeField] private int _damageAmount;

        public int DamageAmount
        {
            get => _damageAmount;
        }

        public override void ApplyEffect()
        {
            throw new System.NotImplementedException();
        }
    }
}