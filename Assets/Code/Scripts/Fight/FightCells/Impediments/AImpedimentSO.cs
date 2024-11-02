using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.Impediments
{
    public abstract class AImpedimentSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public bool Destroyable { get; private set; }

        public abstract void ApplyOnCell(FightCell cell);
        public abstract void Destroy(FightCell fightCell);
    }
}