using UnityEngine;
using Cinemachine;

namespace FrostfallSaga.Fight.Abilities
{
    public class AbilityCameraFollow : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cinemachineCam;

        private void Awake()
        {
            if (cinemachineCam == null)
                cinemachineCam = GetComponent<CinemachineVirtualCamera>();

            cinemachineCam.Priority = 5;  
        }

        public void FollowAbility(Transform ability)
        {
            if (ability == null) return;

            cinemachineCam.Priority = 10;

            cinemachineCam.Follow = ability;
            cinemachineCam.LookAt = ability;

            cinemachineCam.OnTargetObjectWarped(ability, Vector3.zero);
        }

        public void StopFollowing()
        {
            cinemachineCam.Priority = 5;

            cinemachineCam.Follow = null;
            cinemachineCam.LookAt = null;
        }
    }
}
