using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Threading.Tasks;

public class TCGPlayerManager : NetworkBehaviour
{
	public override void OnStartLocalPlayer()
    {
        if(isLocalPlayer)
        {
            PlayerReady();
        }
    }

    public async void PlayerReady()
    {
        TCGGameManager.Instance.CMDPlayerConnected(netId);
    }

    public void OnStartTurn()
    {
        DelayedStartTurn();
    }

    private async void DelayedStartTurn()
    {
    	Debug.Log("DelayedStartTurn");

        foreach(CardBehaviour card in TCGArena.Instance.GetPlayerField())
        {
            card.isInteractable = true;
            card.isAttackEnabled = true;
        }

        TCGGameManager.Instance.CMDDrawCard(NetworkClient.localPlayer.netId, 1, true, false);
    }

    [TargetRpc]
    public void OnTurnEnd()
    {
        DelayedEndTurn();
    }

    private async void DelayedEndTurn()
    {
    	Debug.Log("DelayedEndTurn");
    }
}
