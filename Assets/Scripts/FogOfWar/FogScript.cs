using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class FogScript : MonoBehaviour
{
    public bool keepPrevious;
    public MeshRenderer mr;
    public ComputeShader fogComputeShader;
    public float minMoveDistance = 0.3f; // Minimum distance to move before updating the fog

    private List<Unit> units;
    private Dictionary<Unit, Vector3> lastUnitPositions;

    private NativeArray<Color> fullBlack;
    private Texture2D lowResTexture;
    private ComputeBuffer unitsBuffer;
    private RenderTexture resultTexture;
    private NativeArray<Vector3> unitPositions;
    private const int gridSize = 512;
    private const float cellSize = 80.0f / gridSize;
    private const float revealRadius = 2.5f;

    private void Awake()
    {
        units = new List<Unit>(FindObjectsOfType<Unit>());
        lastUnitPositions = new Dictionary<Unit, Vector3>();
    }

    private void Start()
    {
        InitializeFullBlackArray();
        InitializeTexturesAndBuffers();
        InitializeLastUnitPositions();
    }

    private void Update()
    {
        if (UnitsHaveMoved())
        {
            UpdateUnitsBuffer();
            DispatchComputeShader();
            UpdateTexture();
        }
    }

    private void OnDestroy()
    {
        CleanupResources();
    }

    private void InitializeFullBlackArray()
    {
        fullBlack = new NativeArray<Color>(gridSize * gridSize, Allocator.Persistent);
        for (var i = 0; i < gridSize * gridSize; i++)
        {
            fullBlack[i] = Color.black;
        }
    }

    private void InitializeTexturesAndBuffers()
    {
        lowResTexture = new Texture2D(gridSize, gridSize, TextureFormat.RGBA32, false);
        resultTexture = new RenderTexture(gridSize, gridSize, 0, RenderTextureFormat.ARGB32)
        {
            enableRandomWrite = true,
            useMipMap = false,
            filterMode = FilterMode.Point
        };
        resultTexture.Create();

        unitsBuffer = new ComputeBuffer(1000, sizeof(float) * 3); // Adjust the size based on expected number of units
        unitPositions = new NativeArray<Vector3>(unitsBuffer.count, Allocator.Persistent);
    }

    private void InitializeLastUnitPositions()
    {
        foreach (var unit in units)
        {
            lastUnitPositions[unit] = unit.transform.position;
        }
    }

    private void UpdateUnitsBuffer()
    {
        if (units.Count > unitsBuffer.count)
        {
            unitsBuffer.Dispose();
            unitPositions.Dispose();
            unitsBuffer = new ComputeBuffer(units.Count, sizeof(float) * 3);
            unitPositions = new NativeArray<Vector3>(unitsBuffer.count, Allocator.Persistent);
        }

        for (int i = 0; i < units.Count; i++)
        {
            unitPositions[i] = units[i].transform.position;
        }
        unitsBuffer.SetData(unitPositions);
    }

    private void DispatchComputeShader()
    {
        fogComputeShader.SetTexture(0, "Result", resultTexture);
        fogComputeShader.SetBuffer(0, "UnitPositions", unitsBuffer);
        fogComputeShader.SetFloat("cellSize", cellSize);
        fogComputeShader.SetFloat("revealRadius", revealRadius);
        fogComputeShader.SetInt("gridSize", gridSize);
        fogComputeShader.SetBool("keepPrevious", keepPrevious);

        fogComputeShader.Dispatch(0, gridSize / 8, gridSize / 8, 1);
    }

    private void UpdateTexture()
    {
        RenderTexture.active = resultTexture;
        lowResTexture.ReadPixels(new Rect(0, 0, gridSize, gridSize), 0, 0);
        lowResTexture.Apply();
        mr.material.mainTexture = lowResTexture;
    }

    private void CleanupResources()
    {
        fullBlack.Dispose();
        unitsBuffer.Dispose();
        unitPositions.Dispose();
        resultTexture.Release();
    }

    private bool UnitsHaveMoved()
    {
        foreach (var unit in units)
        {
            var currentPosition = unit.transform.position;
            if (Vector3.Distance(currentPosition, lastUnitPositions[unit]) > minMoveDistance)
            {
                lastUnitPositions[unit] = currentPosition;
                return true;
            }
        }
        return false;
    }
}
