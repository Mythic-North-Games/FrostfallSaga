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

    [Serializable]
    public class ImpedimentAnimationSO
    {

        [SerializeReference] public StaticObjectsExecutor Executor;
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public Vector2Int[] CellsSequence { get; private set; }

        /// <summary>
        /// Executes the animation as configured.
        /// </summary>
        public void Execute(Fighter fighterThatWillExecute, FightCell[] targetCells)
        {

            Executor.Execute(
                fighterThatWillExecute,
                targetCells,
                Prefab);
        }

    }
}