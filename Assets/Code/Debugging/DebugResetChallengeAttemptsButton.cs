using Code.Level.Player;
using UnityCommonFeatures;

namespace Code.Debugging
{
    public class DebugResetChallengeAttemptsButton : ButtonBehaviour
    {
        protected override void OnButtonClicked()
        {
            ChallengeData challengeData = PersistentDataManager.Instance.ChallengeData;
            challengeData.AttemptData.AttemptCount = 0;
            ChallengeData.Save(challengeData);
        }
    }
}