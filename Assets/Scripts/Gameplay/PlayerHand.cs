using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private List<Card> cards = null;

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

    
}
