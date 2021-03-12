using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Deck : NetworkBehaviour
{
    private Stack<Card> gameDeck = new Stack<Card>();
    [SyncVar]private Card topCard;

    public void AddCard(Card card)
    {
        if(card.GetSuit() != topCard.GetSuit() || card.GetValue() != topCard.GetValue()) { return; }
        gameDeck.Push(card);

    }

    public void DrawCard()
    { 
        
    }
}
