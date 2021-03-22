using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;

public class Card : NetworkBehaviour
{
    [SerializeField] private int value;
    [SerializeField] private int suit;
    [SerializeField] GameObject cardModel;
    [SerializeField] private int id = -1;
    [SerializeField] private Sprite cardSprite;

    public static event Action<int> CardPlayed;

    public int GetCardId()
    {
        return id;
    }

    public int GetValue()
    {
        return value;
    }

    public int GetSuit()
    {
        return suit;
    }

    public Sprite GetCardSprite()
    {
        return cardSprite;
    }
}
