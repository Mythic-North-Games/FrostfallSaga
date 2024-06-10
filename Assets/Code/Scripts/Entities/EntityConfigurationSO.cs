using UnityEngine;

namespace FrostfallSaga.Entities
{
    [CreateAssetMenu(fileName = "Entity", menuName = "ScriptableObjects/Entity", order = 0)]
    public class EntitySO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string DefaultAnimationState { get; private set; } = "Idle";
    }
}
