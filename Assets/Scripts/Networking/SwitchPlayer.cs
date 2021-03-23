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
    [SerializeField] private List<int> hand = new List<int>();
    public Deck deck = null;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))] private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))] private string displayName;

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    public event Action<int> HandChanged;

    private void Start()
    {
        //Sets deck object only when deck has been spawned
        Deck.DeckSpawned += FindDeck;
    }

    private void OnDestroy()
    {
        Deck.DeckSpawned -= FindDeck;
    }

    public List<int> GetHand()
    {
        return hand;
    }

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

    //Sets the deck field for this object
    [Client]
    private void FindDeck()
    {
        deck = GameObject.FindWithTag("Deck").GetComponent<Deck>();
    }

    //Takes in card ID, finds card in reference deck and adds it to the hand
    private void DrawCard(int cardId)
    {
        hand.Add(cardId);
    }

    private void HandUpdated(int cardId)
    {
        HandChanged?.Invoke(cardId);
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

    [Command]
    public void CmdTryDrawCard()
    {
        DrawCard(deck.DealCard());
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
