using UnityEngine;

namespace FrostfallSaga.Core.Fight
{
    public abstract class AFighter : MonoBehaviour
    {
        [field: SerializeField]
        [field: Header("World coherence")]
        public string EntitySessionId { get; protected set; }

        public abstract int GetHealth();
    }
}