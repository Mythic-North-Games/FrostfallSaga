using System;
using UnityEngine;

namespace FrostfallSaga.Core.Quests
{
    [Serializable]
    public class QuestEnding
    {
        [field: SerializeField] public string ConclusionText { get; private set; }
    }
}