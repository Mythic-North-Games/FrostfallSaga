using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Audio
{
    public class UIAudioRegistrator : MonoBehaviour
    {
        AudioManager _audioManager;

        private void Start()
        {
            _audioManager = AudioManager.Instance;

            UIDocument[] allUIDocuments = FindObjectsOfType<UIDocument>();
            foreach (UIDocument uIDoc in allUIDocuments)
            {
                RegisterUIAudioForUIDoc(uIDoc);
            }
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
            _audioManager.PlayUISound(_audioManager.UIAudioClips.ButtonClick);
        }

        private void OnButtonHover(MouseEnterEvent evt)
        {
            _audioManager.PlayUISound(_audioManager.UIAudioClips.ButtonHover);
        }
    }
}