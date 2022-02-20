using UnityEngine;
using UnityEngine.UI;

namespace Code.Level
{
    public class ImageLevelOverlay : LevelOverlay
    {
        [SerializeField] private Image _image;

        protected override Material GetMaterial()
        {
            return _image.material;
        }

        protected override void SetColliderOnOff(bool on)
        {
            _image.raycastTarget = on;
        }
    }
}