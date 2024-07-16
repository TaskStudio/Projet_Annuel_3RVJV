using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace FogOfWar
{
    public class FogScript : MonoBehaviour
    {
        public bool keepPrevious;
        public MeshRenderer mr;
        public ComputeShader computeShader;

        private List<Unit> _units;
        private List<Building> _buildings;

        private NativeArray<Color> _fullBlack;
        private Texture2D _lowResTexture;
        private RenderTexture _renderTexture;
        private ComputeBuffer _unitPositionBuffer;
        private ComputeBuffer _buildingPositionBuffer;

        private static readonly int GridSize = 512;
        private static readonly float CellSize = 80.0f / GridSize;
        private static readonly float RevealRadius = 2.5f;
        private static readonly float RevealRadiusBuildings = 10.0f;

        private int _kernelHandle;
        private int _resultID;
        private int _unitPositionsID;
        private int _buildingPositionsID;
        private int _gridSizeID;
        private int _cellSizeID;
        private int _revealRadiusID;
        private int _revealRadiusBuildingsID;
        private int _clearColorID;
        private int _fullBlackColorID;

        private void Start()
        {
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

            _resultID = Shader.PropertyToID("Result");
            _unitPositionsID = Shader.PropertyToID("unitPositions");
            _buildingPositionsID = Shader.PropertyToID("buildingPositions");
            _gridSizeID = Shader.PropertyToID("gridSize");
            _cellSizeID = Shader.PropertyToID("cellSize");
            _revealRadiusID = Shader.PropertyToID("revealRadius");
            _revealRadiusBuildingsID = Shader.PropertyToID("revealRadiusBuildings");
            _clearColorID = Shader.PropertyToID("clearColor");
            _fullBlackColorID = Shader.PropertyToID("fullBlackColor");

            _units = new List<Unit>();
            _buildings = new List<Building>();

            FindAllUnits();
            FindAllBuildings();

            _unitPositionBuffer = new ComputeBuffer(1, 16);
            _unitPositionBuffer.SetData(new Vector4[1]);
            _buildingPositionBuffer = new ComputeBuffer(1, 16);
            _buildingPositionBuffer.SetData(new Vector4[1]);
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

            computeShader.SetInt(_gridSizeID, GridSize);
            computeShader.SetFloat(_cellSizeID, CellSize);
            computeShader.SetFloat(_revealRadiusID, RevealRadius);
            computeShader.SetFloat(_revealRadiusBuildingsID, RevealRadiusBuildings);
            computeShader.SetBuffer(_kernelHandle, _unitPositionsID, _unitPositionBuffer);
            computeShader.SetBuffer(_kernelHandle, _buildingPositionsID, _buildingPositionBuffer);
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
            }

            private void FindAllUnits()
            {
                _units = new List<Unit>(FindObjectsOfType<Unit>());
            }

            private void FindAllBuildings()
            {
                _buildings = new List<Building>(FindObjectsOfType<Building>());
                Debug.Log($"Found {_buildings.Count} building(s)");
            }
        }
    }
