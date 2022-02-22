using System;
using Code.Debugging;
using Code.Flow;
using Code.Level.Player;
using UnityEngine;
using UnityExtras.Core;

namespace Code.UI
{
    public class BadgeIndicator : MonoBehaviour
    {
        [SerializeField] private LevelBadge _goldTimeBadge;
        [SerializeField] private LevelBadge _perfectBadge;
        [SerializeField] private LevelBadge _perfectGoldTimeBadge;
        [SerializeField] private GameObject _noBadgesIndicator;

        public void SetupBadgeIndicator(BadgeData currentBadgeData, BadgeData newBadgeData = default, Action onPerfectGoldShowComplete = null)
        {
            CircumDebug.Assert(currentBadgeData.IsPerfect || !newBadgeData.IsPerfect, "Arguments say this level was NOT perfect but WAS the first perfect. Unexpected.");
            CircumDebug.Assert(currentBadgeData.HasGoldTime || !newBadgeData.HasGoldTime, "Arguments say this level was NOT perfect but WAS the first perfect. Unexpected.");
            CircumDebug.Assert(currentBadgeData.HasPerfectGoldTime || !newBadgeData.HasPerfectGoldTime, "Arguments say this level was NOT perfect but WAS the first perfect. Unexpected.");

            _perfectGoldTimeBadge.ShowHideBadge(currentBadgeData.HasPerfectGoldTime, !newBadgeData.HasPerfectGoldTime, onPerfectGoldShowComplete);

            if (currentBadgeData.HasPerfectGoldTime)
            {
                _perfectBadge.ShowHideBadge(false, true);
                _goldTimeBadge.ShowHideBadge(false, true);
                TryShowHideNoBadgesIndicator(false);
            }
            else
            {
                _perfectBadge.ShowHideBadge(currentBadgeData.IsPerfect, !newBadgeData.IsPerfect);
                _goldTimeBadge.ShowHideBadge(currentBadgeData.HasGoldTime, !newBadgeData.HasGoldTime);
                TryShowHideNoBadgesIndicator(!currentBadgeData.IsPerfect && !currentBadgeData.HasGoldTime);
            }
        }

        private void TryShowHideNoBadgesIndicator(bool show)
        {
            if (_noBadgesIndicator)
            {
                _noBadgesIndicator.SetActiveSafe(show);
            }
        }
    }
}