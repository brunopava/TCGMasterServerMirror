using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class TCGBot : NetworkBehaviour
{
    public override void OnStartServer()
    {
        Debug.Log("TCGBot:OnStartLocalPlayer");
        Debug.Log(isLocalPlayer);
        if(!isLocalPlayer)
        {
            PlayerReady();
        }
    }

    public async void PlayerReady()
    {
        await new WaitForSeconds(1f);
        Debug.Log("PlayerReady: "+netId.ToString());
        TCGGameManager.Instance.CMDPlayerConnected(netId);
    }

    public void OnStartTurn()
    {
        DelayedStartTurn();
    }

    private async void DelayedStartTurn()
    {
        Debug.Log("DelayedStartTurn");

        // List<uint> fieldCards = new List<uint>();
        // foreach(CardBehaviour card in TCGArena.Instance.GetPlayerField())
        // {
        //     card.isInteractable = true;
        //     fieldCards.Add(card.netId);
        // }

        // string json = JsonConvert.SerializeObject(fieldCards);
        // CMDStartTurn(json);

        // TCGGameManager.Instance.CMDDrawCard(NetworkClient.localPlayer.netId, 1, true, false);
        TCGGameManager.Instance.CMDEndTurn(NetworkClient.localPlayer.netId);
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
        // List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

        // foreach(uint current in cards)
        // {
        //     CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();
        //     card.isAttackEnabled = true;
        // }
    }
}
