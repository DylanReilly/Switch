using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using System;

public class Deck : NetworkBehaviour
{
    [SerializeField] private Stack<Card> gameDeck = new Stack<Card>();
    [SerializeField][SyncVar(hook = nameof(OnTopCardUpdated))] private int topCardId = 0;

    public event Action<int> TopCardChanged;
    public static event Action DeckSpawned;

    public int GetTopCard()
    {
        return topCardId;
    }

    #region Server
    [Server]
    public override void OnStartServer()
    {
        LoadDeck();
        DeckSpawned?.Invoke();
        topCardId = gameDeck.Peek().GetCardId();
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

        foreach (Card card in loadDeck)
        {
            gameDeck.Push(card);
            //resourceDeck.Add(card.GetCardId(), card);
        }

        //Shuffle the deck
        Shuffle(loadDeck);

        //Push shuffled numbers onto stack
        foreach(Card card in loadDeck)
        {
            gameDeck.Push(card);
        }
    }

    private void OnTopCardUpdated(int oldCardId, int newCardId)
    {
        TopCardChanged?.Invoke(newCardId);
    }
    #endregion

    #region client
    private void RenderTopCard(int cardId)
    {
        GameObject card = Instantiate(gameDeck.Peek().gameObject, this.gameObject.transform);
        NetworkServer.Spawn(card);
    }


    #endregion
}
