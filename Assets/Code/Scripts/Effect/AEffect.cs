using UnityEngine;

namespace Code.Scripts.Effect
{
    public abstract class AEffect
    {
        [SerializeField] public string Name { get; private set; }
        [SerializeField] public string Description { get; private set; }

        public abstract void ApplyEffect(); // Fighter fighter
    }
}
