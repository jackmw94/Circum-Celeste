using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Debugging;
using Code.Juice;
using Code.UI;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelInstance : MonoBehaviour
    {
        private EscapeCriteria _escapeCriteria;
        private float _escapeDuration;
        private IntroduceElement _introduceElement;

        private List<Player.Player> _players;
        private List<Pickup> _pickups;
        private List<Enemy> _enemies;
        private List<Escape> _escapes;

        private bool _isStarted;
        private float _startTime;

        private bool _escapeShown;
        private Action<bool, bool> _levelFinishedCallback = null;

        private float LevelTime => Time.time - _startTime;
        public bool PlayerIsMoving => _players.Any(p => p.IsMoving);
        
        public void SetupLevel(LevelLayout levelLayout, List<Player.Player> players, List<Pickup> pickups, List<Enemy> enemies, List<Escape> escapes)
        {
            _escapeCriteria = levelLayout.EscapeCriteria;
            _escapeDuration = levelLayout.EscapeTimer;
            _introduceElement = levelLayout.IntroduceElement;

            _players = players;
            _pickups = pickups;
            _enemies = enemies;
            _escapes = escapes;
            
            _escapes.ApplyFunction(InitialiseEscape);

            _isStarted = false;
            _escapeShown = false;
        }

        public void LevelReady()
        {
            _players.ApplyFunction(p => p.LevelReady());

            // todo: put this in an area of code less tied to game logic + handle edge cases like never clicking the button
            switch (_introduceElement)
            {
                case IntroduceElement.PowerButton:
                    UIInputElementsContainer movementUi = GameContainer.Instance.UIInputElementsContainer;
                    movementUi.PulseButton.StartStopPulse(true);
                    break;
            }
        }

        public void StartLevel(Action<bool, bool> levelFinishedCallback)
        {
            _isStarted = true;
            _startTime = Time.time;

            _players.ApplyFunction(p => p.LevelStarted());
            _enemies.ApplyFunction(p => p.LevelStarted());
            
            _levelFinishedCallback = levelFinishedCallback;
        }

        public Vector3 GetPlayerPosition(int playerIndex)
        {
            if (_players.Count >= playerIndex)
            {
                return _players[playerIndex].transform.position;
            }
            
            throw new ArgumentOutOfRangeException(nameof(playerIndex), $"Requested index (={playerIndex}) out of range of players list (count={_players.Count})");

        }

        private void Update()
        {
            if (!_isStarted)
            {
                return;
            }
            
            CheckEscape();
            CheckLevelFailed();
            
#if UNITY_EDITOR
            DebugUpdate();
#endif
        }

        private void LevelCompleted()
        {
            CircumDebug.Assert(_isStarted, "Level has been completed before it's started? What's the deal with that..?");
            CircumDebug.Log("LEVEL COMPLETED");
            
            _players.ApplyFunction(p => p.LevelFinished());
            _enemies.ApplyFunction(p => p.LevelFinished());
            
            Feedbacks.Instance.TriggerFeedback(Feedbacks.FeedbackType.CompletedLevel);

            bool perfectLevel = _players.All(p => p.NoDamageTaken);
            
            _levelFinishedCallback?.Invoke(true, perfectLevel);
            _isStarted = false;
        }

        private void CheckLevelFailed()
        {
            if (_players.All(p => p.IsDead))
            {
                _levelFinishedCallback?.Invoke(false, false);
                _isStarted = false;
            }
        }

        private void CheckEscape()
        {
            if (_escapeShown)
            {
                return;
            }

            switch (_escapeCriteria)
            {
                case EscapeCriteria.Timed:
                    if (LevelTime > _escapeDuration)
                    {
                        ShowEscape();
                    }
                    break;
                case EscapeCriteria.PickedUpAll:
                    if (_pickups.All(p => p.IsCollected))
                    {
                        ShowEscape();
                    }
                    break;
                case EscapeCriteria.DestroyedAll:
                    if (_enemies.All(p => p.IsDead))
                    {
                        ShowEscape();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_escapeCriteria), _escapeCriteria, "Cannot determine when escape should be shown");
            }
        }

        private void InitialiseEscape(Escape escape)
        {
            escape.gameObject.SetActive(false);
            escape.SetEscapeCallback(LevelCompleted);
        }

        private void ShowEscape()
        {
            CircumDebug.Log("Showing escapes");
            _escapes.ApplyFunction(p => p.gameObject.SetActive(true));
        }

#if UNITY_EDITOR
        private void DebugUpdate()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CircumDebug.Log("Debug showing escapes");
                ShowEscape();
            }
        }
#endif
    }
}