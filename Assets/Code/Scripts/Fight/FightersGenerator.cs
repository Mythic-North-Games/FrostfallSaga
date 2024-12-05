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

        [SerializeField] private PreFightDataSO _preFightData;
        [SerializeField] private FighterSetup[] _devAlliesFighterSetup;
        [SerializeField] private FighterSetup[] _devEnemiesFighterSetup;

        private void Start()
        {
            FighterSetup[] alliesFighterSetup = _preFightData.alliesFighterSetup != null && _preFightData.alliesFighterSetup.Length > 0 ?
                _preFightData.alliesFighterSetup :
                _devAlliesFighterSetup;
            FighterSetup[] enemiesFighterSetup = _preFightData.enemiesFighterSetup != null && _preFightData.enemiesFighterSetup.Length > 0 ?
                _preFightData.enemiesFighterSetup :
                _devEnemiesFighterSetup;

            List<Fighter> allies = new();
            alliesFighterSetup.ToList().ForEach(allyFighterSetup =>
                allies.Add(SpawnAndSetupFighter(allyFighterSetup))
            );

            List<Fighter> enemies = new();
            enemiesFighterSetup.ToList().ForEach(enemyFighterSetup =>
                enemies.Add(SpawnAndSetupFighter(enemyFighterSetup, $"{enemies.Count}"))
            );

            onFightersGenerated?.Invoke(allies.ToArray(), enemies.ToArray());

            if (alliesFighterSetup == _preFightData.alliesFighterSetup)
            {
                _preFightData.alliesFighterSetup = null;
                _preFightData.enemiesFighterSetup = null;
                return;
            }

            Debug.Log("Fight launched in dev mode. Not went through kingdom first.");
        }

        private Fighter SpawnAndSetupFighter(FighterSetup fighterSetup, string nameSuffix = "")
        {
            GameObject fighterGameObject = Instantiate(fighterSetup.fighterPrefab);
            fighterGameObject.name = new($"{fighterSetup.name}{nameSuffix}");
            Fighter fighter = fighterGameObject.GetComponent<Fighter>();
            fighter.Setup(fighterSetup);
            return fighter;
        }

        #region Setup & Teardown
        private void Awake()
        {
            if (_preFightData == null)
            {
                Debug.LogError("No PreFightData scriptable object given. Can't generate fighters.");
                return;
            }
            if (_devAlliesFighterSetup == null || _devAlliesFighterSetup.Length == 0)
            {
                return;
            }
            foreach (FighterSetup fighterSetup in _devAlliesFighterSetup)
            {
                fighterSetup.initialStats.magicalResistances = new() 
                {
                    { EMagicalElement.FIRE, 0 },
                    { EMagicalElement.ICE, 0 },
                    { EMagicalElement.LIGHTNING, 0 },
                    { EMagicalElement.EARTH, 0 },
                    { EMagicalElement.LIGHT, 0 },
                    { EMagicalElement.DARKNESS, 0 }
                };
            }

            if (_devEnemiesFighterSetup == null || _devEnemiesFighterSetup.Length == 0)
            {
                return;
            }
            foreach (FighterSetup fighterSetup in _devEnemiesFighterSetup)
            {
                fighterSetup.initialStats.magicalResistances = new() 
                {
                    { EMagicalElement.FIRE, 0 },
                    { EMagicalElement.ICE, 0 },
                    { EMagicalElement.LIGHTNING, 0 },
                    { EMagicalElement.EARTH, 0 },
                    { EMagicalElement.LIGHT, 0 },
                    { EMagicalElement.DARKNESS, 0 }
                };
            }
        }
        #endregion
    }
}