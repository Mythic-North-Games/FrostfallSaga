using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    /// <summary>
    ///     Base class that can be inherited from to help control UI.
    /// </summary>
    public class BaseUIController : MonoBehaviour
    {
        [SerializeField] protected UIDocument _uiDoc;
        private void Start()
        {
            if (_uiDoc == null)
            {
                Debug.LogError("UI Document is not set in the inspector.");
            }
        }
    }
}