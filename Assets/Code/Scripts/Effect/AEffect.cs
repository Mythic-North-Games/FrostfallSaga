using UnityEngine;

namespace Code.Scripts.Effect
{
    public abstract class AEffect
    {
        [SerializeField] private string _name;
        [SerializeField] private string _description;

        public string Name
        {
            get => _name;
        }

        public string Description
        {
            get => _description;
        }

        public abstract void ApplyEffect(); // Fighter fighter
    }
}