using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

public class FogOfWarManager : MonoBehaviour
{
    public int textureSize = 512;
    public float worldSize = 80.0f;
    public LayerMask fogLayer;
    public float visibilityRadius = 5.0f;
    public MeshRenderer meshRenderer;
    public bool keepPrevious; // Add this variable

    private Texture2D fogTexture;
    private NativeArray<Color> fogColors;
    private NativeArray<Color> fullBlack;
    private float fogUpdateRate = 0.1f;
    private float fogUpdateTimer;

    private List<Unit> units;

    public static FogOfWarManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        fogTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        fogColors = new NativeArray<Color>(textureSize * textureSize, Allocator.Persistent);
        fullBlack = new NativeArray<Color>(textureSize * textureSize, Allocator.Persistent);

        for (int i = 0; i < fullBlack.Length; i++)
        {
            fullBlack[i] = new Color(0, 0, 0, 1); // Fully opaque black
        }

        fogColors.CopyFrom(fullBlack);
        fogTexture.SetPixels(fogColors.ToArray());
        fogTexture.Apply();
        
        meshRenderer.material.SetTexture("_MainTex", fogTexture);

        units = new List<Unit>(FindObjectsOfType<Unit>());
        if (units.Count == 0)
        {
            Debug.LogWarning("No units found in the scene.");
        }
    }

    void Update()
    {
        fogUpdateTimer += Time.deltaTime;
        if (fogUpdateTimer >= fogUpdateRate)
        {
            fogUpdateTimer = 0f;
            UpdateFogOfWar();
        }
    }

    void UpdateFogOfWar()
    {
        Debug.Log("Updating Fog of War");

        if (!keepPrevious)
        {
            fogColors.CopyFrom(fullBlack);
        }

        foreach (Unit unit in units)
        {
            if (unit != null && unit.gameObject.activeInHierarchy)
            {
                Vector3 unitPosition = unit.transform.position;
                Vector2Int unitPosInTexture = WorldToTextureCoords(unitPosition);
                Debug.Log($"Unit Position: {unitPosition}, Texture Coords: {unitPosInTexture}");

                int radius = Mathf.CeilToInt(visibilityRadius * textureSize / worldSize);
                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {
                        int newX = Mathf.Clamp(unitPosInTexture.x + x, 0, textureSize - 1);
                        int newY = Mathf.Clamp(unitPosInTexture.y + y, 0, textureSize - 1);
                        float distance = Vector2.Distance(new Vector2(newX, newY), unitPosInTexture);
                        if (distance < visibilityRadius * textureSize / worldSize)
                        {
                            fogColors[newY * textureSize + newX] = new Color(0, 0, 0, 0); // Clear color with zero alpha
                        }
                    }
                }
            }
        }

        fogTexture.SetPixels(fogColors.ToArray());
        fogTexture.Apply();
        meshRenderer.material.SetTexture("_MainTex", fogTexture); // Ensure texture is applied
    }

    Vector2Int WorldToTextureCoords(Vector3 worldPos)
    {
        float normalizedX = (worldPos.x + worldSize * 0.5f) / worldSize;
        float normalizedY = (worldPos.z + worldSize * 0.5f) / worldSize;
        int texX = Mathf.FloorToInt(normalizedX * textureSize);
        int texY = Mathf.FloorToInt(normalizedY * textureSize);
        return new Vector2Int(texX, texY);
    }

    public void RegisterUnit(Unit unit)
    {
        if (units == null)
        {
            units = new List<Unit>();
        }

        if (!units.Contains(unit))
        {
            units.Add(unit);
        }
    }

    private void OnDestroy()
    {
        if (fogColors.IsCreated)
        {
            fogColors.Dispose();
        }
        if (fullBlack.IsCreated)
        {
            fullBlack.Dispose();
        }
    }
}
