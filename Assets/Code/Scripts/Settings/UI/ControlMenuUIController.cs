using FrostfallSaga.Audio;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Settings.UI
{
    public static class ControlMenuUIController
    {
        #region UXML Names & classes
        private static readonly string TITLE_SCREEN_CONTROL_BUTTON_UI_NAME = "TitleScreenButton";
        private static readonly string QUIT_CONTROL_BUTTON_UI_NAME = "QuitButton";
        #endregion

        public static void Setup(VisualElement root)
        {
            LeftArrowButtonUIController titleScreenButtonController = new(
                root.Q<VisualElement>(TITLE_SCREEN_CONTROL_BUTTON_UI_NAME),
                "Title Screen"
            );
            titleScreenButtonController.onButtonClicked += (_) => OnTitleScreenButtonClicked();

            LeftArrowButtonUIController quitButtonController = new(root.Q<VisualElement>(QUIT_CONTROL_BUTTON_UI_NAME), "Quit");
            quitButtonController.onButtonClicked += (_) => OnQuitButtonClicked();
        }

        private static void OnTitleScreenButtonClicked()
        {
            AudioManager.Instance.StopCurrentMusic();
            SceneTransitioner.TransitionToScene(EScenesName.TITLE_SCREEN.ToSceneString());
        }

        private static void OnQuitButtonClicked()
        {
            Application.Quit(); // TODO: Implement a proper quit
        }
    }
}