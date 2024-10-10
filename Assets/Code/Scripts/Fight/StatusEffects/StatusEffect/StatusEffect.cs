using UnityEngine;
using System.Collections.Generic;
using System;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.StatusEffects
{
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "Status Effect/Base")]
    public class StatusEffect : ScriptableObject
    {
        [SerializeField] private string name = "Status Effect";
        [SerializeField] private string description = "Status effect description";
        [SerializeField] private int duration = 3;
        [SerializeField] private bool isRecurring = true;
        [SerializeField] private string animationStateName = "Animation";

        public virtual void ApplyStatusEffect(Fighter fighter) { }
        public virtual void RemoveStatusEffect(Fighter fighter) { }



        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public bool IsRecurring
        {
            get { return isRecurring; }
            set { isRecurring = value; }
        }

        public string AnimationStateName
        {
            get { return animationStateName; }
            set { animationStateName = value; }
        }
    }

}