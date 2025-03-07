using Cinemachine;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    /// <summary>
    ///     Camera used during fight to follow to currently playing fighter.
    /// </summary>
    public class FightCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private FightManager _fightManager;

        private void OnEnable()
        {
            if (_camera == null) _camera = GetComponent<CinemachineVirtualCamera>();
            if (_camera == null)
            {
                Debug.LogError("Fight camera does not find a cinemachine virtual camera to work with.");
                return;
            }

            if (_fightManager == null) _fightManager = FindAnyObjectByType<FightManager>();
            if (_fightManager == null)
            {
                Debug.LogError("Fight camera does not find a fight manager to work with.");
                return;
            }

            _fightManager.onFighterTurnBegan += OnFighterTurnBegan;
            _fightManager.onFightEnded += OnFightEnded;
        }

        private void OnFighterTurnBegan(Fighter fighter, bool _isAlly)
        {
            _camera.Follow = fighter.CameraAnchor;
        }

        private void OnFightEnded(Fighter[] _allies, Fighter[] _enemies)
        {
            _camera.enabled = false;
        }
    }
}