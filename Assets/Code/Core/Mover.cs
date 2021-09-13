namespace Code.Core
{
    public class Mover : LevelElement
    {
        private float _movementScale = 1f;
        protected virtual float MovementSizeScaler => _movementScale;

        public void SetMovementScale(float movementScale)
        {
            _movementScale = movementScale;
        }
    }
}