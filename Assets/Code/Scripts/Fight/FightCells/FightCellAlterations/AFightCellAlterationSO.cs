using System;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    public abstract class AFightCellAlterationSO : ScriptableObject
    {
        [field: SerializeField, Header("Global alteration definiton")] public bool IsPermanent { get; private set; }
        [field: SerializeField] public int Duration { get; private set; }

        [field: SerializeField, Header("For the UI")] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        
        public Action<FightCell> onAlterationApplied;
        public Action<FightCell> onAlterationRemoved;

        public abstract void Apply(FightCell cell);
        public abstract void Remove(FightCell cell);
    }
}