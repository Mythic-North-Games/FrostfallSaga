using UnityEngine;
using FrostfallSaga.Core.Quests;

namespace FrostfallSaga.Core.Cities.CitySituations
{
    public abstract class ACitySituationSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Illustration { get; private set; }

        [field: SerializeField, Tooltip("Optional quest to be added on situation start")]
        public AQuestSO OnStartQuest { get; private set; }

        [field: SerializeField, Tooltip("Optional quest to be added on situation end")]
        public AQuestSO OnEndQuest { get; private set; }
    }
}