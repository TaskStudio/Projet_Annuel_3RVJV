using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Construction
{
    [Serializable]
    public class Building : MonoBehaviour
    {
        public enum BuildingStates
        {
            Preview,
            Constructing,
            Constructed
        }

        [SerializeField] private Material previewMaterial, previewInvalidMaterial, material;
        [SerializeField] private MeshRenderer objectRenderer;
        private float constructionTime;

        public BuildingStates state { get; internal set; }

        private void Update()
        {
            if (state == BuildingStates.Constructing)
            {
                constructionTime -= Time.deltaTime;
                if (constructionTime <= 0)
                    FinishConstruction();
            }
        }

        internal void PreviewValid()
        {
            state = BuildingStates.Preview;
            objectRenderer.materials = new[] { previewMaterial };
            objectRenderer.shadowCastingMode = ShadowCastingMode.Off;
            objectRenderer.receiveShadows = false;
        }

        internal void PreviewInvalid()
        {
            state = BuildingStates.Preview;
            objectRenderer.materials = new[] { previewInvalidMaterial };
            objectRenderer.shadowCastingMode = ShadowCastingMode.Off;
            objectRenderer.receiveShadows = false;
        }

        internal void StartConstruction(float constructionTime)
        {
            state = BuildingStates.Constructing;
            this.constructionTime = constructionTime;
        }

        internal void FinishConstruction()
        {
            state = BuildingStates.Constructed;
            objectRenderer.materials = new[] { material };
            objectRenderer.shadowCastingMode = ShadowCastingMode.On;
            objectRenderer.receiveShadows = true;
        }
    }
}