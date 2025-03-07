using UnityEngine;

namespace FrostfallSaga.Fight.Fighters
{
    [CreateAssetMenu(
        fileName = "FighterBuffVisualsController",
        menuName = "ScriptableObjects/Fight/Statuses/FighterBuffVisualsController"
    )]
    public class FighterBuffVisualsController : ScriptableObject
    {
        [field: SerializeField] public GameObject RecurringParticles { get; private set; }
        [field: SerializeField] public GameObject ApplicationParticles { get; private set; }
        private ParticleSystem _recurringParticles;

        public bool IsShowingRecurringVisuals => _recurringParticles != null && _recurringParticles.isPlaying;

        public void ShowRecurringVisuals(Fighter fighter)
        {
            if (RecurringParticles == null)
            {
                Debug.LogWarning("RecurringStatusParticles is not set.");
                return;
            }

            _recurringParticles = Instantiate(RecurringParticles, fighter.transform).GetComponent<ParticleSystem>();
        }

        public void HideRecurringVisuals()
        {
            if (RecurringParticles == null) return;
            _recurringParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public void ShowApplicationVisuals(Fighter fighter)
        {
            if (ApplicationParticles == null)
            {
                Debug.LogWarning("ApplicationParticles is not set.");
                return;
            }

            Instantiate(ApplicationParticles, fighter.transform);
        }
    }
}