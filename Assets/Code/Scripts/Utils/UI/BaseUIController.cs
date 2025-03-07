using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Utils.UI
{
    /// <summary>
    ///     Base class that can be inherited from to help control UI.
    /// </summary>
    public class BaseUIController : MonoBehaviour
    {
        [SerializeField] protected UIDocument _uiDoc;
    }
}