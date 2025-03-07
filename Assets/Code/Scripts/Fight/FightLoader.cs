using System;
using System.Collections.Generic;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class FightLoader : MonoBehaviour
    {
        [SerializeField] private FightHexGrid _fightHexGrid;

        [SerializeField] private EntityConfigurationSO[] _devAlliesConfs;
        [SerializeField] private EntityConfigurationSO[] _devEnemiesConfs;
        private FightersGenerator _fighterGenerator;
        public Action<Fighter[], Fighter[]> onFightLoaded;

        #region Setup & tear down

        private void Awake()
        {
            if (_fightHexGrid == null) _fightHexGrid = FindObjectOfType<FightHexGrid>();
            _fighterGenerator = new FightersGenerator(_devAlliesConfs, _devEnemiesConfs);
        }

        #endregion

        private void Start()
        {
            Debug.Log("Generating FightHexGrid...");
            GenerateGrid();
            Debug.Log("FightHexGrid Generated !");
            Debug.Log("Generating Figthers...");
            KeyValuePair<Fighter[], Fighter[]> fighters = GenerateFighters();
            Debug.Log("Fighters Generated !");
            onFightLoaded?.Invoke(fighters.Key, fighters.Value);
        }

        private void GenerateGrid()
        {
            _fightHexGrid.GenerateGrid();
        }

        private KeyValuePair<Fighter[], Fighter[]> GenerateFighters()
        {
            return _fighterGenerator.GenerateFighters();
        }
    }
}