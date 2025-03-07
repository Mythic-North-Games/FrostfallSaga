using System;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [Serializable]
    public abstract class AFightCellAlteration
    {
        [field: SerializeField]
        [field: Header("For the UI")]
        public string Name { get; protected set; }

        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public Sprite Icon { get; protected set; }

        [field: SerializeField]
        [field: Header("Global alteration definiton")]
        public bool IsPermanent { get; protected set; }

        [field: SerializeField] public int Duration { get; protected set; }

        public Action<FightCell, AFightCellAlteration> onAlterationApplied;
        public Action<FightCell, AFightCellAlteration> onAlterationRemoved;

        public AFightCellAlteration(
            string name,
            string description,
            Sprite icon,
            bool isPermanent,
            int duration,
            bool canBeReplaced,
            bool canApplyWithFighter
        )
        {
            Name = name;
            Description = description;
            Icon = icon;
            IsPermanent = isPermanent;
            Duration = duration;
            CanBeReplaced = canBeReplaced;
            CanApplyWithFighter = canApplyWithFighter;
        }

        public bool CanBeReplaced { get; protected set; }
        public bool CanApplyWithFighter { get; protected set; }

        public abstract void Apply(FightCell cell);
        public abstract void Remove(FightCell cell);
    }
}