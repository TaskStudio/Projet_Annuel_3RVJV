using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Construction
{
    [ExecuteInEditMode]
    public class BuildingPanel : MonoBehaviour
    {
        [SerializeField] private BuildingDatabaseSO buildingDatabase;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private PlacementSystem placementSystem;

        private void OnEnable()
        {
            ClearChildren();
            foreach (var buildingData in buildingDatabase.buildingsData)
            {
                var button = Instantiate(buttonPrefab, transform);
                button.name = $"{buildingData.Name}Button";
                var buildingButton = button.GetComponent<BuildingButton>();
                buildingButton.buildingID = buildingData.ID;
                buildingButton.buildingName = buildingData.Name;
                buildingButton.placementSystem = placementSystem;
            }
        }

        private void OnDisable()
        {
            ClearChildren();
        }

        private void ClearChildren()
        {
            while (transform.childCount > 0) 
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }
}
