using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class FogScript : MonoBehaviour
{
    public bool keepPrevious;
    public MeshRenderer mr;

    private List<Unit> units;

    private NativeArray<Color> fullBlack;
    private NativeArray<Color> lowResColors;
    private Texture2D lowResTexture;
    private const int gridSize = 512; // Increased resolution to 128x128 grid
    private const float cellSize = 80.0f / gridSize; // Cell size based on the 80x80 plane
    private const float revealRadius = 2.5f; // Radius of the circular reveal

    // Start is called before the first frame update
    private void Start()
    {
        fullBlack = new NativeArray<Color>(gridSize * gridSize, Allocator.Persistent);

        for (var i = 0; i < gridSize; i++)
        {
            for (var j = 0; j < gridSize; j++)
            {
                fullBlack[gridSize * i + j] = Color.black;
            }
        }

        lowResColors = new NativeArray<Color>(gridSize * gridSize, Allocator.Persistent);
        lowResTexture = new Texture2D(gridSize, gridSize);

        // Find all units in the scene
        FindAllUnits();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!keepPrevious)
        {
            lowResColors.CopyFrom(fullBlack);
        }

        // Update the list of units every frame
        FindAllUnits();

        foreach (var unit in units)
        {
            RevealFogAtUnitPosition(unit.transform.position);
        }

        lowResTexture.SetPixels(lowResColors.ToArray());
        lowResTexture.Apply();
        mr.material.mainTexture = lowResTexture;
    }

    private void OnDestroy()
    {
        fullBlack.Dispose();
        lowResColors.Dispose();
    }

    private void FindAllUnits()
    {
        units = new List<Unit>(FindObjectsOfType<Unit>());
    }

    private void RevealFogAtUnitPosition(Vector3 position)
    {
        int centerX = Mathf.Clamp((int)((position.x + 40.0f) / cellSize), 0, gridSize - 1);
        int centerY = Mathf.Clamp((int)((position.z + 35.0f) / cellSize), 0, gridSize - 1);
        int revealRadiusInCells = Mathf.CeilToInt(revealRadius / cellSize);

        for (int y = -revealRadiusInCells; y <= revealRadiusInCells; y++)
        {
            for (int x = -revealRadiusInCells; x <= revealRadiusInCells; x++)
            {
                int posX = centerX + x;
                int posY = centerY + y;

                if (posX >= 0 && posX < gridSize && posY >= 0 && posY < gridSize)
                {
                    float distance = Mathf.Sqrt(x * x + y * y) * cellSize;
                    if (distance <= revealRadius)
                    {
                        lowResColors[gridSize * posY + posX] = Color.clear;
                    }
                }
            }
        }
    }
}
