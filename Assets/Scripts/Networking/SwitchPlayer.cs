using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SwitchPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))] private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))] private string displayName;

    [SerializeField]private List<Card> myCards = new List<Card>();

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    public event Action<int> MyCardsUpdated;

    public string GetDisplayName()
    {
        return displayName;
    }

    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public void AddCard(Card card)
    {
        myCards.Add(card);
        MyCardsUpdated?.Invoke(card.GetCardId());
    }

    #region Server
    public override void OnStartServer()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }


    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) { return; }

        ((SwitchNetworkManager)NetworkManager.singleton).StartGame();
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return; }
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }

        DontDestroyOnLoad(gameObject);

        ((SwitchNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();
        if (!isClientOnly) { return; }
        ((SwitchNetworkManager)NetworkManager.singleton).Players.Remove(this);
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    
    #endregion
}
