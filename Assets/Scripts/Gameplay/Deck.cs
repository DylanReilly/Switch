using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using System;

public class Deck : NetworkBehaviour
{
    [SerializeField][SyncVar] private List<int> gameDeck = new List<int>();
    [SerializeField][SyncVar(hook = nameof(OnTopCardUpdated))] private int topCardId = 0;
    [SerializeField] private DeckHandler deckHandler = null;

    public event Action<int> TopCardChanged;
    public static event Action DeckSpawned;

    public int GetTopCard()
    {
        return gameDeck[gameDeck.Count - 1];
    }

    #region Server
    [Server]
    public override void OnStartServer()
    {
        LoadDeck();
        DeckSpawned?.Invoke();
        topCardId = gameDeck[gameDeck.Count - 1];
    }

    //Used to shuffle the deck
    [Server]
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
    [Server]
    private void LoadDeck()
    {
        UnityEngine.Object[] loadDeck;
        loadDeck = Resources.LoadAll("Cards/CardInstances", typeof(Card));

        //Shuffle the deck
        Shuffle(loadDeck);

        //Push shuffled numbers onto stack
        foreach(Card card in loadDeck)
        {
            gameDeck.Add(card.GetCardId());
        }
    }

    private void OnTopCardUpdated(int oldCardId, int newCardId)
    {
        TopCardChanged?.Invoke(newCardId);
    }

    public void RemoveTopCard()
    {
        gameDeck.RemoveAt(gameDeck.Count - 1);
        topCardId = gameDeck[gameDeck.Count - 1];
    }
    #endregion

    #region client
    #endregion
}
