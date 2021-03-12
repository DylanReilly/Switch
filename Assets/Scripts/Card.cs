using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Card : NetworkBehaviour
{
    [SerializeField] private int value;
    [SerializeField] private int suit;
    [SerializeField] GameObject cardModel;
    [SerializeField] private int id = -1;
    [SerializeField] private GameObject cardSprite;

    public static event Action<int> CardPlayed;

    public int GetValue()
    {
        return value;
    }

    public int GetSuit()
    {
        return suit;
    }

    public void PlayCard(Card card)
    {
        CardPlayed?.Invoke(this.GetValue());
    }
}
