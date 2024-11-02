using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.Impediments
{
    public abstract class AImpedimentSO : ScriptableObject
    {
        [field: SerializeField, Header("Impediment definition")] public GameObject Prefab { get; private set; }
        [field: SerializeField] public bool Destroyable { get; private set; } = true;

        [field: SerializeField, Header("For the UI")] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        public virtual void ApplyOnCell(FightCell fightCell)
        {
            fightCell.SetImpediment(this, Instantiate(Prefab, fightCell.transform));
        }

        public virtual void Destroy(FightCell fightCell)
        {
            fightCell.SetImpediment(null, null);
        }
    }
}