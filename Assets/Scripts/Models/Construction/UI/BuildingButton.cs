using TMPro;
using UnityEngine;

public class BuildingButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;

    public PlacementSystem placementSystem { get; internal set; }
    public int buildingID { get; internal set; }
    public string buildingName { get; internal set; }

    public void Start()
    {
        buttonText.text = buildingName;
    }

    public void ClickHandler()
    {
        placementSystem.StartPlacement(buildingID);
    }
}