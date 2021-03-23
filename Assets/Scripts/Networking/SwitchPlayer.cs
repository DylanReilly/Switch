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
    [SerializeField] private List<Card> hand = null;
    public Deck deck = null;

    //Stores a copy of every card for working over the network
    private Dictionary<int, Card> referenceDeck = new Dictionary<int, Card>();

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))] private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))] private string displayName;

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    public event Action HandChanged;

    private void Start()
    {
        //Sets eck object only when deck has been spawned
        Deck.DeckSpawned += FindDeck;

        //Loads a reference deck of key & value pairs to enable easy lookup
        LoadReferenceDeck();
    }

    private void OnDestroy()
    {
        Deck.DeckSpawned -= FindDeck;
    }

    public List<Card> GetHand()
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
    private void FindDeck()
    {
        deck = GameObject.FindWithTag("Deck").GetComponent<Deck>();
    }

    //Takes in card ID, finds card in reference deck and adds it to the hand
    private void DrawCard(int cardId)
    {
        hand.Add(referenceDeck[cardId]);
        HandChanged?.Invoke();
    }

    private void LoadReferenceDeck()
    {
        UnityEngine.Object[] loadDeck;
        loadDeck = Resources.LoadAll("Cards/CardInstances", typeof(Card));

        foreach (Card card in loadDeck)
        {
            referenceDeck.Add(card.GetCardId(), card);
        }
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

    //Try to move a card from player hand to the deck
    [Command]
    public void CmdTryPlayCard(int cardId)
    {
        Card cardToPlay = null;
        foreach (Card card in hand)
        {
            if (card.GetCardId() == cardId)
            {
                cardToPlay = card;
                break;
            }
        }

        if(cardToPlay = null) 
        {
            Debug.Log("Card is null");    
            return; 
        }

        //Only play card if suit or value match, or if card is an Ace(ID of 1)
        if (deck.GetTopCard().GetSuit() != cardToPlay.GetSuit() 
            && deck.GetTopCard().GetValue() != cardToPlay.GetValue()
            || cardToPlay.GetValue() != 1) { return; }

        //Add card to deck
        deck.PlayCard(cardToPlay);

        //Triggers event to update UI
        HandChanged?.Invoke();
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
