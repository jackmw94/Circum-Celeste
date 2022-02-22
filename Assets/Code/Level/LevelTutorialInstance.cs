using System.Collections;
using System.Linq;
using Code.Core;
using Code.Debugging;
using UnityEngine;
using UnityExtras.Code.Optional.EasingFunctions;
using UnityExtras.Core;

namespace Code.Level
{
    public class LevelTutorialInstance : LevelPlayInstance
    {
        private const float ShowStarDuration = 0.66f;
        private const float ShowStarInitialRotation = 12f;
        
        private TutorialDescription _tutorialDescription;

        private int _tutorialIndex = -1;
        private bool _hasShownPickups = false;
        
        public void InitialiseTutorial(TutorialDescription tutorialDescription)
        {
            _tutorialDescription = tutorialDescription;
            
            SetupTutorialCommand(new TutorialDescription.TutorialCommand());

            _players.ApplyFunction(p => p.TutorialDisableOnStayDamage());
        }

        protected override void OnLevelReady()
        {
            base.OnLevelReady();
            AdvanceTutorialCommand();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            TutorialInstruction.Instance.Hide();
        }

        private void AdvanceTutorialCommand()
        {
            _tutorialIndex++;
            if (_tutorialIndex >= _tutorialDescription.TutorialCommands.Length)
            {
                LevelCompleted();
                TutorialInstruction.Instance.SetText(string.Empty);
                return;
            }

            TutorialDescription.TutorialCommand nextTutorialCommand = _tutorialDescription.TutorialCommands[_tutorialIndex];
            SetupTutorialCommand(nextTutorialCommand);

            string localisedInstruction = nextTutorialCommand.GetLocalisedInstruction();
            CircumDebug.Log(localisedInstruction);
            TutorialInstruction.Instance.SetText(localisedInstruction);

            StartCoroutine(AdvanceTutorialCommandOnceCompleted(nextTutorialCommand));
        }

        private void SetupTutorialCommand(TutorialDescription.TutorialCommand command)
        {
            _enemies.ApplyFunction(p => p.gameObject.SetActiveSafe(command.BlackHolesEnabled));
            _escapes.ApplyFunction(p => p.gameObject.SetActiveSafe(!p.IsCollected && command.EscapeEnabled));
            _hazards.ApplyFunction(p => p.gameObject.SetActiveSafe(!p.IsCollected && command.HazardsEnabled));

            if (command.PickupsEnabled && !_hasShownPickups)
            {
                _hasShownPickups = true;
                StartCoroutine(ShowPickups());
            }
            else
            {
                _pickups.ApplyFunction(p => p.gameObject.SetActiveSafe(!p.IsCollected && command.PickupsEnabled));
            }

            _players.ApplyFunction(p => p.OrbiterTransform.gameObject.SetActiveSafe(command.OrbiterEnabled));
            _players.ApplyFunction(p => p.SetInvulnerable(!command.OrbiterDamageEnabled));
            
            GameContainer.Instance.UIInputElementsContainer.IntroduceMoverHandle.SetIntroducing(command.IntroduceElement == IntroduceElement.MovementHandle);
        }

        private IEnumerator ShowPickups(bool debug = false)
        {
            if (debug)
            {
                _pickups.ApplyFunction(pickup => pickup.gameObject.SetActiveSafe(false));
                yield return new WaitForSeconds(3f);
            }
            
            foreach (Pickup pickup in _pickups)
            {
                StartCoroutine(AnimatePickupEntry(pickup.transform));
                yield return new WaitForSeconds(0.15f);
            }
        }
        
        private IEnumerator AnimatePickupEntry(Transform pickup)
        {
            float targetScale = pickup.localScale.x;
            
            pickup.gameObject.SetActiveSafe(true);
            pickup.localScale = Vector3.zero;

            yield return Utilities.LerpOverTime(0f, 1f, ShowStarDuration, f =>
            {
                float linearScale = Mathf.Lerp(0f, targetScale, f);
                float easedScale = EasingFunctions.ConvertLinearToEased(EasingFunctions.EasingType.Sine, EasingFunctions.EasingDirection.InAndOut, linearScale);
                pickup.localScale = easedScale * Vector3.one;

                float easedRotationAmount = EasingFunctions.ConvertLinearToEased(EasingFunctions.EasingType.Sine, EasingFunctions.EasingDirection.InAndOut, f);
                float rotationAmount = 1f - easedRotationAmount;
                pickup.Rotate(Vector3.forward, ShowStarInitialRotation * rotationAmount);
            });
        }

        protected override void Update()
        {
            _players.ApplyFunction(p =>
            {
                if (p.CurrentHealth <= 1)
                {
                    p.TutorialResetHealth();
                }
            });
            
            
            //todo: delete this!
            if (Input.GetKeyDown(KeyCode.N))
            {
                AdvanceTutorialCommand();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(ShowPickups(true));
            }
            
#if UNITY_EDITOR
            DebugUpdate();
#endif
        }

        private IEnumerator AdvanceTutorialCommandOnceCompleted(TutorialDescription.TutorialCommand command)
        {
            switch (command.CompleteCommandCriteria)
            {
                case TutorialDescription.TutorialCommand.TutorialCommandCompletionCriteria.Timer:
                    yield return new WaitForSeconds(command.Duration);
                    break;
                case TutorialDescription.TutorialCommand.TutorialCommandCompletionCriteria.IsMoving:
                    yield return new WaitUntil(() => _players.Any(p => p.IsMoving));
                    yield return new WaitForSeconds(command.Duration);
                    break;
                case TutorialDescription.TutorialCommand.TutorialCommandCompletionCriteria.AllPickupsCollected:
                    yield return new WaitUntil(() => _pickups.All(p => p.IsCollected));
                    yield return new WaitForSeconds(command.Duration);
                    break;
                case TutorialDescription.TutorialCommand.TutorialCommandCompletionCriteria.EnteredEscape:
                    yield return new WaitUntil(() => _escapes.Any(p => p.IsCollected));
                    yield return new WaitForSeconds(command.Duration);
                    break;
            }
            
            AdvanceTutorialCommand();
        }
    }
}