using UnityEngine;

namespace FrostfallSaga.Fight.Abilities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BaseAbility : MonoBehaviour
    {
        [field: SerializeField] public BaseAbilitySO AbilityData { get; private set; }

        private void Reset()
        {
            if (AbilityData == null)
            {
                Debug.LogWarning("No AbilityData set for ability " + name);
                return;

            }

            if (AbilityData.IconSprite == null)
            {
                Debug.LogWarning("No IconSprite set for ability " + name);
                return;
            }

            GetComponent<SpriteRenderer>().sprite = AbilityData.IconSprite;
        }

        private void OnEnable()
        {
            if (AbilityData == null)
            {
                Debug.LogError("No AbilityData set for ability " + name);
                return;

            }

            if (AbilityData.IconSprite == null)
            {
                Debug.LogError("No IconSprite set for ability " + name);
                return;
            }

            GetComponent<SpriteRenderer>().sprite = AbilityData.IconSprite;
        }
    }
}