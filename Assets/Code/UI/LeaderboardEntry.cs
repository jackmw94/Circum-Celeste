using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _positionLabel;
        [SerializeField] private TextMeshProUGUI _nameLabel;
        [SerializeField] private TextMeshProUGUI _scoreLabel;

        public void SetupLeaderboardEntry(int position, string playerName, int score)
        {
            _positionLabel.text = position.ToString();
            _nameLabel.text = string.IsNullOrEmpty(playerName) ? "[no name]" : playerName;
            _scoreLabel.text = score.ToString();
        }
    }
}