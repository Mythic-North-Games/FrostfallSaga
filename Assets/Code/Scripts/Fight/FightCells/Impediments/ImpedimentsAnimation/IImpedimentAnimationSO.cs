using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
      [CreateAssetMenu(
        fileName = "ImpedimentAnimation",
        menuName = "ScriptableObjects/Fight/Impediments/Animations/ImpedimentAnimation",
        order = 0
    )]
    public  class IImpedimentAnimationSO : ScriptableObject
    {
    
        [SerializeReference] public StaticObjectsExecutor Executor;
        [field: SerializeField] public GameObject Prefab { get; private set; }

        /// <summary>
        /// Executes the animation as configured.
        /// </summary>
        public void Execute(Fighter fighterThatWillExecute, FightCell TargetCell)
        {
            FightCell[] targetCells={TargetCell};
            Executor.Execute(fighterThatWillExecute, targetCells, Prefab);
        }

   
    }
}