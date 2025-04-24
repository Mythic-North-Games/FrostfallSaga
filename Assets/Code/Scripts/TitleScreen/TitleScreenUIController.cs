using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Audio;
using FrostfallSaga.Core;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.TitleScreen
{
    public class TitleScreenUIController : BaseUIController
    {
        #region UXML element names and classes
        private static readonly string START_BUTTON_UI_NAME = "StartButton";

        private static readonly string START_BUTTON_HIDDEN_CLASSNAME = "startButtonHidden";
        #endregion

        [SerializeField] private float _startButtonHideDuration = 1f;

        private Button _startButton;

        private void Awake()
        {
            _startButton = _uiDoc.rootVisualElement.Q<Button>(START_BUTTON_UI_NAME);
            _startButton.RegisterCallback<ClickEvent>(OnStartButtonClicked);
        }

        private void Start()
        {
            SceneTransitioner.FadeInCurrentScene();
        }

        private void OnStartButtonClicked(ClickEvent _evt)
        {
            _startButton.style.transitionDuration = new List<TimeValue>()
            {
                new TimeValue(_startButtonHideDuration, TimeUnit.Second)
            };
            StartCoroutine(HideStartButtonAndTransitionToNextScene());
        }

        private IEnumerator HideStartButtonAndTransitionToNextScene()
        {
            _startButton.AddToClassList(START_BUTTON_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(_startButtonHideDuration);
            AudioManager.Instance.PlayMusicSound(AudioManager.Instance.MusicAudioClips.Kingdom);
            SceneTransitioner.TransitionToScene(EScenesName.KINGDOM.ToSceneString());
        }
    }
}