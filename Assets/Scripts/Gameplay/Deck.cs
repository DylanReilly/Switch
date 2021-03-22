using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

public class Deck : NetworkBehaviour
{
    [SerializeField] private List<Card> gameDeck = new List<Card>();
    [SyncVar] private Card topCard;

    #region Server
    //Load and shuffle deck when game starts
    private void Start()
    {
        Object[] loadDeck;
        loadDeck = Resources.LoadAll("Cards/CardInstances", typeof(Card));
        Shuffle(loadDeck);

        foreach (Card card in loadDeck)
        {
            Debug.Log(card.GetCardId());
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
    
    #endregion
}
