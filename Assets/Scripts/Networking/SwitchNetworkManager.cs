using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Extends network manager for network functionality
public class SwitchNetworkManager : NetworkManager
{
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;
    [SerializeField] private Deck deckPrefab = null;

    public List<SwitchPlayer> Players { get; } = new List<SwitchPlayer>();

    private bool isGameInProgress = false;

    #region Server
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress) { return; }

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        SwitchPlayer player = conn.identity.GetComponent<SwitchPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        if (Players.Count < 1) { return; }

        isGameInProgress = true;

        ServerChangeScene("Scene_Map_01");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //Performs all base logic of the overrided class
        base.OnServerAddPlayer(conn);

        SwitchPlayer player = conn.identity.GetComponent<SwitchPlayer>();

        Players.Add(player);

        player.SetDisplayName($"Player{Players.Count}");

        player.SetPartyOwner(Players.Count == 1);
    }

    //Called when the scene is changed
    public override void OnServerSceneChanged(string sceneName)
    {
        //Checks the name to see if its a map, menu etc
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Menu"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }

        ////Spawns the deck on the network
        //if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        //{
        //    Deck deck = Instantiate(deckPrefab, new Vector3(0f, 0.55f, 0f), Quaternion.identity);
        //    NetworkServer.Spawn(deck.gameObject);
        //}
    }
    #endregion

    #region Client
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
    }
    #endregion
}
