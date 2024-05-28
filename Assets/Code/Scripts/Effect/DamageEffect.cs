using UnityEngine;

namespace Code.Scripts.Effect
{
    public class DamageEffect : AEffect
    {
        [SerializeField] public int DamageAmount { get; private set; }

        public override void ApplyEffect()
        {
            throw new System.NotImplementedException();
        }
    }
}
