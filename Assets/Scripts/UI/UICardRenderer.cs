using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class UICardRenderer : MonoBehaviour
{
    [SerializeField] private Image imagePrefab;
    [SerializeField] GameObject HandStartPosition;

    private SwitchPlayer player;
    //An offset to stop cards rendering directly on top of each other
    int OFFSET = 0;

    private void Start()
    {
        //Cache player when object is created
        player = NetworkClient.connection.identity.GetComponent<SwitchPlayer>();

        //Render cards when game starts
        UpdateCardUI();

        //Update card UI when event is invoked
        player.PickedUpCard += UpdateCardUI;
    }

    private void OnDestroy()
    {
        //Unsubscribe from event when destroyed
        player.PickedUpCard -= UpdateCardUI;
    }

    private void UpdateCardUI()
    {
        //Loop through the players hand and render cards sequencially on UI
        foreach (Card card in player.GetHand().GetCards())
        {
            Image imageInstance = Instantiate(imagePrefab);
            imageInstance.transform.SetParent(HandStartPosition.transform, false);
            imageInstance.sprite = card.GetCardSprite();
            imageInstance.rectTransform.anchoredPosition += new Vector2(OFFSET, 0);

            //Offset moves cards over so they arent rendered on top of each other
            OFFSET += 50;
        }
    }
}
