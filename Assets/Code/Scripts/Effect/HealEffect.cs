using UnityEngine;

namespace Code.Scripts.Effect
{
    public class HealEffect : AEffect
    {
        [SerializeField] private int _healAmount;

        public int HealAmount
        {
            get => _healAmount;
        }
        /**
         µ
         */
        public override void ApplyEffect()
        {
            throw new System.NotImplementedException();

        }
    }
}