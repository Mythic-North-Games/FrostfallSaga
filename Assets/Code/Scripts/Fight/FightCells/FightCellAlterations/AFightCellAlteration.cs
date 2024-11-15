using System;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [Serializable]
    public abstract class AFightCellAlteration
    {
        [field: SerializeField, Header("Global alteration definiton")] public bool IsPermanent { get; protected set; }
        [field: SerializeField] public int Duration { get; protected set; }
        public bool CanBeReplaced { get; protected set; }
        public bool CanApplyWithFighter { get; protected set; }

        [field: SerializeField, Header("For the UI")] public string Name { get; protected set; }
        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public Sprite Icon { get; protected set; }

        public Action<FightCell, AFightCellAlteration> onAlterationApplied;
        public Action<FightCell, AFightCellAlteration> onAlterationRemoved;

        public AFightCellAlteration() { }

        public AFightCellAlteration(
            bool isPermanent,
            int duration,
            bool canBeReplaced,
            bool canApplyWithFighter,
            string name,
            string description,
            Sprite icon
        )
        {
            IsPermanent = isPermanent;
            Duration = duration;
            CanBeReplaced = canBeReplaced;
            CanApplyWithFighter = canApplyWithFighter;
            Name = name;
            Description = description;
            Icon = icon;
        }

        public abstract void Apply(FightCell cell);
        public abstract void Remove(FightCell cell);
    }
}