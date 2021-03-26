using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckHandler : NetworkBehaviour
{
    [SerializeField] Deck deck = null;
    private Dictionary<int, Card> refDeck = new Dictionary<int, Card>();
    SwitchPlayer player = null;

    public event Action PickedupCard;

    private void Start()
    {
        deck.TopCardChanged += RenderTopCard;

        UnityEngine.Object[] loadDeck;
        loadDeck = Resources.LoadAll("Cards/CardInstances", typeof(Card));

        foreach (Card card in loadDeck)
        {
            refDeck.Add(card.GetCardId(), card);
        }
    }

    private void OnDestroy()
    {
        deck.TopCardChanged -= RenderTopCard;
    }

    private void RenderTopCard(int cardId)
    {
        GameObject currentCard = GameObject.FindWithTag("Card");
        if (currentCard != null)
        {
            Destroy(currentCard, 0f);
        }
        GameObject card = Instantiate(refDeck[cardId].gameObject, this.gameObject.transform);
    }

    [Client]
    public void PickupCard()
    {
        player = NetworkClient.connection.identity.GetComponent<SwitchPlayer>();

        player.AddCard(refDeck[deck.GetTopCard()]);

        deck.RemoveTopCard();
    }

}
