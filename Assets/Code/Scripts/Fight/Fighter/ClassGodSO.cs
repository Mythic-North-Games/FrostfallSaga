using UnityEngine;

namespace FrostfallSaga.Fight.Fighters
{
    /// <summary>
    /// Repesents a class god, a god that is associated with a class.
    /// It defines how the fighter favors are increased and decreased during fight.
    /// </summary>
    [CreateAssetMenu(fileName = "ClassGod", menuName = "ScriptableObjects/Fight/ClassGod", order = 0)]
    public class ClassGodSO : ScriptableObject
    {
        [field: SerializeField] public string GodName { get; private set; }
        [field: SerializeField] public GodFavorsActionToInt[] FavorGivingActions { get; private set; }
    }
}
