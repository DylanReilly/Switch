using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

public class Deck : NetworkBehaviour
{
    [SerializeField] private List<Card> gameDeck = new List<Card>();
    [SyncVar] private Card topCard;

    public Card GetTopCard()
    {
        return topCard;
    }

    #region Server
    //Load and shuffle deck when game starts
    private void Start()
    {
        Object[] loadDeck;
        loadDeck = Resources.LoadAll("Cards/CardInstances", typeof(Card));
        Shuffle(loadDeck);

        foreach (Card card in loadDeck)
        {
            gameDeck.Add(card);
        }
    }

    //Used to shuffle the deck
    public void Shuffle(Object[] deck)
    {
        Object tempGO;
        for (int i = 0; i < deck.Length; i++)
        {
            int rnd = Random.Range(0, deck.Length);
            tempGO = deck[rnd];
            deck[rnd] = deck[i];
            deck[i] = tempGO;
        }
    }

    public void PlayCard(Card card)
    {
        gameDeck.Add(card);
    }
    
    #endregion
}
