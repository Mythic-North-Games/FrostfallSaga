using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [CreateAssetMenu(fileName = "StatusVisualsController", menuName = "ScriptableObjects/Fight/Statuses/StatusVisualsController")]
    public class StatusVisualsController : ScriptableObject
    {
        [field: SerializeField] public GameObject RecurringStatusParticles { get; private set; }
        [field: SerializeField] public GameObject StatusApplicationParticles { get; private set; }

        public bool IsShowingRecurringVisuals
        { 
            get { return _recurringStatusParticles != null && _recurringStatusParticles.isPlaying; }
        }
        private ParticleSystem _recurringStatusParticles;

        public void ShowRecurringStatusVisuals(Fighter fighter)
        {
            if (RecurringStatusParticles == null)
            {
                Debug.LogWarning("RecurringStatusParticles is not set.");
                return;
            }
            _recurringStatusParticles = Instantiate(RecurringStatusParticles, fighter.transform).GetComponent<ParticleSystem>();
        }

        public void HideRecurringStatusVisuals()
        {
            if (RecurringStatusParticles == null)
            {
                return;
            }
            _recurringStatusParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public void ShowStatusApplicationVisuals(Fighter fighter)
        {
            if (StatusApplicationParticles == null)
            {
                Debug.LogWarning("StatusApplicationParticles is not set.");
                return;
            }
            Instantiate(StatusApplicationParticles, fighter.transform);
        }
    }
}