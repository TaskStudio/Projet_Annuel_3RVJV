using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    [SerializeField] private UIDocument uiDocument;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        LogPlayerName();
    }

    public async void LogPlayerName()
    {
        try
        {
            var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            Debug.Log($"Player Name: {playerName}");

            // Update the UI label with the player name
            var root = uiDocument.rootVisualElement;
            var connexionNameLabel = root.Q<Label>("ConnexionName");
            if (connexionNameLabel != null)
            {
                connexionNameLabel.text = $"Connected as {playerName}";
            }
            else
            {
                Debug.LogError("ConnexionName label not found in the UXML.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching player name: {ex.Message}");
        }
    }
}