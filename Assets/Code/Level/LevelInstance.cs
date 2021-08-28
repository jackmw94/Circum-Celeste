﻿using System;
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
        private Action<LevelResult> _levelFinishedCallback = null;

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
            
            GameContainer.Instance.TimerUI.ResetTimer();

            _escapes.ApplyFunction(InitialiseEscape);

            _isStarted = false;
            _escapeShown = false;
        }

        public void LevelReady()
        {
            CircumDebug.Log($"Level '{name}' ready");
            _players.ApplyFunction(p => p.LevelReady());
            
            // assuming this will always count up, therefore reset == hidden
            GameContainer.Instance.TimerUI.ResetTimer();

            HandleUIIntroductions();
        }

        private void HandleUIIntroductions()
        {
            UIInputElementsContainer movementUi = GameContainer.Instance.UIInputElementsContainer;

            movementUi.PulsePowerButton.StartStopPulse(_introduceElement == IntroduceElement.PowerButton);
            movementUi.PulseMoverHandle.StartStopPulse(_introduceElement == IntroduceElement.MovementHandle);
        }

        public void StartLevel(Action<LevelResult> levelFinishedCallback)
        {
            _isStarted = true;
            _startTime = Time.time;

            if (_escapeCriteria == EscapeCriteria.Timed)
            {
                GameContainer.Instance.TimerUI.StartTimer(_escapeDuration);
            }

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

            LevelResult levelResult = null;
            // if we're not recording, we're watching a replay and therefore do not want to record the results
            if (_players[0].IsRecording)
            {
                levelResult = new LevelResult(true, perfectLevel, _players[0].GetLevelRecording());
            }

            _levelFinishedCallback?.Invoke(levelResult);
            _isStarted = false;
        }

        private void CheckLevelFailed()
        {
            if (_players.All(p => p.IsDead))
            {
                LevelResult levelResult = new LevelResult(false, false, null);
                _levelFinishedCallback?.Invoke(levelResult);
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
            _escapeShown = true;
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