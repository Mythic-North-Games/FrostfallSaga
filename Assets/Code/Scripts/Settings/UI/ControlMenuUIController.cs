using FrostfallSaga.Audio;
using FrostfallSaga.Core;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Settings.UI
{
    public static class ControlMenuUIController
    {
        #region UXML Names & classes
        private static readonly string TITLE_SCREEN_BUTTON_UI_NAME = "TitleScreenButton";
        private static readonly string QUIT_BUTTON_UI_NAME = "QuitButton";
        #endregion

        public static void Setup(VisualElement root)
        {
            root.Q<Button>(TITLE_SCREEN_BUTTON_UI_NAME).RegisterCallback<ClickEvent>(OnTitleScreenButtonClicked);
            root.Q<Button>(QUIT_BUTTON_UI_NAME).RegisterCallback<ClickEvent>(OnQuitButtonClicked);
        }

        private static void OnTitleScreenButtonClicked(ClickEvent _evt)
        {
            AudioManager.Instance.StopCurrentMusic();
            SceneTransitioner.TransitionToScene(EScenesName.TITLE_SCREEN.ToSceneString());
        }

        private static void OnQuitButtonClicked(ClickEvent _evt)
        {
            Application.Quit(); // TODO: Implement a proper quit
        }
    }
}