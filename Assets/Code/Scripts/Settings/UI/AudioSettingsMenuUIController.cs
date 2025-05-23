using FrostfallSaga.Audio;
using UnityEngine.UIElements;

namespace FrostfallSaga.Settings.UI
{
    public static class SettingsMenuUIController
    {
        #region UXML Names & classes
        private static readonly string MASTER_VOLUME_SLIDER_UI_NAME = "MasterVolumeSlider";
        private static readonly string MUSIC_VOLUME_SLIDER_UI_NAME = "MusicVolumeSlider";
        private static readonly string SFX_VOLUME_SLIDER_UI_NAME = "SFXVolumeSlider";
        private static readonly string UI_VOLUME_SLIDER_UI_NAME = "UIVolumeSlider";
        private static readonly string SLIDER_VALUE_LABEL_UI_NAME = "SliderValue";

        private static readonly string SLIDER_DRAGGER_ACTIVE_CLASSNAME = "sliderDraggerActive";
        #endregion

        private static AudioManager _audioManager;
        private static SliderInt _masterVolumeSlider;
        private static SliderInt _musicVolumeSlider;
        private static SliderInt _sfxVolumeSlider;
        private static SliderInt _uiVolumeSlider;

        public static void Setup(VisualElement root)
        {
            _audioManager = AudioManager.Instance;
            
            _masterVolumeSlider = root.Q<SliderInt>(MASTER_VOLUME_SLIDER_UI_NAME);
            SetSliderValue(_masterVolumeSlider, (int)(_audioManager.MasterVolume * 100));
            _masterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeSliderValueChanged);
            _masterVolumeSlider.Q<VisualElement>("unity-dragger").RegisterCallback<MouseEnterEvent>(OnMouseEnterDragger);
            _masterVolumeSlider.Q<VisualElement>("unity-dragger").RegisterCallback<MouseLeaveEvent>(OnMouseLeaveDragger);

            _musicVolumeSlider = root.Q<SliderInt>(MUSIC_VOLUME_SLIDER_UI_NAME);
            SetSliderValue(_musicVolumeSlider, (int)(_audioManager.MusicVolume * 100));
            _musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeSliderValueChanged);
            _musicVolumeSlider.Q<VisualElement>("unity-dragger").RegisterCallback<MouseEnterEvent>(OnMouseEnterDragger);
            _musicVolumeSlider.Q<VisualElement>("unity-dragger").RegisterCallback<MouseLeaveEvent>(OnMouseLeaveDragger);

            _sfxVolumeSlider = root.Q<SliderInt>(SFX_VOLUME_SLIDER_UI_NAME);
            SetSliderValue(_sfxVolumeSlider, (int)(_audioManager.FXVolume * 100));
            _sfxVolumeSlider.RegisterValueChangedCallback(OnSFXVolumeSliderValueChanged);
            _sfxVolumeSlider.Q<VisualElement>("unity-dragger").RegisterCallback<MouseEnterEvent>(OnMouseEnterDragger);
            _sfxVolumeSlider.Q<VisualElement>("unity-dragger").RegisterCallback<MouseLeaveEvent>(OnMouseLeaveDragger);

            _uiVolumeSlider = root.Q<SliderInt>(UI_VOLUME_SLIDER_UI_NAME);
            SetSliderValue(_uiVolumeSlider, (int)(_audioManager.UIVolume * 100));
            _uiVolumeSlider.RegisterValueChangedCallback(OnUIVolumeSliderValueChanged);
            _uiVolumeSlider.Q<VisualElement>("unity-dragger").RegisterCallback<MouseEnterEvent>(OnMouseEnterDragger);
            _uiVolumeSlider.Q<VisualElement>("unity-dragger").RegisterCallback<MouseLeaveEvent>(OnMouseLeaveDragger);
        }

        private static void OnMasterVolumeSliderValueChanged(ChangeEvent<int> evt)
        {
            _masterVolumeSlider.parent.Q<Label>(SLIDER_VALUE_LABEL_UI_NAME).text = $"{evt.newValue}%";
            _audioManager.SetMasterVolume(evt.newValue / 100f);
        }

        private static void OnMusicVolumeSliderValueChanged(ChangeEvent<int> evt)
        {
            _musicVolumeSlider.parent.Q<Label>(SLIDER_VALUE_LABEL_UI_NAME).text = $"{evt.newValue}%";
            _audioManager.SetMusicVolume(evt.newValue / 100f);
        }

        private static void OnSFXVolumeSliderValueChanged(ChangeEvent<int> evt)
        {
            _sfxVolumeSlider.parent.Q<Label>(SLIDER_VALUE_LABEL_UI_NAME).text = $"{evt.newValue}%";
            _audioManager.SetFXVolume(evt.newValue / 100f);
        }

        private static void OnUIVolumeSliderValueChanged(ChangeEvent<int> evt)
        {
            _uiVolumeSlider.parent.Q<Label>(SLIDER_VALUE_LABEL_UI_NAME).text = $"{evt.newValue}%";
            _audioManager.SetUIVolume(evt.newValue / 100f);
        }

        private static void OnMouseEnterDragger(MouseEnterEvent evt)
        {
            VisualElement dragger = (VisualElement)evt.currentTarget;
            dragger.AddToClassList(SLIDER_DRAGGER_ACTIVE_CLASSNAME);
        }

        private static void OnMouseLeaveDragger(MouseLeaveEvent evt)
        {
            VisualElement dragger = (VisualElement)evt.currentTarget;
            dragger.RemoveFromClassList(SLIDER_DRAGGER_ACTIVE_CLASSNAME);
        }

        private static void SetSliderValue(SliderInt slider, int value)
        {
            slider.SetValueWithoutNotify(value);
            slider.parent.Q<Label>(SLIDER_VALUE_LABEL_UI_NAME).text = $"{value}%";
        }
    }
}