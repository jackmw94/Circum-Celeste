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
    public class LevelPlayInstance : LevelInstanceBase
    {
        private EscapeCriteria _escapeCriteria;
        private float _escapeDuration;
        private IntroduceElement _introduceElement;

        private List<Player.Player> _players;
        private List<Pickup> _pickups;
        private List<Enemy> _enemies;
        private List<Escape> _escapes;
        private List<Hazard> _hazards;

        private bool _isStarted;
        private float _startTime;

        private LevelRecorder _levelRecorder;

        private bool _escapeShown;
        private Action<LevelResult> _levelFinishedCallback = null;

        private float LevelTime => Time.time - _startTime;
        public override bool PlayerStartedPlaying => _players.Any(p => p.IsMoving);
        
        public void SetupLevel(LevelLayout levelLayout, List<Player.Player> players, List<Pickup> pickups, List<Enemy> enemies, List<Escape> escapes, List<Hazard> hazards)
        {
            _escapeCriteria = levelLayout.EscapeCriteria;
            _escapeDuration = levelLayout.EscapeTimer;
            _introduceElement = levelLayout.IntroduceElement;

            _players = players;
            _pickups = pickups;
            _enemies = enemies;
            _escapes = escapes;
            _hazards = hazards;
            
            GameContainer.Instance.TimerUI.ResetTimer();
            
            _isStarted = false;
            _escapeShown = false;
            
            ApplyLevelElementFunction(p => p.LevelSetup());
        }

        public override void LevelReady()
        {
            CircumDebug.Log($"Level '{name}' ready");

            ApplyLevelElementFunction(p => p.LevelReady());

            // assuming this will always count up, therefore reset == hidden
            GameContainer.Instance.TimerUI.ResetTimer();

            HandleUIIntroductions();
        }

        public override void StartLevel(Action<LevelResult> levelFinishedCallback)
        {
            _isStarted = true;
            _startTime = Time.time;

            if (_escapeCriteria == EscapeCriteria.Timed)
            {
                GameContainer.Instance.TimerUI.StartTimer(_escapeDuration);
            }

            ApplyLevelElementFunction(p => p.LevelStarted());

            _levelRecorder = gameObject.AddComponent<LevelRecorder>();
            
            _levelFinishedCallback = levelFinishedCallback;
        }

        public override Vector3 GetPlayerPosition(int playerIndex)
        {
            if (_players.Count >= playerIndex)
            {
                return _players[playerIndex].transform.position;
            }
            
            throw new ArgumentOutOfRangeException(nameof(playerIndex), $"Requested index (={playerIndex}) out of range of players list (count={_players.Count})");

        }

        private void ApplyLevelElementFunction(Action<LevelElement> levelElementFunction)
        {
            _players.ApplyFunction(levelElementFunction);
            _enemies.ApplyFunction(levelElementFunction);
            _pickups.ApplyFunction(levelElementFunction);
            _hazards.ApplyFunction(levelElementFunction);
            _escapes.ApplyFunction(levelElementFunction);
        }

        private void HandleUIIntroductions()
        {
            UIInputElementsContainer movementUi = GameContainer.Instance.UIInputElementsContainer;

            movementUi.IntroducePowerButton.SetIntroducing(_introduceElement == IntroduceElement.PowerButton);
            movementUi.IntroduceMoverHandle.SetIntroducing(_introduceElement == IntroduceElement.MovementHandle);
        }

        private void Update()
        {
            if (!_isStarted)
            {
                return;
            }

            // high level game loop
            
            if (CheckLevelFailed())
            {
                return;
            }
            
            CheckEscapeReadyToShow();
            
            CheckLevelSuccess();
            
            //
            

#if UNITY_EDITOR
            DebugUpdate();
#endif
        }

        private void LevelCompleted()
        {
            CircumDebug.Assert(_isStarted, "Level has been completed before it's started? What's the deal with that..?");
            CircumDebug.Log("LEVEL COMPLETED");
            
            ApplyLevelElementFunction(p => p.LevelFinished());
            
            Feedbacks.Instance.TriggerFeedback(Feedbacks.FeedbackType.CompletedLevel);

            bool perfectLevel = _players.All(p => p.NoDamageTaken);

            LevelResult levelResult = new LevelResult(true, perfectLevel, new LevelRecordingData
            {
                FrameData = _levelRecorder.FrameData,
                LevelTime = _levelRecorder.LevelTime 
            });

            _levelFinishedCallback?.Invoke(levelResult);
            _isStarted = false;
        }

        private void CheckLevelSuccess()
        {
            foreach (Escape escape in _escapes)
            {
                if (escape.IsCollected)
                {
                    LevelCompleted();
                    return;
                }
            }
        }

        private bool CheckLevelFailed()
        {
            if (!_players.All(p => p.IsDead))
            {
                return false;
            }
            
            LevelResult levelResult = new LevelResult(false, false, null);
            _levelFinishedCallback?.Invoke(levelResult);
            _isStarted = false;
            return true;

        }

        private void CheckEscapeReadyToShow()
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