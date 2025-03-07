using System;
using System.Collections.Generic;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Fight.Fighters;
using Unity.Properties;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class FightLoader : MonoBehaviour
    {
        public Action<Fighter[], Fighter[]> onFightLoaded;

        [SerializeField] private FightHexGrid _fightHexGrid;
        private FightersGenerator _fighterGenerator;

        [SerializeField] private EntityConfigurationSO[] _devAlliesConfs;
        [SerializeField] private EntityConfigurationSO[] _devEnemiesConfs;

        void Start()
        {
            Debug.Log("Generating FightHexGrid...");
            GenerateGrid();
            Debug.Log("FightHexGrid Generated !");
            Debug.Log("Generating Figthers...");
            KeyValuePair<Fighter[], Fighter[]>  fighters = GenerateFighters();
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

        #region Setup & tear down
        private void Awake()
        {
            if (_fightHexGrid == null)
            {
                _fightHexGrid = FindObjectOfType<FightHexGrid>();
            }
            _fighterGenerator = new FightersGenerator(_devAlliesConfs, _devEnemiesConfs);
        }
        #endregion

    }
}
