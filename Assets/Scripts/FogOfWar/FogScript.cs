using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace FogOfWar
{
    public class FogScript : MonoBehaviour
    {
        private static readonly int GridSize = 512;
        private static readonly float CellSize = 80.0f / GridSize;
        private static readonly float RevealRadius = 2.5f;
        private static readonly float RevealRadiusResourceStorages = 5.0f;
        private static readonly float RevealRadiusVisionTowers = 15.0f;
        private static readonly float RevealRadiusFactories = 10.0f;
        public bool keepPrevious;
        public MeshRenderer mr;
        public ComputeShader computeShader;
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
        private int _revealRadiusID;
        private int _revealRadiusResourceStoragesID;
        private int _revealRadiusVisionTowersID;
        private ComputeBuffer _unitPositionBuffer;
        private int _unitPositionsID;

        private List<Unit> _units;
        private ComputeBuffer _visionTowerPositionBuffer;
        private int _visionTowerPositionsID;
        private List<Building> _visionTowers;

        private void Start()
        {
            InitializeBuffers();

            _fullBlack = new NativeArray<Color>(GridSize * GridSize, Allocator.Persistent);
            for (var i = 0; i < GridSize; i++)
            for (var j = 0; j < GridSize; j++)
                _fullBlack[GridSize * i + j] = Color.black;

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
        }

        private void Update()
        {
            if (_kernelHandle < 0) return;

            if (!keepPrevious && _renderTexture != null) Graphics.Blit(Texture2D.blackTexture, _renderTexture);

            FindAllUnits();
            FindAllResourceStorages();
            FindAllVisionTowers();
            FindAllFactories();

            var unitPositions = new Vector4[_units.Count];
            for (var i = 0; i < _units.Count; i++)
            {
                var pos = _units[i].transform.position;
                unitPositions[i] = new Vector4(pos.x, pos.z, 0.0f, 0.0f);
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

            if (_renderTexture != null)
            {
                computeShader.SetInt(_gridSizeID, GridSize);
                computeShader.SetFloat(_cellSizeID, CellSize);
                computeShader.SetFloat(_revealRadiusID, RevealRadius);
                computeShader.SetFloat(_revealRadiusResourceStoragesID, RevealRadiusResourceStorages);
                computeShader.SetFloat(_revealRadiusVisionTowersID, RevealRadiusVisionTowers);
                computeShader.SetFloat(_revealRadiusFactoriesID, RevealRadiusFactories);
                computeShader.SetBuffer(_kernelHandle, _unitPositionsID, _unitPositionBuffer);
                computeShader.SetBuffer(_kernelHandle, _resourceStoragePositionsID, _resourceStoragePositionBuffer);
                computeShader.SetBuffer(_kernelHandle, _visionTowerPositionsID, _visionTowerPositionBuffer);
                computeShader.SetBuffer(_kernelHandle, _factoryPositionsID, _factoryPositionBuffer);
                computeShader.SetVector(_clearColorID, Color.clear);
                computeShader.SetVector(_fullBlackColorID, Color.black);

                computeShader.SetTexture(_kernelHandle, _resultID, _renderTexture);
                computeShader.Dispatch(_kernelHandle, GridSize / 8, GridSize / 8, 1);

                AsyncGPUReadback.Request(_renderTexture, 0, TextureFormat.RGBA32, OnCompleteReadback);
            }
        }

        private void OnDestroy()
        {
            _fullBlack.Dispose();
            _unitPositionBuffer.Release();
            _resourceStoragePositionBuffer.Release();
            _visionTowerPositionBuffer.Release();
            _factoryPositionBuffer.Release();
        }

        private void InitializeBuffers()
        {
            _unitPositionBuffer = new ComputeBuffer(1, 16);
            _resourceStoragePositionBuffer = new ComputeBuffer(1, 16);
            _visionTowerPositionBuffer = new ComputeBuffer(1, 16);
            _factoryPositionBuffer = new ComputeBuffer(1, 16);
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
            _gridSizeID = Shader.PropertyToID("gridSize");
            _cellSizeID = Shader.PropertyToID("cellSize");
            _revealRadiusID = Shader.PropertyToID("revealRadius");
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
        }

        private void OnCompleteReadback(AsyncGPUReadbackRequest request)
        {
            if (request.hasError) return;

            if (_lowResTexture != null)
            {
                var data = request.GetData<Color32>();
                _lowResTexture.SetPixels32(data.ToArray());
                _lowResTexture.Apply();

                if (mr != null && mr.material != null) mr.material.mainTexture = _lowResTexture;
            }
        }

        private void FindAllUnits()
        {
            _units = new List<Unit>(FindObjectsOfType<Ally>());
        }

        private void FindAllResourceStorages()
        {
            _resourceStorages =
                new List<Building>(FindObjectsOfType<Building>().Where(b => b.CompareTag("RessourceStorage")));
        }

        private void FindAllVisionTowers()
        {
            _visionTowers = new List<Building>(FindObjectsOfType<Building>().Where(b => b.CompareTag("VisionTower")));
        }

        private void FindAllFactories()
        {
            _factories = new List<Building>(FindObjectsOfType<Building>().Where(b => b.CompareTag("Factory")));
        }
    }
}