using UnityEngine;

namespace Code.Scripts.Effect
{
    public class HealEffect : AEffect
    {
        [SerializeField] public int HealAmount { get; private set; }

        public override void ApplyEffect()
        {
            throw new System.NotImplementedException();
        }
    }
}
