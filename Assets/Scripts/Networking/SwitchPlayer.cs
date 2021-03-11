﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class SwitchPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;

    //Hook is used in syncvars, Whenever the variable is changed the hooked method is called
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    public string getDisplayName()
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


    #region Server

    //subscribes to events, meaning it will listen and react to these events
    public override void OnStartServer()
    {
        DontDestroyOnLoad(gameObject);
    }

    //Unsubscribes from events, stops listening
    public override void OnStopServer()
    {

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

    //Subscribes to events
    public override void OnStartAuthority()
    {
        //Ignore if you are the server
        if (NetworkServer.active) { return; }
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }

        DontDestroyOnLoad(gameObject);

        ((SwitchNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    //Unsubscribe from events
    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        //Ignore if you are the server
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