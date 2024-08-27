using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{

    private VisualElement _settingsPanel;
    private Button _settingButton;




    //Start
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
    
        //
        _settingsPanel = root.Q<VisualElement>("SettingsPanel");

        _settingButton = root.Q<Button>("SettingButton");



        _timelinePanel

        
        _settingButton.RegisterCallback<ClickEvent>(OnOpenButtonClicked)


        
    }

    void Update()
    {

    }

}