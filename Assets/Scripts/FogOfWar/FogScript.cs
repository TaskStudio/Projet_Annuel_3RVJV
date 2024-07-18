using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

namespace FogOfWar
{
    public class FogScript : MonoBehaviour
    {
        public bool keepPrevious;
        public MeshRenderer mr;
        public ComputeShader computeShader;

        private List<Unit> _units;
        private List<Building> _buildings;
        private List<Building> _visionTowers;
        private List<Building> _factories;

        private NativeArray<Color> _fullBlack;
        private Texture2D _lowResTexture;
        private RenderTexture _renderTexture;
        private ComputeBuffer _unitPositionBuffer;
        private ComputeBuffer _buildingPositionBuffer;
        private ComputeBuffer _visionTowerPositionBuffer;
        private ComputeBuffer _factoryPositionBuffer;

        private static readonly int GridSize = 512;
        private static readonly float CellSize = 80.0f / GridSize;
        private static readonly float RevealRadius = 2.5f;
        private static readonly float RevealRadiusBuildings = 10.0f;
        private static readonly float RevealRadiusVisionTowers = 15.0f;
        private static readonly float RevealRadiusFactories = 5.0f;

        private int _kernelHandle;
        private int _resultID;
        private int _unitPositionsID;
        private int _buildingPositionsID;
        private int _visionTowerPositionsID;
        private int _factoryPositionsID;
        private int _gridSizeID;
        private int _cellSizeID;
        private int _revealRadiusID;
        private int _revealRadiusBuildingsID;
        private int _revealRadiusVisionTowersID;
        private int _revealRadiusFactoriesID;
        private int _clearColorID;
        private int _fullBlackColorID;

        private void Start()
        {
            InitializeBuffers();

            _fullBlack = new NativeArray<Color>(GridSize * GridSize, Allocator.Persistent);
            for (var i = 0; i < GridSize; i++)
            {
                for (var j = 0; j < GridSize; j++)
                {
                    _fullBlack[GridSize * i + j] = Color.black;
                }
            }

            _lowResTexture = new Texture2D(GridSize, GridSize);
            _renderTexture = new RenderTexture(GridSize, GridSize, 0)
            {
                enableRandomWrite = true
            };
            _renderTexture.Create();

            _kernelHandle = computeShader.FindKernel("CSMain");
            if (_kernelHandle < 0)
            {
                Debug.LogError("Failed to find kernel 'CSMain'");
                return;
            }

            InitializeShaderProperties();
            FindAllObjects();
        }

        private void InitializeBuffers()
        {
            _unitPositionBuffer = new ComputeBuffer(1, 16);
            _buildingPositionBuffer = new ComputeBuffer(1, 16);
            _visionTowerPositionBuffer = new ComputeBuffer(1, 16);
            _factoryPositionBuffer = new ComputeBuffer(1, 16);
        }

        private void UpdateBuffers(int unitCount, int buildingCount, int visionTowerCount, int factoryCount)
        {
            UpdateBuffer(ref _unitPositionBuffer, unitCount);
            UpdateBuffer(ref _buildingPositionBuffer, buildingCount);
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
            _buildingPositionsID = Shader.PropertyToID("buildingPositions");
            _visionTowerPositionsID = Shader.PropertyToID("visionTowerPositions");
            _factoryPositionsID = Shader.PropertyToID("factoryPositions");
            _gridSizeID = Shader.PropertyToID("gridSize");
            _cellSizeID = Shader.PropertyToID("cellSize");
            _revealRadiusID = Shader.PropertyToID("revealRadius");
            _revealRadiusBuildingsID = Shader.PropertyToID("revealRadiusBuildings");
            _revealRadiusVisionTowersID = Shader.PropertyToID("revealRadiusVisionTowers");
            _revealRadiusFactoriesID = Shader.PropertyToID("revealRadiusFactories");
            _clearColorID = Shader.PropertyToID("clearColor");
            _fullBlackColorID = Shader.PropertyToID("fullBlackColor");
        }

        private void FindAllObjects()
        {
            FindAllUnits();
            FindAllBuildings();
            FindAllVisionTowers();
            FindAllFactories();
        }

