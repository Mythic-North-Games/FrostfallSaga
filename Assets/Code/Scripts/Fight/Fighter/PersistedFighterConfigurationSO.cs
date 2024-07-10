using UnityEngine;

namespace FrostfallSaga.Fight.Fighters
{
    [CreateAssetMenu(fileName = "PersistedFighter", menuName = "ScriptableObjects/Fight/PersistedFighter", order = 0)]
    public class PersistedFighterConfigurationSO : FighterConfigurationSO
    {
        [field: SerializeField] public ActiveAbilityToAnimation[] EquipedActiveAbilities { get; private set; }
        [field: SerializeField] public int Health { get; private set; }
    }
}