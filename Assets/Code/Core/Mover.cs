namespace Code.Core
{
    public class Mover : LevelElement
    {
        private float _movementScale = 1f;
        protected float MovementSizeScaler => _movementScale;

        public void SetMovementScale(float movementScale)
        {
            _movementScale = movementScale;
        }
    }
}