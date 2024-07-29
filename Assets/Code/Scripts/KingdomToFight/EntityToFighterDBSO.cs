using UnityEngine;

namespace FrostfallSaga.KingdomToFight
{
    [CreateAssetMenu(fileName = "EntityToFighterDBSO", menuName = "ScriptableObjects/EntityToFighterDBSO", order = 0)]
    public class EntityToFighterDBSO : ScriptableObject
    {
        [field: SerializeField] public EntityToFighter[] DB { get; private set; }
    }
}