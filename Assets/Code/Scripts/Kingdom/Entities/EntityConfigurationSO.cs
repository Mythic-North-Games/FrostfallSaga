using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Kingdom.Entities
{

    [CreateAssetMenu(fileName = "EntityConfiguration", menuName = "ScriptableObjects/Entities/EntityConfiguration", order = 0)]
    public class EntityConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public FighterConfigurationSO FighterConfiguration { get; private set; }
    }
}