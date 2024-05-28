using UnityEngine;
using Code.Scripts.Targeter;

namespace Code.Scripts.Ability
{
    public class ActiveAbility
    {
        
        [SerializeField] private Targeter.Targeter _targeter;
        [SerializeField] private int _actionsPointsCost;

        public Targeter.Targeter Targeter
        {
            get => _targeter;
        }

        public int ActionsPointsCost
        {
            get => _actionsPointsCost;
        }
    }
}