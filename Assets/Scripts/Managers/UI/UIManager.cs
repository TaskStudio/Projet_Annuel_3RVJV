using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }  
    public VisualElement selectedEntitiesContainer;
    private List<IProfile> selectedEntities = new List<IProfile>();
    private ListView listView;

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

        selectedEntitiesContainer = rootVisualElement.Q<VisualElement>("SelectedEntitiesContainer");
        if (selectedEntitiesContainer == null)
        {
            Debug.LogError("SelectedEntitiesContainer is not found in the UXML. Check the UXML and the name.");
            return;
        }

        listView = new ListView();
        listView.itemsSource = selectedEntities;
        listView.makeItem = () => new VisualElement();
        listView.bindItem = (element, i) =>
        {
            var profile = selectedEntities[i];
            element.Clear();
            var nameLabel = new Label { text = profile.Name };
            var descriptionLabel = new Label { text = profile.Description };
            element.Add(nameLabel);
            element.Add(descriptionLabel);

            if (profile is EntityProfile entityProfile)
            {
                var image = new Image { image = entityProfile.Image };
                var hpLabel = new Label { text = "HP: " + entityProfile.HealthPoints };
                var manaLabel = new Label { text = "Mana: " + entityProfile.Mana };
                var physicalResLabel = new Label { text = "Physical Resistance: " + entityProfile.PhysicalResistance };
                var magicalResLabel = new Label { text = "Magical Resistance: " + entityProfile.MagicalResistance };
                var attackSpeedLabel = new Label { text = "Attack Speed: " + entityProfile.AttackSpeed };
                var movementSpeedLabel = new Label { text = "Movement Speed: " + entityProfile.MovementSpeed };
                element.Add(image);
                element.Add(hpLabel);
                element.Add(manaLabel);
                element.Add(physicalResLabel);
                element.Add(magicalResLabel);
                element.Add(attackSpeedLabel);
                element.Add(movementSpeedLabel);    
            }
            else if (profile is BuildingProfile buildingProfile)
            {
                var producesLabel = new Label { text = "Produces: " + string.Join(", ", buildingProfile.ProducesEntities.Select(e => e.Name)) };
                element.Add(producesLabel);
            }
            else if (profile is ResourceProfile resourceProfile)
            {
                var resourceTypeLabel = new Label { text = "Resource Type: " + resourceProfile.ResourceType };
                element.Add(resourceTypeLabel);
            }
        };
        listView.style.flexGrow = 1.0f;

        selectedEntitiesContainer.Add(listView);
    }

    public void UpdateSelectedEntities(List<IProfile> newSelectedEntities)
    {
        selectedEntities.Clear();
        selectedEntities.AddRange(newSelectedEntities);
        listView.RefreshItems();
    }
}
