using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Audio;


namespace FrostfallSaga.Utils.UI
{
    /// <summary>
    /// Base class that can be inherited from to help control UI.
    /// </summary>
    public class BaseUIController : MonoBehaviour
    {
        [SerializeField] protected UIDocument _uiDoc;
        private void Start()
        {   
            
            // Vérification
            Debug.Log("UIDocument trouvé : " + (_uiDoc != null));

            if (_uiDoc == null)
            {
                Debug.LogError("UI Document is not set in the inspector.");
                return;
            }
 
            foreach(VisualElement child in _uiDoc.rootVisualElement.Query<Button>().Build())
            {
                Debug.Log($"Child found: {child.name} of type {child.GetType()}");

                if (child is Button button)
                {
                    Debug.Log($"Button found: {button.name}");
                    button.RegisterCallback<ClickEvent>(OnButtonClick);
                    button.RegisterCallback<MouseEnterEvent>(OnButtonHover);
                }
            }
        }

        private void OnButtonClick(ClickEvent evt)
{
    Debug.Log("Bouton cliqué !");
    AudioManager.instance?.PlayUISound(UISounds.ButtonClick);
}

        private void OnButtonHover(MouseEnterEvent evt)
{
    Debug.Log("Souris sur le bouton !");
    AudioManager.instance?.PlayUISound(UISounds.ButtonHover);
}
    }
}