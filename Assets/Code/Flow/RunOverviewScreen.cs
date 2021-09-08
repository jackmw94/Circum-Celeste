using Code.Level.Player;
using TMPro;
using UnityEngine;

namespace Code.Flow
{
    public class RunOverviewScreen : MonoBehaviour
    {
        [SerializeField] private GameObject _highestLevelRoot;
        [SerializeField] private GameObject _highestNoDeathsRoot;
        [SerializeField] private GameObject _highestPerfectRoot;
        [Space(15)]
        [SerializeField] private TextMeshProUGUI _highestLevelText;
        [SerializeField] private TextMeshProUGUI _highestNoDeathsText;
        [SerializeField] private TextMeshProUGUI _highestPerfectText;
        
        public void SetupRunOverviewScreen()
        {
            PlayerStats playerStats = PersistentDataManager.Instance.PlayerStats;

            _highestLevelRoot.SetActive(playerStats.HighestLevelIndex >= 0);
            _highestLevelText.text = (playerStats.HighestLevelIndex + 1).ToString();

            _highestNoDeathsRoot.SetActive(playerStats.HighestLevelNoDeathsIndex > 0);
            _highestNoDeathsText.text = (playerStats.HighestLevelNoDeathsIndex + 1).ToString();

            _highestPerfectRoot.SetActive(playerStats.HighestPerfectLevelIndex > 0);
            _highestPerfectText.text = (playerStats.HighestPerfectLevelIndex + 1).ToString();
        }
    }
}