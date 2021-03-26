using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class UICardRenderer : MonoBehaviour
{
    [SerializeField] private Image imagePrefab;
    [SerializeField] GameObject HandStartPosition;

    private Dictionary<int, Card> referenceDeck = new Dictionary<int, Card>();

    [SerializeField]private SwitchPlayer player;
    //An offset to stop cards rendering directly on top of each other
    int OFFSET = 0;

    private void Start()
    {
        //Cache player when object is created
        player = NetworkClient.connection.identity.GetComponent<SwitchPlayer>();

        //Update card UI when event is invoked
        player.MyCardsUpdated += UpdateCardUI;

        LoadReferenceDeck();
    }

    private void OnDestroy()
    {
        //Unsubscribe from event when destroyed
        player.MyCardsUpdated -= UpdateCardUI;
    }

    //Loads dictionary storing card data, using cardId as a key
    private void LoadReferenceDeck()
    {
        UnityEngine.Object[] loadDeck;
        loadDeck = Resources.LoadAll("Cards/CardInstances", typeof(Card));

        foreach (Card card in loadDeck)
        {
            referenceDeck.Add(card.GetCardId(), card);
        }
    }

    public void UpdateCardUI(int cardId)
    {
        Debug.Log("Render started");
        //Renders the latest card in the players hand to the UI

        Card card = referenceDeck[cardId];

        Image imageInstance = Instantiate(imagePrefab);
        imageInstance.transform.SetParent(HandStartPosition.transform, false);
        imageInstance.sprite = card.GetCardSprite();
        imageInstance.rectTransform.anchoredPosition += new Vector2(OFFSET, 0);

        //Offset moves cards over so they arent rendered on top of each other
        OFFSET += 50;
        Debug.Log("Render finished");
    }
}
