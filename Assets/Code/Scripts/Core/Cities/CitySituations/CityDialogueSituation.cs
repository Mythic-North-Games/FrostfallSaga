using FrostfallSaga.Core.Dialogues;
using UnityEngine;

namespace FrostfallSaga.Core.Cities.CitySituations
{
    [CreateAssetMenu(fileName = "CityDialogueSituation",
        menuName = "ScriptableObjects/Cities/CitySituations/CityDialogueSituation", order = 0)]
    public class CityDialogueSituation : ACitySituationSO
    {
        [field: SerializeField] public DialogueSO Dialogue { get; private set; }
    }
}