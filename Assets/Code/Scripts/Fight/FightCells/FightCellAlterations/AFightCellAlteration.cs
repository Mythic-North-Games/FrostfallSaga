using System;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [Serializable]
    public abstract class AFightCellAlteration
    {
        [field: SerializeField, Header("For the UI")] public string Name { get; protected set; }
        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public Sprite Icon { get; protected set; }

        [field: SerializeField, Header("Global alteration definiton")] public bool IsPermanent { get; protected set; }
        [field: SerializeField] public int Duration { get; protected set; }
        public bool CanBeReplaced { get; protected set; }
        [field: SerializeField] public  bool CanApplyWithFighter  { get; protected set; }

        public Action<FightCell, AFightCellAlteration> onAlterationApplied;
        public Action<FightCell, AFightCellAlteration> onAlterationRemoved;

        public abstract void Apply(FightCell cell);
        public abstract void Remove(FightCell cell);
    }
}