using UnityEngine;

public class LevelCell : MonoBehaviour
{
    public void Initialise(Vector2Int cellCoordinates, float gridSize)
    {
        float size = 10f / gridSize;
        Transform cellTransform = transform;
        cellTransform.localScale = Vector3.one * size;
        cellTransform.localPosition = new Vector3(cellCoordinates.x, cellCoordinates.y, 0f) * size;
    }
}