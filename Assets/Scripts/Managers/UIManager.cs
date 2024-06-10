using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public VisualElement selectedEntitiesContainer;
    private List<EntityProfile> selectedEntities = new List<EntityProfile>();
    private ListView listView;

    void Start()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        selectedEntitiesContainer = rootVisualElement.Q<VisualElement>("SelectedEntitiesContainer");

        // Configure the ListView
        listView = new ListView();
        listView.itemsSource = selectedEntities;
        listView.makeItem = () => new VisualElement(); // Customize this part to create your item
        listView.bindItem = (element, i) =>
        {
            var entityProfile = selectedEntities[i];
            element.Clear();
            var image = new Image();
            image.image = entityProfile.Image;
            element.Add(image);
        };
        listView.style.flexGrow = 1.0f;

        selectedEntitiesContainer.Add(listView);
    }

    public void UpdateSelectedEntities(List<EntityProfile> newSelectedEntities)
    {
        selectedEntities.Clear();
        selectedEntities.AddRange(newSelectedEntities);
        listView.RefreshItems(); // Refresh the ListView to update the UI
    }
}