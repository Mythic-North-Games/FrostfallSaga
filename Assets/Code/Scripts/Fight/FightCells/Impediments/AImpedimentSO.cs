using UnityEngine;
using FrostfallSaga.GameObjectVisuals.GameObjectSpawnControllers;
using FrostfallSaga.GameObjectVisuals.GameObjectDestroyControllers;

namespace FrostfallSaga.Fight.FightCells.Impediments
{
    public abstract class AImpedimentSO : ScriptableObject
    {
        [field: SerializeField, Header("Impediment definition")] public GameObject Prefab { get; private set; }
        [field: SerializeField] public bool Destroyable { get; private set; } = true;

        [field: SerializeField, Header("Visuals controllers")]
        public AGameObjectSpawnController SpawnController { get; private set; }
        [field: SerializeField] public AGameObjectDestroyController DestroyController { get; private set; }

        [field: SerializeField, Header("For the UI")] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        private void Awake()
        {
            if (Prefab == null)
            {
                Debug.LogWarning($"Impediment {name} has no prefab.");
            }
        }
    }
}