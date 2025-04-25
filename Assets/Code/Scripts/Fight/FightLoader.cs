using System;
using System.Collections.Generic;
using FrostfallSaga.Audio;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.BookMenu.UI;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class FightLoader : MonoBehaviour
    {
        [SerializeField] private FightHexGrid _hexGrid;
        [SerializeField] private BookMenuBarUIController _bookMenuBarUIController;
        [SerializeField] private EntityConfigurationSO[] _devAlliesConfs;
        [SerializeField] private EntityConfigurationSO[] _devEnemiesConfs;
        private FightersGenerator _fighterGenerator;
        public Action<Fighter[], Fighter[]> onFightLoaded;

        #region Setup & tear down

        private void Awake()
        {
            _hexGrid ??= FindObjectOfType<FightHexGrid>();
            _bookMenuBarUIController ??= FindObjectOfType<BookMenuBarUIController>();
            _fighterGenerator = new FightersGenerator(_devAlliesConfs, _devEnemiesConfs);
        }

        #endregion

        private void Start()
        {
            _bookMenuBarUIController.SetButtonsVisibility(
                questsMenuButtonVisible: false,
                inventoryMenuButtonVisible: false,
                abilitySystemMenuButtonVisible: false,
                settingsMenuButtonVisible: true
            );

            Debug.Log("Generating Fight Grid...");
            _hexGrid.GenerateGrid();
            Debug.Log("Fight Grid Generated !");
            Debug.Log("Generating Fighters...");
            KeyValuePair<Fighter[], Fighter[]> fighters = GenerateFighters();
            Debug.Log("Fighters Generated !");
            SceneTransitioner.FadeInCurrentScene();

            AudioManager audioManager = AudioManager.Instance;
            audioManager.PlayMusicSound(audioManager.MusicAudioClips.Fight);
            onFightLoaded?.Invoke(fighters.Key, fighters.Value);
        }

        private KeyValuePair<Fighter[], Fighter[]> GenerateFighters()
        {
            return _fighterGenerator.GenerateFighters();
        }
    }
}