using System;
using UnityEngine;

namespace FrostfallSaga.Core.Cities
{
    [Serializable]
    public class TavernConfiguration
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int RestCost { get; private set; }
        [field: SerializeField] public Sprite TavernIllustration { get; private set; }
    }
}