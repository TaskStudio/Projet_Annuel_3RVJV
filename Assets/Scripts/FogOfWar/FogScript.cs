using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace FogOfWar
{
    public class FogScript : MonoBehaviour
    {
        private static readonly int GridSize = 1024;
        private static float CellSize;
        private static readonly float RevealRadiusResourceStorages = 5.0f;
        private static readonly float RevealRadiusVisionTowers = 15.0f;
        private static readonly float RevealRadiusFactories = 10.0f;
        public bool keepPrevious;
        public MeshRenderer mr;
        public ComputeShader computeShader;
        public GameObject GroundPlane;
        public GameObject Plane;
        private int _cellSizeID;
        private int _clearColorID;
        private List<Building> _factories;
        private ComputeBuffer _factoryPositionBuffer;
        private int _factoryPositionsID;

        private NativeArray<Color> _fullBlack;
        private int _fullBlackColorID;
        private int _gridSizeID;

        private int _kernelHandle;
        private Texture2D _lowResTexture;
        private RenderTexture _renderTexture;
        private ComputeBuffer _resourceStoragePositionBuffer;
        private int _resourceStoragePositionsID;
        private List<Building> _resourceStorages;
        private int _resultID;
        private int _revealRadiusFactoriesID;
        private int _revealRadiusResourceStoragesID;
        private int _revealRadiusVisionTowersID;
        private ComputeBuffer _unitPositionBuffer;
        private int _unitPositionsID;

        private List<Unit> _units;
        private ComputeBuffer _visionTowerPositionBuffer;
        private int _visionTowerPositionsID;
        private List<Building> _visionTowers;

        private ComputeBuffer _enemyPositionBuffer;
        private int _enemyPositionsID;
        private List<Unit> _enemies;
        
        private Dictionary<Vector2Int, bool> clearedZoneCache;
        private float cacheUpdateInterval = 0.5f;
        private float nextCacheUpdateTime;

        private void Start()
        {
            computeShader.SetFloat("groundPlaneScaleX", GroundPlane.transform.localScale.x);
            computeShader.SetFloat("groundPlaneScaleZ", GroundPlane.transform.localScale.z);
            clearedZoneCache = new Dictionary<Vector2Int, bool>();

            if (GroundPlane != null && Plane != null)
            {
                Plane.transform.localScale = GroundPlane.transform.localScale;
                CellSize = GroundPlane.transform.localScale.x * 10.0f / GridSize;
            }

            InitializeBuffers();

            _fullBlack = new NativeArray<Color>(GridSize * GridSize, Allocator.Persistent);
            for (var i = 0; i < GridSize; i++)
            for (var j = 0; j < GridSize; j++)
                _fullBlack[GridSize * i + j] = new Color(0, 0, 0, 0.8f); 

            _lowResTexture = new Texture2D(GridSize, GridSize);
            _renderTexture = new RenderTexture(GridSize, GridSize, 0)
            {
                enableRandomWrite = true
            };
            _renderTexture.Create();

            _kernelHandle = computeShader.FindKernel("CSMain");
            if (_kernelHandle < 0) return;

            InitializeShaderProperties();
            FindAllObjects();

            // Assign the render texture to the fog plane
            if (mr != null) mr.material.mainTexture = _renderTexture;
        }

        private void Update()
        {
            if (_kernelHandle < 0) return;

            if (!keepPrevious && _renderTexture != null) Graphics.Blit(Texture2D.blackTexture, _renderTexture);

            FindAllUnits();
            FindAllResourceStorages();
            FindAllVisionTowers();
            FindAllFactories();
            FindAllEnemies();

            var unitPositions = new Vector4[_units.Count];
            for (var i = 0; i < _units.Count; i++)
            {
                var pos = _units[i].transform.position;
                unitPositions[i] = new Vector4(pos.x, pos.z, 0.0f, _units[i].Data.detectionRange); 
            }

            _unitPositionBuffer.Release();
            _unitPositionBuffer = new ComputeBuffer(_units.Count > 0 ? _units.Count : 1, 16);
            _unitPositionBuffer.SetData(unitPositions.Length > 0 ? unitPositions : new Vector4[1]);

            var resourceStoragePositions = new Vector4[_resourceStorages.Count > 0 ? _resourceStorages.Count : 1];
            for (var i = 0; i < _resourceStorages.Count; i++)
            {
                var centerPos = _resourceStorages[i].transform.position;
                resourceStoragePositions[i] = new Vector4(centerPos.x, centerPos.z, 0.0f, 0.0f);
            }

            if (_resourceStorages.Count == 0) resourceStoragePositions[0] = new Vector4(-9999, -9999, 0, 0);
            _resourceStoragePositionBuffer.Release();
            _resourceStoragePositionBuffer = new ComputeBuffer(resourceStoragePositions.Length, 16);
            _resourceStoragePositionBuffer.SetData(resourceStoragePositions);

            var visionTowerPositions = new Vector4[_visionTowers.Count > 0 ? _visionTowers.Count : 1];
            for (var i = 0; i < _visionTowers.Count; i++)
            {
                var centerPos = _visionTowers[i].transform.position;
                visionTowerPositions[i] = new Vector4(centerPos.x, centerPos.z, 0.0f, 0.0f);
            }

            if (_visionTowers.Count == 0) visionTowerPositions[0] = new Vector4(-9999, -9999, 0, 0);
            _visionTowerPositionBuffer.Release();
            _visionTowerPositionBuffer = new ComputeBuffer(visionTowerPositions.Length, 16);
            _visionTowerPositionBuffer.SetData(visionTowerPositions);

            var factoryPositions = new Vector4[_factories.Count > 0 ? _factories.Count : 1];
            for (var i = 0; i < _factories.Count; i++)
            {
                var centerPos = _factories[i].transform.position;
                factoryPositions[i] = new Vector4(centerPos.x, centerPos.z, 0.0f, 0.0f);
            }

            if (_factories.Count == 0) factoryPositions[0] = new Vector4(-9999, -9999, 0, 0);
            _factoryPositionBuffer.Release();
            _factoryPositionBuffer = new ComputeBuffer(factoryPositions.Length, 16);
            _factoryPositionBuffer.SetData(factoryPositions);

            var enemyPositions = new Vector4[_enemies.Count];
            for (var i = 0; i < _enemies.Count; i++)
            {
                var pos = _enemies[i].transform.position;
                enemyPositions[i] = new Vector4(pos.x, pos.z, 0.0f, 0.0f); 
            }

            _enemyPositionBuffer.Release();
            _enemyPositionBuffer = new ComputeBuffer(_enemies.Count > 0 ? _enemies.Count : 1, 16);
            _enemyPositionBuffer.SetData(enemyPositions.Length > 0 ? enemyPositions : new Vector4[1]);

            if (Time.time >= nextCacheUpdateTime)
            {
                clearedZoneCache.Clear();
                nextCacheUpdateTime = Time.time + cacheUpdateInterval;
            }

            if (_renderTexture != null)
            {
                computeShader.SetInt(_gridSizeID, GridSize);
                computeShader.SetFloat(_cellSizeID, CellSize);
                computeShader.SetFloat(_revealRadiusResourceStoragesID, RevealRadiusResourceStorages);
                computeShader.SetFloat(_revealRadiusVisionTowersID, RevealRadiusVisionTowers);
                computeShader.SetFloat(_revealRadiusFactoriesID, RevealRadiusFactories);
                computeShader.SetBuffer(_kernelHandle, _unitPositionsID, _unitPositionBuffer);
                computeShader.SetBuffer(_kernelHandle, _resourceStoragePositionsID, _resourceStoragePositionBuffer);
                computeShader.SetBuffer(_kernelHandle, _visionTowerPositionsID, _visionTowerPositionBuffer);
                computeShader.SetBuffer(_kernelHandle, _factoryPositionsID, _factoryPositionBuffer);
                computeShader.SetBuffer(_kernelHandle, _enemyPositionsID, _enemyPositionBuffer); 
                computeShader.SetVector(_clearColorID, Color.clear);
                computeShader.SetVector(_fullBlackColorID, new Color(0, 0, 0, 0.8f)); 

                computeShader.SetTexture(_kernelHandle, _resultID, _renderTexture);
                computeShader.Dispatch(_kernelHandle, GridSize / 8, GridSize / 8, 1);

                AsyncGPUReadback.Request(_renderTexture, 0, TextureFormat.RGBA32, OnCompleteReadback);
            }

            UpdateEnemyVisibility();
        }

        private void OnDestroy()
        {
            _fullBlack.Dispose();
            _unitPositionBuffer.Release();
            _resourceStoragePositionBuffer.Release();
            _visionTowerPositionBuffer.Release();
            _factoryPositionBuffer.Release();
            _enemyPositionBuffer.Release();
        }

        private void InitializeBuffers()
        {
            _unitPositionBuffer = new ComputeBuffer(1, 16);
            _resourceStoragePositionBuffer = new ComputeBuffer(1, 16);
            _visionTowerPositionBuffer = new ComputeBuffer(1, 16);
            _factoryPositionBuffer = new ComputeBuffer(1, 16);
            _enemyPositionBuffer = new ComputeBuffer(1, 16);
        }

        private void UpdateBuffers(int unitCount, int resourceStorageCount, int visionTowerCount, int factoryCount)
        {
            UpdateBuffer(ref _unitPositionBuffer, unitCount);
            UpdateBuffer(ref _resourceStoragePositionBuffer, resourceStorageCount);
            UpdateBuffer(ref _visionTowerPositionBuffer, visionTowerCount);
            UpdateBuffer(ref _factoryPositionBuffer, factoryCount);
        }

        private void UpdateBuffer(ref ComputeBuffer buffer, int count)
        {
            if (buffer.count != count)
            {
                buffer.Release();
                buffer = new ComputeBuffer(count > 0 ? count : 1, 16);
            }
        }

        private void InitializeShaderProperties()
        {
            _resultID = Shader.PropertyToID("Result");
            _unitPositionsID = Shader.PropertyToID("unitPositions");
            _resourceStoragePositionsID = Shader.PropertyToID("resourceStoragePositions");
            _visionTowerPositionsID = Shader.PropertyToID("visionTowerPositions");
            _factoryPositionsID = Shader.PropertyToID("factoryPositions");
            _enemyPositionsID = Shader.PropertyToID("enemyPositions");
            _gridSizeID = Shader.PropertyToID("gridSize");
            _cellSizeID = Shader.PropertyToID("cellSize");
            _revealRadiusResourceStoragesID = Shader.PropertyToID("revealRadiusResourceStorages");
            _revealRadiusVisionTowersID = Shader.PropertyToID("revealRadiusVisionTowers");
            _revealRadiusFactoriesID = Shader.PropertyToID("revealRadiusFactories");
            _clearColorID = Shader.PropertyToID("clearColor");
            _fullBlackColorID = Shader.PropertyToID("fullBlackColor");
        }

        private void FindAllObjects()
        {
            FindAllUnits();
            FindAllResourceStorages();
            FindAllVisionTowers();
            FindAllFactories();
            FindAllEnemies();
        }

        private void OnCompleteReadback(AsyncGPUReadbackRequest request)
        {
            if (request.hasError) return;

            if (_lowResTexture != null)
            {
                NativeArray<Color32> data = request.GetData<Color32>();

                _lowResTexture.SetPixelData(data, 0);
                _lowResTexture.Apply(updateMipmaps: false, makeNoLongerReadable: false);

                if (mr != null && mr.material != null)
                    mr.material.mainTexture = _lowResTexture;
            }
        }

        private void FindAllUnits()
        {
            _units = new List<Unit>(FindObjectsOfType<Ally>());
        }

        private void FindAllResourceStorages()
        {
            _resourceStorages = new List<Building>(FindObjectsOfType<Building>().Where(b => b.CompareTag("RessourceStorage")));
        }

        private void FindAllVisionTowers()
        {
            _visionTowers = new List<Building>(FindObjectsOfType<Building>().Where(b => b.CompareTag("VisionTower")));
        }

        private void FindAllFactories()
        {
            _factories = new List<Building>(FindObjectsOfType<Building>().Where(b => b.CompareTag("Factory")));
        }

        private void FindAllEnemies()
        {
            _enemies = new List<Unit>(FindObjectsOfType<Enemy>());
        }

        private void UpdateEnemyVisibility()
        {
            foreach (var enemy in _enemies)
            {
                var isVisible = IsPositionInClearedZone(enemy.transform.position);
                SetVisibility(enemy, isVisible);
            }
        }

        private void SetVisibility(Unit enemy, bool isVisible)
        {
            foreach (var renderer in enemy.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = isVisible;
            }

            // Hide health bar if present and check health
            var healthBar = enemy.GetComponentInChildren<HealthBar>();
            if (healthBar != null)
            {
                bool shouldShowHealthBar = isVisible && healthBar.GetCurrentHealth() < healthBar.GetMaxHealth();
                healthBar.SetVisibility(shouldShowHealthBar);
            }
        }



        public bool IsPositionInClearedZone(Vector3 position)
        {
            var x = Mathf.FloorToInt((position.x + GroundPlane.transform.localScale.x * 5.0f) / CellSize);
            var z = Mathf.FloorToInt((position.z + GroundPlane.transform.localScale.z * 5.0f) / CellSize);

            x = Mathf.Clamp(x, 0, GridSize - 1);
            z = Mathf.Clamp(z, 0, GridSize - 1);

            Vector2Int gridPos = new Vector2Int(x, z);

            if (clearedZoneCache.TryGetValue(gridPos, out bool isCleared))
            {
                return isCleared;
            }

            isCleared = _lowResTexture.GetPixel(x, z).a < 0.5f;
            clearedZoneCache[gridPos] = isCleared;

            return isCleared;
        }
    }
}
