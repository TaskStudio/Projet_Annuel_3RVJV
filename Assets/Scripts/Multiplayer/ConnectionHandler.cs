using System;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

public enum ConnectionType
{
    Host,
    Client
}

public class ConnectionHandler : MonoBehaviour
{
    public ConnectionType connectionType;

#if UNITY_EDITOR
    private void OnEnable()
    {
        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState; 
    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState; 
    }

    private void OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopping)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
#endif
    
    private void Start()
    {
        #if UNITY_EDITOR
        Debug.Log("ConnectionHandler Start: Unity Editor mode");

        // Add checks and logs for InstanceFinder
        if (InstanceFinder.ClientManager == null)
        {
            Debug.LogError("ConnectionHandler Start: ClientManager is null.");
            return;
        }

        if (InstanceFinder.ServerManager == null)
        {
            Debug.LogError("ConnectionHandler Start: ServerManager is null.");
            return;
        }

        if (ParrelSync.ClonesManager.IsClone())
        {
            Debug.Log("ConnectionHandler Start: IsClone is true, starting client connection");
            InstanceFinder.ClientManager.StartConnection();
        }
        else
        {
            Debug.Log("ConnectionHandler Start: IsClone is false, checking connectionType");

            if (connectionType == ConnectionType.Host)
            {
                Debug.Log("ConnectionHandler Start: ConnectionType is Host, starting server and client connection");
                InstanceFinder.ServerManager.StartConnection();
                InstanceFinder.ClientManager.StartConnection();
            }
            else
            {
                Debug.Log("ConnectionHandler Start: ConnectionType is Client, starting client connection");
                InstanceFinder.ClientManager.StartConnection();
            }
        }
        #endif
        
        #if DEDICATED_SERVER
        Debug.Log("ConnectionHandler Start: Dedicated server mode, starting server connection");
        if (InstanceFinder.ServerManager != null)
        {
            InstanceFinder.ServerManager.StartConnection();
        }
        else
        {
            Debug.LogError("ConnectionHandler Start: ServerManager is null.");
        }
        #endif
    }
}
