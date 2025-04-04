using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Audio
{
    public class UIAudioRegistrator : MonoBehaviour
    {
        private void Start()
        {
            UIDocument[] allUIDocuments = FindObjectsOfType<UIDocument>();
            foreach (UIDocument uIDoc in allUIDocuments) RegisterUIAudioForUIDoc(uIDoc);
        }

        private void RegisterUIAudioForUIDoc(UIDocument uIDoc)
        {
            foreach (VisualElement child in uIDoc.rootVisualElement.Query<Button>().Build())
            {
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
            AudioManager.Instance.PlayUISound(UISounds.ButtonClick);
        }

        private void OnButtonHover(MouseEnterEvent evt)
        {
            AudioManager.Instance.PlayUISound(UISounds.ButtonHover);
        }
    }
}