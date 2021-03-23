using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using System;

public class Deck : NetworkBehaviour
{
    [SerializeField] private Stack<Card> gameDeck = new Stack<Card>();
    [SerializeField] private Card topCard;
    public static event Action DeckSpawned;

    public Card GetTopCard()
    {
        return gameDeck.Peek();
    }

    #region Server
    private void Start()
    {
        LoadDeck();
        topCard = gameDeck.Peek();
        DeckSpawned?.Invoke();
    }

    //Used to shuffle the deck
    public void Shuffle(UnityEngine.Object[] deck)
    {
        UnityEngine.Object tempGO;
        for (int i = 0; i < deck.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, deck.Length);
            tempGO = deck[rnd];
            deck[rnd] = deck[i];
            deck[i] = tempGO;
        }
    }

    //Loads deck from resources and shuffles order
    private void LoadDeck()
    {
        UnityEngine.Object[] loadDeck;
        loadDeck = Resources.LoadAll("Cards/CardInstances", typeof(Card));
        Shuffle(loadDeck);

        foreach (Card card in loadDeck)
        {
            gameDeck.Push(card);
        }
    }

    public int DealCard()
    {
        return gameDeck.Pop().GetCardId();
    }

    private void UpdateTopCard(Card oldCard, Card newCard)
    {
        topCard = gameDeck.Peek();
    }

    public void PlayCard(Card card)
    {
        gameDeck.Push(card);
    }
    
    #endregion
}
