using System;
using System.Collections.Generic;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class FightLoader : MonoBehaviour
    {
        [SerializeField] private FightHexGrid hexGrid;
        [SerializeField] private EntityConfigurationSO[] devAlliesConfs;
        [SerializeField] private EntityConfigurationSO[] devEnemiesConfs;
        private FightersGenerator _fighterGenerator;
        public Action<Fighter[], Fighter[]> OnFightLoaded;

        #region Setup & tear down

        private void Awake()
        {
            hexGrid ??= FindObjectOfType<FightHexGrid>();
            _fighterGenerator = new FightersGenerator(devAlliesConfs, devEnemiesConfs);
        }

        #endregion

        private void Start()
        {
            Debug.Log("Generating Fight Grid...");
            hexGrid.GenerateGrid();
            Debug.Log("Fight Grid Generated !");
            Debug.Log("Generating Fighters...");
            KeyValuePair<Fighter[], Fighter[]> fighters = GenerateFighters();
            Debug.Log("Fighters Generated !");
            OnFightLoaded?.Invoke(fighters.Key, fighters.Value);
        }

        private KeyValuePair<Fighter[], Fighter[]> GenerateFighters()
        {
            return _fighterGenerator.GenerateFighters();
        }
    }
}