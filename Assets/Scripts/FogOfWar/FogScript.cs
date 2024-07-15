using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace FogOfWar // Corrected namespace
{
    public class FogScript : MonoBehaviour
    {
        public bool keepPrevious;
        public MeshRenderer mr;
        public ComputeShader computeShader;

        private List<Unit> _units;

        private NativeArray<Color> _fullBlack;
        private Texture2D _lowResTexture;
        private RenderTexture _renderTexture;
        private ComputeBuffer _unitPositionBuffer;

        private static readonly int GridSize = 512;
        private static readonly float CellSize = 80.0f / GridSize;
        private static readonly float RevealRadius = 2.5f;

        private int _kernelHandle;
        private int _resultID;
        private int _unitPositionsID;
        private int _gridSizeID;
        private int _cellSizeID;
        private int _revealRadiusID;
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

            // Cache property IDs
            _resultID = Shader.PropertyToID("Result");
            _unitPositionsID = Shader.PropertyToID("unitPositions");
            _gridSizeID = Shader.PropertyToID("gridSize");
            _cellSizeID = Shader.PropertyToID("cellSize");
            _revealRadiusID = Shader.PropertyToID("revealRadius");
            _clearColorID = Shader.PropertyToID("clearColor");
            _fullBlackColorID = Shader.PropertyToID("fullBlackColor");

            // Find all units in the scene
            FindAllUnits();
        }

        private void Update()
        {
            if (!keepPrevious)
            {
                Graphics.Blit(Texture2D.blackTexture, _renderTexture);
            }

            // Update the list of units every frame
            FindAllUnits();

            // Create and fill the compute buffer with unit positions
            Vector4[] unitPositions = new Vector4[_units.Count];
            for (int i = 0; i < _units.Count; i++)
            {
                Vector3 pos = _units[i].transform.position;
                unitPositions[i] = new Vector4(pos.x, pos.z, 0.0f, 0.0f); // x and z coordinates, y is set to 0
            }
            _unitPositionBuffer = new ComputeBuffer(_units.Count, 16);
            _unitPositionBuffer.SetData(unitPositions);

            computeShader.SetInt(_gridSizeID, GridSize);
            computeShader.SetFloat(_cellSizeID, CellSize);
            computeShader.SetFloat(_revealRadiusID, RevealRadius);
            computeShader.SetBuffer(_kernelHandle, _unitPositionsID, _unitPositionBuffer);
            computeShader.SetVector(_clearColorID, Color.clear);
            computeShader.SetVector(_fullBlackColorID, Color.black);

            computeShader.SetTexture(_kernelHandle, _resultID, _renderTexture);
            computeShader.Dispatch(_kernelHandle, GridSize / 8, GridSize / 8, 1);

            // Release the compute buffer
            _unitPositionBuffer.Release();

            // Asynchronously read back the texture if needed
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
        }

        private void FindAllUnits()
        {
            _units = new List<Unit>(FindObjectsOfType<Unit>());
        }
    }
}
