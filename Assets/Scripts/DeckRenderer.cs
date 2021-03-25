using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckRenderer : MonoBehaviour
{
    [SerializeField] Deck deck = null;
    [SerializeField] int topCardId = 0;
    private Dictionary<int, Card> refDeck = new Dictionary<int, Card>();

    private void Start()
    {
        deck.TopCardChanged += RenderTopCard;
        topCardId = deck.GetTopCard();

        UnityEngine.Object[] loadDeck;
        loadDeck = Resources.LoadAll("Cards/CardInstances", typeof(Card));

        foreach (Card card in loadDeck)
        {
            refDeck.Add(card.GetCardId(), card);
        }
    }

    private void RenderTopCard(int cardId)
    {
        Debug.Log("Rendered");
        Debug.Log(cardId);
        topCardId = cardId;
        GameObject card = Instantiate(refDeck[cardId].gameObject, this.gameObject.transform);
    }

}
