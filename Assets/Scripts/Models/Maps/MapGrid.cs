using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Transform groundTransform;
    [SerializeField] private Material gridMaterial;

    public Grid Grid => grid;

    public void SetGridCellSize(float cellSize)
    {
        Vector3 scale = groundTransform.localScale;

        gridMaterial.SetVector("_Size", new Vector4(1.0f / cellSize, 1.0f / cellSize, 0, 0));
        grid.cellSize = new Vector3(cellSize / scale.x, 1.0f / scale.y, cellSize / scale.z);
        transform.localScale = scale;
    }
}