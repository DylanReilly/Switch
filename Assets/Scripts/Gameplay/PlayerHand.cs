using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private List<Card> cards = new List<Card>();

    public List<Card> GetCards()
    {
        return cards;
    }

    public void AddCards(List<Card> newCards)
    {
        if(newCards.Count == 0) { return; }

        foreach(Card card in newCards)
        {
            cards.Add(card);
        }
    }

    public void AddSingleCard(Card card)
    {
        cards.Add(card);
    }

    
}
