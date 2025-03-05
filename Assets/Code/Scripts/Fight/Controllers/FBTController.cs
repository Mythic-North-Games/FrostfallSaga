using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Controllers.FighterBehaviourTrees;

namespace FrostfallSaga.Fight.Controllers
{
    /// <summary>
    /// Fighter controller that uses a behaviour tree to decide the actions of the fighter.
    /// </summary>
    public class FBTController : AFighterController
    {
        private FighterBehaviourTree _fbt;
        private FightManager _fightManager;
        private Fighter _possessedFighter;

        private int _timeBetweenActionsInSec;
        private float _betweenActionTimer = 0;
        private bool _waitingBeforeNextAction = false;

        private bool _firstExecution = false;

        private void Awake()
        {
            enabled = false;    // Only run when playing a turn.
        }

        public void Setup(FightManager fightManager, int timeBetweenActionsInSec = 1)
        {
            _fightManager = fightManager;
            _timeBetweenActionsInSec = timeBetweenActionsInSec;

            _fightManager.onFightEnded += OnFightEnded;
        }

        /// <summary>
        /// Plays the turn of the fighter using the behaviour tree set in the controller.
        /// </summary>
        /// <param name="fighterToPlay">The fighter to control.</param>
        /// <param name="fighterTeams">All the fighters of the fight and their corresponding team.</param>
        /// <param name="fightGrid">The fight grid.</param>
        public override void PlayTurn(Fighter fighterToPlay)
        {
            if (fighterToPlay.PersonalityTrait == null)
            {
                Debug.LogError("The fighter to play has no personality trait.");
                return;
            }

            _possessedFighter = fighterToPlay;
            _fbt = FighterBehaviourTreeFactory.CreateBehaviourTree(
                fighterToPlay.PersonalityTrait.FighterBehaviourTreeID,
                fighterToPlay,
                _fightManager.FightGrid,
                _fightManager.FighterTeams
            );
            _firstExecution = true;
            _waitingBeforeNextAction = true;
            enabled = true;
        }

        private void Update()
        {
            // Wait while possessed fighter is doing an action
            if (_fbt.IsActionRunning())
            {
                return;
            }
            
            // Trigger onFighterActionEnded event when an action is done
            if (!_firstExecution)
            {
                onFighterActionEnded?.Invoke(_possessedFighter);
                _waitingBeforeNextAction = true;
            }

            // Wait before starting the next action
            if (_waitingBeforeNextAction && _betweenActionTimer < _timeBetweenActionsInSec)
            {
                _betweenActionTimer += Time.deltaTime;
                return;
            }
            
            // Reset timer and flag to start the next action if timer is done
            if (_waitingBeforeNextAction)
            {
                _betweenActionTimer = 0;
                _waitingBeforeNextAction = false;
            }

            // Execute the behaviour tree after waiting
            _fbt.Execute();
            if (_fbt.HasTurnEnded())
            {
                enabled = false;
                onFighterActionEnded?.Invoke(_possessedFighter);
                onFighterTurnEnded?.Invoke(_possessedFighter);
            }
            else
            {
                onFighterActionStarted?.Invoke(_possessedFighter);
            }
            _firstExecution = false;
        }

        private void OnFightEnded(Fighter[] _allies, Fighter[] _enemies)
        {
            enabled = false;
        }
    }
}