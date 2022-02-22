using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Debugging;
using Code.Juice;
using Code.UI;
using UnityEngine;
using UnityExtras.Core;

namespace Code.Level
{
    public class LevelPlayInstance : LevelInstanceBase
    {
        private EscapeCriteria _escapeCriteria;
        private float _escapeDuration;
        private IntroduceElement _introduceElement;

        protected List<Player.Player> _players;
        protected List<Pickup> _pickups;
        protected List<Enemy> _enemies;
        protected List<Escape> _escapes;
        protected List<Hazard> _hazards;
        protected List<BeamHazard> _beamHazards;

        private float _startTime;

        private LevelRecorder _levelRecorder;

        private bool _escapeShown;
        private float LevelTime => Time.time - _startTime;
        public override bool PlayerStartedPlaying => _players.Any(p => p.IsMoving);

        public void SetupLevel(
            LevelLayout levelLayout, 
            List<Player.Player> players,
            List<Pickup> pickups, 
            List<Enemy> enemies, 
            List<Escape> escapes,
            List<Hazard> hazards,
            List<BeamHazard> beamHazards,
            float speedScale, 
            int gridSize)
        {
            _escapeCriteria = levelLayout.EscapeCriteria;
            _escapeDuration = levelLayout.EscapeTimer;
            _introduceElement = levelLayout.IntroduceElement;

            _players = players;
            _pickups = pickups;
            _enemies = enemies;
            _escapes = escapes;
            _hazards = hazards;
            _beamHazards = beamHazards;

            CircumDebug.Log($"Grid size = {gridSize}");
            _players.ApplyFunction(p => p.SetupForGridSize(gridSize));
            GetComponentsInChildren<Mover>().ApplyFunction(p => p.SetMovementScale(speedScale));

            GameContainer.Instance.CountdownTimerUI.ResetTimer();

            _escapeShown = false;

            ApplyLevelElementFunction(p => p.LevelSetup());
        }

        protected override void OnLevelReady()
        {
            CircumDebug.Log($"Level '{name}' ready");

            ApplyLevelElementFunction(p => p.LevelReady());

            // assuming this will always count up, therefore reset == hidden
            GameContainer gameContainer = GameContainer.Instance;
            gameContainer.CountdownTimerUI.ResetTimer();

            HandleReadyIntroductions();
        }

        protected override void OnStartLevel()
        {
            _startTime = Time.time;

            if (_escapeCriteria == EscapeCriteria.Timed)
            {
                GameContainer.Instance.CountdownTimerUI.StartTimer(_escapeDuration);
            }

            ApplyLevelElementFunction(p => p.LevelStarted());

            _levelRecorder = gameObject.AddComponent<LevelRecorder>();
            
            HandleStartedIntroductions();
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
            _beamHazards.ApplyFunction(levelElementFunction);
        }

        private void HandleReadyIntroductions()
        {
            UIInputElementsContainer movementUi = GameContainer.Instance.UIInputElementsContainer;

            movementUi.IntroducePowerButton.SetIntroducing(_introduceElement.HasFlag(IntroduceElement.PowerButton));
            movementUi.IntroduceMoverHandle.SetIntroducing(_introduceElement.HasFlag(IntroduceElement.MovementHandle));
            movementUi.IntroduceOrbiter.SetIntroducing(_introduceElement.HasFlag(IntroduceElement.Orbiter));
        }

        private void HandleStartedIntroductions()
        {
            _escapes.ApplyFunction(p => p.SetIntroducing(_introduceElement.HasFlag(IntroduceElement.Escape)));
        }

        protected virtual void Update()
        {
            if (!IsStarted)
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

        protected void LevelCompleted()
        {
            CircumDebug.Assert(IsStarted, "Level has been completed before it's started? What's the deal with that..?");
            CircumDebug.Log($"-*- LEVEL {name} COMPLETED! -*-");
            
            ApplyLevelElementFunction(p => p.LevelFinished());
            
            Feedbacks.Instance.TriggerFeedback(Feedbacks.FeedbackType.CompletedLevel);

            bool perfectLevel = _players.All(p => p.NoDamageTaken);

            LevelResult levelResult = new LevelResult(true, new LevelRecordingData
            {
                FrameData = _levelRecorder.FrameData,
                LevelTime = _levelRecorder.LevelTime,
                IsPerfect = perfectLevel
            });

            LevelFinished(levelResult);
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
            
            LevelResult levelResult = new LevelResult(false, new LevelRecordingData()
            {
                FrameData = null,
                LevelTime = _levelRecorder.LevelTime,
                IsPerfect = false
            });
            LevelFinished(levelResult);
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
        
        protected void ShowEscape()
        {
            CircumDebug.Log("Showing escapes");
            _escapeShown = true;
            _escapes.ApplyFunction(p => p.gameObject.SetActive(true));
        }

#if UNITY_EDITOR
        protected void DebugUpdate()
        {
            if (Input.GetKeyDown(EditorKeyCodeBindings.ShowEscape))
            {
                CircumDebug.Log("Debug showing escapes");
                ShowEscape();
            }
        }
#endif
    }
}