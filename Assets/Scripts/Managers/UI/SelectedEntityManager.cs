using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectedEntityManager : MonoBehaviour
{
    private readonly List<BaseObject> selectedProfiles = new();

    public static SelectedEntityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void UpdateSelectedEntities(List<BaseObject> newSelectedProfiles)
    {
        selectedProfiles.Clear();
        selectedProfiles.AddRange(newSelectedProfiles);

        UpdateSelectedEntitiesList();
        var selectedProfile = selectedProfiles.Count > 0 ? selectedProfiles[0] : null;
        UpdateFirstSelectedEntity(selectedProfile); // Display the face of the first entity selected 
        StatisticsUpdater.Instance
            .UpdateStatisticsContainer(selectedProfile); // Display the stats of the first entity selected 

        // Update visibility of #Character and #Selected panels
        bool hasSelectedEntities = selectedProfiles.Count > 0;
        UIManager.Instance.characterPanel.style.display = hasSelectedEntities ? DisplayStyle.Flex : DisplayStyle.None;
        UIManager.Instance.selectedPanel.style.display = hasSelectedEntities ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void UpdateFirstSelectedEntity(BaseObject profile)
    {
        if (profile == null || GetProfileImage(profile) == null) return;

        if (profile is BaseObject entityProfile)
        {
            var backgroundImage = GetProfileImage(profile);
            if (backgroundImage != null)
                UIManager.Instance.faceContainer.style.backgroundImage = new StyleBackground(backgroundImage);
            else
                UIManager.Instance.faceContainer.style.backgroundImage = null;
        }
    }

    private void UpdateSelectedEntitiesList()
    {
        UIManager.Instance.selectedEntitiesList.Clear();

        for (int i = 0; i < selectedProfiles.Count; i++)
        {
            var profile = selectedProfiles[i];
            var image = new Image { image = GetProfileImage(profile) };
            image.AddToClassList("selectedEntities");

            // Add the class only to the first element
            if (i == 0) image.AddToClassList("selectedEntity");

            UIManager.Instance.selectedEntitiesList.Add(image);
        }
    }

    private Texture2D GetProfileImage(BaseObject profile)
    {
        if (profile is BaseObject objectProfile) return objectProfile.Data.image;
        return null;
    }
}