        private void Update()
        {
            if (_kernelHandle < 0)
            {
                Debug.LogError("Invalid kernel handle");
                return;
            }

            if (!keepPrevious)
            {
                Graphics.Blit(Texture2D.blackTexture, _renderTexture);
            }

            FindAllUnits();
            FindAllBuildings();
            FindAllVisionTowers();
            FindAllFactories();

            Vector4[] unitPositions = new Vector4[_units.Count];
            for (int i = 0; i < _units.Count; i++)
            {
                Vector3 pos = _units[i].transform.position;
                unitPositions[i] = new Vector4(pos.x, pos.z, 0.0f, 0.0f);
            }
            _unitPositionBuffer.Release();
            _unitPositionBuffer = new ComputeBuffer(_units.Count > 0 ? _units.Count : 1, 16);
            _unitPositionBuffer.SetData(unitPositions.Length > 0 ? unitPositions : new Vector4[1]);

            Vector4[] buildingPositions = new Vector4[_buildings.Count];
            for (int i = 0; i < _buildings.Count; i++)
            {
                Vector3 pos = _buildings[i].transform.position;
                buildingPositions[i] = new Vector4(pos.x, pos.z, 0.0f, 0.0f);
            }
            if (buildingPositions.Length == 0)
            {
                buildingPositions = new Vector4[] { new Vector4(float.MaxValue, float.MaxValue, 0, 0) };
            }
            _buildingPositionBuffer.Release();
            _buildingPositionBuffer = new ComputeBuffer(_buildings.Count > 0 ? _buildings.Count : 1, 16);
            _buildingPositionBuffer.SetData(buildingPositions.Length > 0 ? buildingPositions : new Vector4[1]);

            Vector4[] visionTowerPositions = new Vector4[_visionTowers.Count];
            for (int i = 0; i < _visionTowers.Count; i++)
            {
                Vector3 pos = _visionTowers[i].transform.position;
                visionTowerPositions[i] = new Vector4(pos.x, pos.z, 0.0f, 0.0f);
            }
            if (visionTowerPositions.Length == 0)
            {
                visionTowerPositions = new Vector4[] { new Vector4(float.MaxValue, float.MaxValue, 0, 0) };
            }
            _visionTowerPositionBuffer.Release();
            _visionTowerPositionBuffer = new ComputeBuffer(_visionTowers.Count > 0 ? _visionTowers.Count : 1, 16);
            _visionTowerPositionBuffer.SetData(visionTowerPositions.Length > 0 ? visionTowerPositions : new Vector4[1]);

            Vector4[] factoryPositions = new Vector4[_factories.Count];
            for (int i = 0; i < _factories.Count; i++)
            {
                Vector3 pos = _factories[i].transform.position;
                factoryPositions[i] = new Vector4(pos.x, pos.z, 0.0f, 0.0f);
            }
            if (factoryPositions.Length == 0)
            {
                factoryPositions = new Vector4[] { new Vector4(float.MaxValue, float.MaxValue, 0, 0) };
            }
            _factoryPositionBuffer.Release();
            _factoryPositionBuffer = new ComputeBuffer(_factories.Count > 0 ? _factories.Count : 1, 16);
            _factoryPositionBuffer.SetData(factoryPositions.Length > 0 ? factoryPositions : new Vector4[1]);

            computeShader.SetInt(_gridSizeID, GridSize);
            computeShader.SetFloat(_cellSizeID, CellSize);
            computeShader.SetFloat(_revealRadiusID, RevealRadius);
            computeShader.SetFloat(_revealRadiusBuildingsID, RevealRadiusBuildings);
            computeShader.SetFloat(_revealRadiusVisionTowersID, RevealRadiusVisionTowers);
            computeShader.SetFloat(_revealRadiusFactoriesID, RevealRadiusFactories);
            computeShader.SetBuffer(_kernelHandle, _unitPositionsID, _unitPositionBuffer);
            computeShader.SetBuffer(_kernelHandle, _buildingPositionsID, _buildingPositionBuffer);
            computeShader.SetBuffer(_kernelHandle, _visionTowerPositionsID, _visionTowerPositionBuffer);
            computeShader.SetBuffer(_kernelHandle, _factoryPositionsID, _factoryPositionBuffer);
            computeShader.SetVector(_clearColorID, Color.clear);
            computeShader.SetVector(_fullBlackColorID, Color.black);

            computeShader.SetTexture(_kernelHandle, _resultID, _renderTexture);
            computeShader.Dispatch(_kernelHandle, GridSize / 8, GridSize / 8, 1);

            AsyncGPUReadback.Request(_renderTexture, 0, TextureFormat.RGBA32, OnCompleteReadback);
        }

        private void OnCompleteReadback(AsyncGPUReadbackRequest request)
        {
            if (request.hasError)
            {
                Debug.LogError("Error reading back texture from GPU");
                return;
            }

            NativeArray<Color32> data = request.GetData<Color32>();
            _lowResTexture.SetPixels32(data.ToArray());
            _lowResTexture.Apply();

            mr.material.mainTexture = _lowResTexture;
        }

        private void OnDestroy()
        {
            _fullBlack.Dispose();
            _unitPositionBuffer.Release();
            _buildingPositionBuffer.Release();
            _visionTowerPositionBuffer.Release();
            _factoryPositionBuffer.Release();
        }

        private void FindAllUnits()
        {
            _units = new List<Unit>(FindObjectsOfType<Ally>());
        }

        private void FindAllBuildings()
        {
            _buildings = new List<Building>(FindObjectsOfType<Building>());
            Debug.Log($"Found {_buildings.Count} building(s)");
        }

        private void FindAllVisionTowers()
        {
            _visionTowers = new List<Building>(FindObjectsOfType<Building>().Where(b => b.CompareTag("VisionTower")));
            Debug.Log($"Found {_visionTowers.Count} VisionTower(s)");
        }

        private void FindAllFactories()
        {
            _factories = new List<Building>(FindObjectsOfType<Building>().Where(b => b.CompareTag("Factory")));
            Debug.Log($"Found {_factories.Count} Factory(ies)");
        }
    }
}
