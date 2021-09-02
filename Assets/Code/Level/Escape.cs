using System;
using Code.Core;
using Code.Level.Player;
using Code.VFX;
using UnityEngine;

namespace Code.Level
{
    public class Escape : Collectable
    {
        private Action _onEscapeEntered = null;

        public void SetEscapeCallback(Action onEscapedCallback)
        {
            _onEscapeEntered = onEscapedCallback;
        }

        protected override bool CanObjectCollect(GameObject other)
        {
            return other.IsPlayer();
        }
        
        protected override void CollectableCollected(Vector3 _)
        {
            VfxManager.Instance.SpawnVfx(VfxType.PlayerEscaped, transform.position);
            _onEscapeEntered?.Invoke();
        }

        protected override bool DoesReplayCollect(LevelRecordFrameData frameReplay, int index, out Vector3 hitFrom)
        {
            throw new NotImplementedException();
        }
    }
}