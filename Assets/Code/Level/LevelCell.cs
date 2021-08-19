using UnityEngine;

namespace Code.Level
{
    public static class LevelCellHelper
    {
        public const float RealGridDimension = 10f;
        
        public static void Initialise(Transform transform, Vector2Int cellCoordinates, float gridSize)
        {
            float size = RealGridDimension / gridSize;
            Transform cellTransform = transform;
            cellTransform.localScale = Vector3.one * size;
            
            // origin is at top left so y should be negative
            float cellY = -(cellCoordinates.y + 1);
            cellTransform.localPosition = new Vector3(cellCoordinates.x, cellY, 0f) * size + new Vector3(size / 2f, size / 2f, 0f);
        }
    }
}