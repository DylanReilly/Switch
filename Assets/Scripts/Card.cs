using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Card : NetworkBehaviour
{
    [SerializeField] private int value;
    [SerializeField] private char suit;
    [SerializeField] GameObject cardModel;

    private int owner;

    public void SetOwner(int newOwner)
    {
        owner = newOwner;
    }

    public int GetValue()
    {
        return value;
    }

    public char GetSuit()
    {
        return suit;
    }
}
