using System;
using System.Collections.Generic;
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

        [Header("Building")]
        [SerializeField] private Material previewMaterial;
        [SerializeField] private Material previewInvalidMaterial;
        [SerializeField] private Material buildingMaterial;
        [Space(5)]
        [SerializeField] private MeshRenderer objectRenderer;


        [Space(10)] [Header("Grid")]
        [SerializeField] private Material gridMaterial;
        [SerializeField] private Material gridInvalidMaterial;
        [Space(5)]
        [SerializeField] private MeshRenderer gridRenderer;

        private float constructionTime;

        [Space(10)] [Header("Production")]
        [SerializeField] private EntityFactory entityFactory;
        private Queue<Entity> productionQueue;

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
            gridRenderer.materials = new[] { gridMaterial };
            objectRenderer.shadowCastingMode = ShadowCastingMode.Off;
            objectRenderer.receiveShadows = false;
        }

        internal void PreviewInvalid()
        {
            state = BuildingStates.Preview;
            objectRenderer.materials = new[] { previewInvalidMaterial };
            gridRenderer.materials = new[] { gridInvalidMaterial };
            objectRenderer.shadowCastingMode = ShadowCastingMode.Off;
            objectRenderer.receiveShadows = false;
        }

        internal void StartConstruction(float constructionTime)
        {
            state = BuildingStates.Constructing;
            this.constructionTime = constructionTime;
            gridRenderer.enabled = false;
        }

        internal void FinishConstruction()
        {
            state = BuildingStates.Constructed;
            objectRenderer.materials = new[] { buildingMaterial };
            objectRenderer.shadowCastingMode = ShadowCastingMode.On;
            objectRenderer.receiveShadows = true;
        }
    }
}