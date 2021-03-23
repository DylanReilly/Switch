using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControls : MonoBehaviour
{
    private SwitchPlayer player = null;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<SwitchPlayer>();
    }

    public void DrawCard()
    {
        player.CmdTryDrawCard();
    }
}
