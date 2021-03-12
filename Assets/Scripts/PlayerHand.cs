using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private List<Card> hand = new List<Card>();

    public List<Card> GetHand()
    {
        return hand;
    }

    public void AddCards(List<Card> newCards)
    {
        if(newCards.Count == 0) { return; }

        foreach(Card card in newCards)
        {
            hand.Add(card);
        }
    }

    
}
