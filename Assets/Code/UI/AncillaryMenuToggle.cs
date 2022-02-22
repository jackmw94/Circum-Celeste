using System.Collections.Generic;
using System.Linq;
using Code.Level;
using UnityCommonFeatures;
using UnityEngine;
using UnityExtras.Core;

namespace Code.UI
{
    public class AncillaryMenuToggle : ButtonBehaviour
    {
        [SerializeField] private UnityCommonFeatures.UIPanel _ancillaryMenu;
        [SerializeField] private UnityCommonFeatures.UIPanel[] _otherAncillaryMenus;
        [SerializeField] private LevelOverlay _ancillaryMenuOverlay;
        [SerializeField] private Color _targetColour;
        
        protected override void OnButtonClicked()
        {
            VisibleState currentPanelState = _ancillaryMenu.CurrentPanelState;
            bool shouldShow = currentPanelState == VisibleState.Hidden ||
                              currentPanelState == VisibleState.ChangingToHidden;
            ShowHideAncillaryMenu(shouldShow);
        }

        private void ShowHideAncillaryMenu(bool show)
        {
            LevelOverlay.OverlayTransitionConfiguration overlayTransitionConfiguration = new LevelOverlay.OverlayTransitionConfiguration
            {
                Position = Vector2.zero,
                TargetColour = show ? _targetColour : Color.white,
                Twirl = true
            };

            // assuming we're not going to have lots of menus on at the same time
            // therefore just have to wait until first found menu is finished
            IEnumerable<UnityCommonFeatures.UIPanel> otherMenusShowing = _otherAncillaryMenus.Where(p => p.IsShowing).ToArray();
            if (otherMenusShowing.Any())
            {
                otherMenusShowing.First().Hide(onComplete: () =>
                {
                    _ancillaryMenu.ShowHide(show);
                    _ancillaryMenuOverlay.ShowHideOverlay(show, overlayTransitionConfiguration);
                });
                otherMenusShowing.Skip(1).ApplyFunction(p => p.Hide());
            }
            else
            {
                _ancillaryMenu.ShowHide(show);
                _ancillaryMenuOverlay.ShowHideOverlay(show, overlayTransitionConfiguration);
            }
        }
    }
}