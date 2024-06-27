using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public List<GameObject> meleeAttackers;
    public List<GameObject> rangedAttackers;

    private Dictionary<NetworkConnection, GameObject> playerInstances = new Dictionary<NetworkConnection, GameObject>();

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        InstanceFinder.ServerManager.OnServerConnectionState += OnServerConnectionState;
        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
    }

    private void OnServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            Debug.Log("Server started");
        }
    }

    private void OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            if (IsHost)
            {
                AssignPlayerInstance(InstanceFinder.ClientManager.Connection);
            }
        }
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        InstanceFinder.ServerManager.OnServerConnectionState -= OnServerConnectionState;
        InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
    }

    [Server]
    public void AssignPlayerInstance(NetworkConnection conn)
    {
        GameObject playerInstance = conn.ClientId % 2 == 0 ? GetAvailableMeleeAttacker() : GetAvailableRangedAttacker();
        if (playerInstance != null)
        {
            // Give ownership to the connecting player
            playerInstance.GetComponent<NetworkObject>().GiveOwnership(conn);
            playerInstances.Add(conn, playerInstance);
        }
        else
        {
            Debug.LogWarning("No available player instance to assign.");
        }
    }

    private GameObject GetAvailableMeleeAttacker()
    {
        foreach (var attacker in meleeAttackers)
        {
            if (!attacker.GetComponent<NetworkObject>().IsOwner)
            {
                return attacker;
            }
        }
        return null;
    }

    private GameObject GetAvailableRangedAttacker()
    {
        foreach (var attacker in rangedAttackers)
        {
            if (!attacker.GetComponent<NetworkObject>().IsOwner)
            {
                return attacker;
            }
        }
        return null;
    }

    public void OnNetworkDespawn()
    {
        foreach (var playerInstance in playerInstances.Values)
        {
            if (playerInstance != null)
            {
                playerInstance.GetComponent<NetworkObject>().RemoveOwnership();
            }
        }
        playerInstances.Clear();
    }
}
