using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        List<uint> fieldCards = new List<uint>();
        foreach(CardBehaviour card in TCGArena.Instance.GetPlayerField())
        {
            card.isInteractable = true;
            fieldCards.Add(card.netId);
        }

        string json = JsonConvert.SerializeObject(fieldCards);
        CMDStartTurn(json);

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

    [Command(requiresAuthority = false)]
    public void CMDStartTurn(string json)
    {
        List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

        foreach(uint current in cards)
        {
            CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();
            card.isAttackEnabled = true;
        }
    }
}
