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
            foreach (BuildingData buildingData in buildingDatabase.buildingsData)
            {
                GameObject button = Instantiate(buttonPrefab, transform);
                button.name = $"{buildingData.DisplayName}Button";
                var buildingButton = button.GetComponent<BuildingButton>();
                buildingButton.buildingID = buildingData.IdNumber;
                buildingButton.buildingName = buildingData.DisplayName;
                buildingButton.placementSystem = placementSystem;
            }
        }

        private void OnDisable()
        {
            ClearChildren();
        }

        private void ClearChildren()
        {
            while (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}