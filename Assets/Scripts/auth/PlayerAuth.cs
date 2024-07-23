using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GetPlayerScoreOptions = Unity.Services.Leaderboards.GetPlayerScoreOptions;

namespace Unity.Services.Authentication.PlayerAccounts.Samples
{
    class PlayerAccountsDemo : MonoBehaviour
    {
        [SerializeField] Text m_StatusText;
        [SerializeField] Text m_ExceptionText;
        [SerializeField] GameObject m_SignOut;
        [SerializeField] Toggle m_PlayerAccountSignOut;

        string m_ExternalIds;
        bool isSigningIn = false; // Flag to track if a sign-in is in progress

        async void Awake()
        {
            await UnityServices.InitializeAsync();
            PlayerAccountService.Instance.SignedIn += SignInWithUnity;

            // Check if already signed in
            if (PlayerAccountService.Instance.IsSignedIn)
            {
                SignInWithUnity();
            }
        }

        public async void StartSignInAsync()
        {
            if (PlayerAccountService.Instance.IsSignedIn || isSigningIn)
            {
                SignInWithUnity();
                return;
            }

            try
            {
                isSigningIn = true; // Set the flag to indicate sign-in is in progress
                await PlayerAccountService.Instance.StartSignInAsync();
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                SetException(ex);
            }
            finally
            {
                isSigningIn = false; // Reset the flag
            }
        }

        public void SignOut()
        {
            AuthenticationService.Instance.SignOut();

            if (m_PlayerAccountSignOut.isOn)
            {
                PlayerAccountService.Instance.SignOut();
            }

            UpdateUI();
        }

        public void OpenAccountPortal()
        {
            Application.OpenURL(PlayerAccountService.Instance.AccountPortalUrl);
        }

        async void SignInWithUnity()
        {
            try
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance
                        .AccessToken);
                }

                m_ExternalIds = GetExternalIds(AuthenticationService.Instance.PlayerInfo);
                UpdateUI();
                LogPlayerName();
                var metadata = new Dictionary<string, string>() { { "team", "red" } };
                var playerEntry = await LeaderboardsService.Instance
                    .AddPlayerScoreAsync(
                        "Leaderboard",
                        178,
                        new AddPlayerScoreOptions
                            { Metadata = new Dictionary<string, string>() { { "team", "red" } } });
                Debug.Log(JsonConvert.SerializeObject(playerEntry));

                GetPlayerScore();
                Debug.Log(GetPlayerScore());
                RedirectToScene("MainScene");
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                SetException(ex);
            }
            catch (InvalidOperationException ex)
            {
                Debug.LogWarning("Player is already signing in.");
                SetException(ex);
            }
        }

        void RedirectToScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        void UpdateUI()
        {
            var statusBuilder = new StringBuilder();

            statusBuilder.AppendLine(
                $"Player Accounts State: <b>{(PlayerAccountService.Instance.IsSignedIn ? "Signed in" : "Signed out")}</b>");
            statusBuilder.AppendLine(
                $"Player Accounts Access token: <b>{(string.IsNullOrEmpty(PlayerAccountService.Instance.AccessToken) ? "Missing" : "Exists")}</b>\n");
            statusBuilder.AppendLine(
                $"Authentication Service State: <b>{(AuthenticationService.Instance.IsSignedIn ? "Signed in" : "Signed out")}</b>");

            if (AuthenticationService.Instance.IsSignedIn)
            {
                if (m_SignOut != null)
                {
                    m_SignOut.SetActive(true);
                }

                statusBuilder.AppendLine(GetPlayerInfoText());
                statusBuilder.AppendLine($"PlayerId: <b>{AuthenticationService.Instance.PlayerId}</b>");
            }

            m_StatusText.text = statusBuilder.ToString();
            SetException(null);
        }

        string GetExternalIds(PlayerInfo playerInfo)
        {
            if (playerInfo.Identities == null)
            {
                return "None";
            }

            var sb = new StringBuilder();
            foreach (var id in playerInfo.Identities)
            {
                sb.Append(" " + id.TypeId);
            }

            return sb.ToString();
        }

        string GetPlayerInfoText()
        {
            return $"ExternalIds: <b>{m_ExternalIds}</b>";
        }

        void SetException(Exception ex)
        {
            m_ExceptionText.text = ex != null ? $"{ex.GetType().Name}: {ex.Message}" : "";
        }

        void LogPlayerName()
        {
            AuthenticationService.Instance.GetPlayerNameAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError(task.Exception);
                }
                else
                {
                    Debug.Log($"Player Name: {task.Result}");
                }
            });
        }

        public async Task<Dictionary<string, object>> GetPlayerScore()
        {
            var scoreResponse = await LeaderboardsService.Instance
                .GetPlayerScoreAsync(
                    "Leaderboard",
                    new GetPlayerScoreOptions { IncludeMetadata = true });

            string jsonString = JsonConvert.SerializeObject(scoreResponse);
            Dictionary<string, object> scoreDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

            return scoreDictionary;
        }




    }
}
