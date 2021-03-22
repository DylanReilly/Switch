using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEditor;

public class SwitchPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;
    [SerializeField] private PlayerHand hand = null;
    [SerializeField] private Deck deck = null;
    private List<Card> cardsToPlay = null;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    public event Action HandChanged;

    public PlayerHand GetHand()
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
        foreach (Card card in hand.GetCards())
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

        //Only play card if suit or value match, or if card is an Ace
        if (deck.GetTopCard().GetSuit() != cardToPlay.GetSuit() 
            && deck.GetTopCard().GetValue() != cardToPlay.GetValue()
            || cardToPlay.GetValue() != 1) { return; }

        //Add card to deck
        deck.PlayCard(cardToPlay);

        //Triggers event to update UI
        HandChanged?.Invoke();
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
