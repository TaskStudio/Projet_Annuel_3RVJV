using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Transform groundTransform;

    private void OnEnable()
    {
        Vector3 scale = groundTransform.localScale;
        grid.cellSize = new Vector3(1.0f / scale.x, 1.0f / scale.y, 1.0f / scale.z);
        transform.localScale = scale;
    }
}