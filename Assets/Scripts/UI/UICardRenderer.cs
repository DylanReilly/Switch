using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class UICardRenderer : MonoBehaviour
{
    [SerializeField] private Image imagePrefab;
    [SerializeField] GameObject HandStartPosition;
    [SerializeField] List<Card> cards = new List<Card>();

    //An offset to stop cards rendering directly on top of each other
    int OFFSET = 0;

    void Start()
    {
        foreach (Card card in cards)
        {
            Image imageInstance = Instantiate(imagePrefab);
            imageInstance.transform.SetParent(HandStartPosition.transform, false);
            imageInstance.sprite = card.GetCardSprite();
            imageInstance.rectTransform.anchoredPosition += new Vector2(OFFSET, 0);
            
            OFFSET += 50;
        }
    }
}
