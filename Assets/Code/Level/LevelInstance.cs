using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityExtras.Code.Core;

namespace Code.Level
{
    public class LevelInstance : MonoBehaviour
    {
        private EscapeCriteria _escapeCriteria;
        private float _escapeDuration;

        private List<Pickup> _pickups;
        private List<Enemy> _enemies;
        private List<Escape> _escapes;

        private bool _isStarted;
        private float _startTime;

        private bool _escapeShown;

        private float LevelTime => Time.time - _startTime;
        
        public void SetupLevel(LevelLayout levelLayout, List<Pickup> pickups, List<Enemy> enemies, List<Escape> escapes)
        {
            _escapeCriteria = levelLayout.EscapeCriteria;
            _escapeDuration = levelLayout.EscapeTimer;

            _pickups = pickups;
            _enemies = enemies;
            _escapes = escapes;

            _escapes.ApplyFunction(InitialiseEscape);
            _enemies.ApplyFunction(InitialiseEnemies);

            _isStarted = false;
            _escapeShown = false;
        }

        public void StartLevel()
        {
            _isStarted = true;
            _startTime = Time.time;

            _enemies.ApplyFunction(p => p.CanMove = true);
        }

        private void LevelCompleted()
        {
            Debug.Assert(_isStarted, "Level has been completed before it's started? What's the deal with that..?");
            Debug.Log("LEVEL COMPLETED");
        }

        private void Update()
        {
            CheckEscape();
            
#if UNITY_EDITOR
            DebugUpdate();
#endif
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

        private void InitialiseEnemies(Enemy enemy)
        {
            enemy.CanMove = false;
        }

        private void ShowEscape()
        {
            _escapes.ApplyFunction(p => p.gameObject.SetActive(true));
        }

#if UNITY_EDITOR
        private void DebugUpdate()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Debug showing escapes");
                ShowEscape();
            }
        }
#endif
    }
}