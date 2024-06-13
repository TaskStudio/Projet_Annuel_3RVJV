using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public VisualElement selectedEntitiesList;
    public VisualElement statisticsScrollView;
    public VisualElement faceContainer;

    private List<IProfile> selectedProfiles = new List<IProfile>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        if (rootVisualElement == null)
        {
            Debug.LogError("Root Visual Element is null. Ensure UIDocument component is set up correctly.");
            return;
        }

        selectedEntitiesList = rootVisualElement.Q<VisualElement>("SelectedEntitiesList");
        statisticsScrollView = rootVisualElement.Q<VisualElement>("StatisticsScrollView");
        faceContainer = rootVisualElement.Q<VisualElement>("FaceContainer");
        
        if (selectedEntitiesList == null || statisticsScrollView == null || faceContainer == null)
        {
            Debug.LogError("Containers are not found in the UXML. Check the UXML and the names.");
            return;
        }
    }

    private Texture2D GetProfileImage(IProfile profile)
    {
        if (profile is EntityProfile entityProfile)
        {
            return entityProfile.Image;
        }
        return null;
    }

    public void UpdateSelectedEntities(List<IProfile> newSelectedProfiles)
    {
        selectedProfiles.Clear();
        selectedProfiles.AddRange(newSelectedProfiles);

        UpdateSelectedEntitiesList();
        var selectedProfile = selectedProfiles.Count > 0 ? selectedProfiles[0] : null;
        UpdateFirstSelectedEntity(selectedProfile);// Display the face of the first entity selected 
        UpdateStatisticsContainer(selectedProfile); // Display the stats of the first entity selected 
    }
    
    private void UpdateFirstSelectedEntity(IProfile profile)
    {
        faceContainer.Clear();
        if (profile == null) return;
        
        if (profile is EntityProfile entityProfile)
        {
            var image = new Image { image = GetProfileImage(profile) };
            faceContainer.Add(image);
        }
    }

    private void UpdateSelectedEntitiesList()
    {
        selectedEntitiesList.Clear();

        for (int i = 0; i < selectedProfiles.Count; i++)
        {
            var profile = selectedProfiles[i];
            var image = new Image { image = GetProfileImage(profile) };
            image.AddToClassList("selectedEntities");

            // Add the class only to the first element
            if (i == 0)
            {
                image.AddToClassList("selectedEntity");
            }

            selectedEntitiesList.Add(image);
        }
    }

    private void UpdateStatisticsContainer(IProfile profile)
    {
        statisticsScrollView.Clear();
        if (profile == null) return;

        var nameLabel = new Label { text = profile.Name };
        var descriptionLabel = new Label { text = profile.Description };

        statisticsScrollView.Add(nameLabel);
        statisticsScrollView.Add(descriptionLabel);

        if (profile is EntityProfile entityProfile)
        {
            var hpLabel = new Label { text = "HP: " + entityProfile.Health };
            var manaLabel = new Label { text = "Mana: " + entityProfile.Mana };
            var physicalResLabel = new Label { text = "Physical Resistance: " + entityProfile.PhysicalResistance };
            var magicalResLabel = new Label { text = "Magical Resistance: " + entityProfile.MagicalResistance };
            var attackSpeedLabel = new Label { text = "Attack Speed: " + entityProfile.AttackSpeed };
            var movementSpeedLabel = new Label { text = "Movement Speed: " + entityProfile.MovementSpeed };

            statisticsScrollView.Add(hpLabel);
            statisticsScrollView.Add(manaLabel);
            statisticsScrollView.Add(physicalResLabel);
            statisticsScrollView.Add(magicalResLabel);
            statisticsScrollView.Add(attackSpeedLabel);
            statisticsScrollView.Add(movementSpeedLabel);
        }
    }
}