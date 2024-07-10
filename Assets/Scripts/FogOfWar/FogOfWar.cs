using UnityEngine;

namespace FogOfWar
{
    public class FogOfWar : MonoBehaviour
    {
        [SerializeField]
        private Material _fogMaterial;
        [SerializeField]
        private float _cellSize = 1f;
        [SerializeField]
        private Transform _planeTransform;
        [SerializeField]
        private int _gridSize = 256; // Smaller grid size for easier debugging

        private Texture2D _fogTexture;
        private Color[] _fogColors;
        private bool[,] _visibilityGrid;

        private void Start()
        {
            if (_fogMaterial == null)
            {
                Debug.LogError("Fog material not assigned.");
                return;
            }

            _fogTexture = new Texture2D(_gridSize, _gridSize, TextureFormat.RGBA32, false);
            _fogColors = new Color[_gridSize * _gridSize];
            _visibilityGrid = new bool[_gridSize, _gridSize];

            for (int i = 0; i < _fogColors.Length; i++)
            {
                _fogColors[i] = Color.black;
            }

            _fogTexture.SetPixels(_fogColors);
            _fogTexture.Apply();
            _fogMaterial.SetTexture("_MainTex", _fogTexture);

            Debug.Log("Fog of War initialized.");
        }

        private void Update()
        {
            ClearFogAroundUnits();
            UpdateFogTexture();
        }

        private void ClearFogAroundUnits()
        {
            Unit[] units = FindObjectsOfType<Unit>();
            Debug.Log($"Number of units found: {units.Length}");
            foreach (var unit in units)
            {
                Vector3 unitPosition = unit.transform.position;
                Debug.Log($"Clearing fog at position: {unitPosition}");
                ClearFogAtPosition(unitPosition);
            }
        }

        private void ClearFogAtPosition(Vector3 position)
        {
            // Calculate grid coordinates
            int centerX = Mathf.FloorToInt((position.x + 40f) / 80f * _gridSize);
            int centerY = Mathf.FloorToInt((position.z + 40f) / 80f * _gridSize);
            int radius = Mathf.CeilToInt(_cellSize / 80f * _gridSize);

            Debug.Log($"World position: {position}, Local position: ({centerX}, {centerY}), Radius: {radius}");

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    int px = centerX + x;
                    int py = centerY + y;

                    if (px >= 0 && px < _gridSize && py >= 0 && py < _gridSize)
                    {
                        _visibilityGrid[px, py] = true;
                        Debug.Log($"World position: {position}, Local position: ({centerX}, {centerY}), Radius: {radius}");
                        Debug.Log($"Clearing grid cell at: ({px}, {py})");
                    }
                }
            }
        }

        private void UpdateFogTexture()
        {
            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    _fogColors[y * _gridSize + x] = _visibilityGrid[x, y] ? Color.clear : Color.black;
                }
            }

            _fogTexture.SetPixels(_fogColors);
            _fogTexture.Apply();
        }

        private void OnDrawGizmos()
        {
            if (_fogColors == null)
                return;

            Unit[] units = FindObjectsOfType<Unit>();
            foreach (var unit in units)
            {
                Vector3 unitPosition = unit.transform.position;
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(unitPosition, _cellSize);
            }
        }
    }
}
