using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight
{
    public class FightersGenerator : MonoBehaviour
    {
        public Action<Fighter[], Fighter[]> onFightersGenerated;

        [SerializeField] private GameObject _fighterPrefab;
        [SerializeField] private PreFightDataSO _preFightData;

        private void OnEnable()
        {
            if (_preFightData == null)
            {
                Debug.LogError("No PreFightData scriptable object given. Can't generate fighters.");
                return;
            }
            if (_fighterPrefab == null)
            {
                Debug.LogError("No fighter prefab given. Can't generate fighters.");
                return;
            }
        }

        private void Start()
        {
            List<Fighter> allies = new();
            _preFightData.alliesFighterSetup.ToList().ForEach(allyFighterSetup => {
                GameObject fighterGameObject = Instantiate(_fighterPrefab);
                Fighter fighter = fighterGameObject.GetComponent<Fighter>();
                fighter.Setup(allyFighterSetup);
                allies.Add(fighter);
            });

            List<Fighter> enemies = new();
            _preFightData.enemiesFighterSetup.ToList().ForEach(enemyFighterSetup => {
                GameObject fighterGameObject = Instantiate(_fighterPrefab);
                Fighter fighter = fighterGameObject.GetComponent<Fighter>();
                fighter.Setup(enemyFighterSetup);
                enemies.Add(fighter);
            });

            onFightersGenerated?.Invoke(allies.ToArray(), enemies.ToArray());

            _preFightData.alliesFighterSetup = null;
            _preFightData.enemiesFighterSetup = null;
        }

        private void OnDisable()
        {
            if (_preFightData == null)
            {
                Debug.LogWarning("No PreFightData scriptable object given. Can't tear down properly.");
                return;
            }
            if (_fighterPrefab == null)
            {
                Debug.LogWarning("No fighter prefab given. Can't tear down properly.");
                return;
            }
        }
    }
}