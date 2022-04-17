using Code.Core;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset _latinFont;
        [SerializeField] private TMP_FontAsset _chineseFont;
        [Space(15)]
        [SerializeField] private TextMeshProUGUI _positionLabel;
        [SerializeField] private TextMeshProUGUI _nameLabel;
        [SerializeField] private TextMeshProUGUI _scoreLabel;

        public void SetupLeaderboardEntry(int position, string playerName, int score)
        {
            _positionLabel.text = position.ToString();
            _nameLabel.text = string.IsNullOrEmpty(playerName) ? "-" : playerName;
            _nameLabel.font = playerName.HasChineseCharacters() ? _chineseFont : _latinFont;
            _scoreLabel.text = score.ToString();
        }
    }
}