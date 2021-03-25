using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using System;

public class Deck : NetworkBehaviour
{
    [SerializeField] private Stack<int> gameDeck = new Stack<int>();
    [SerializeField][SyncVar] private int topCardId;
    public static event Action DeckSpawned;
    public static event Action<int> TopCardChanged;

    public int GetTopCardId()
    {
        return topCardId;
    }

    #region Server
    public override void OnStartServer()
    {
        LoadDeck();
        //DeckSpawned?.Invoke();
        topCardId = gameDeck.Peek();
    }

    //Used to shuffle the deck
    public void Shuffle(int[] deck)
    {
        int tempGO;
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
        //Fill an array with numbers 1 - 52, representing cardIds
        int[] loadDeck = new int[52];
        for (int i = 0; i < 52; i++)
        {
            loadDeck[i] = i + 1;
        }
        
        //Shuffle the deck
        Shuffle(loadDeck);

        //Push shuffled numbers onto stack
        for (int i = 0; i < loadDeck.Length; i++)
        {
            gameDeck.Push(loadDeck[i]);
        }
    } 
    #endregion
}
