using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Mirror;
using MasterServerToolkit.MasterServer;

public enum ResourceType
{
    Default
}

public class TCGGameManager : NetworkBehaviour
{
    public static TCGGameManager Instance;

    public GameObject cardPrefab;

    public float cardSpawnDelay = 0.1f;

    [SyncVar]
    public uint playerTurn;
    [SyncVar]
    public int currentTurn;
    [SyncVar]
    public bool isGameOver = false;
    [SyncVar]
    public uint victoriousPlayer;
    [SyncVar]
    public uint defeatedPlayer;

    public TCGServer gameServer;
    private TCGClient gameClient;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        gameServer = GetComponent<TCGServer>();
        gameClient = GetComponent<TCGClient>();
    }

    #region COMMANDS

    [Command(requiresAuthority = false)]
    public void CMDPlayerConnected(uint playerID)
    {
        Debug.Log("CMDPlayerConnected: " + playerID.ToString());
        gameServer.OnPlayerConnected(playerID);
    }

    [Command(requiresAuthority = false)]
    public void CMDStartMatch(uint playerID)
    {
        gameServer.OnStartMatch(playerID);
    }

    [Command(requiresAuthority = false)]
    public void CMDDrawInitialCards(uint playerID)
    {
        gameServer.DrawInitialCards(playerID);
    }

    [Command(requiresAuthority = false)]
    public void CMDDrawCard(uint playerID, int drawAmount, bool isTurnStart, bool isOponent)
    {
        gameServer.DrawCard(playerID,drawAmount,isTurnStart,isOponent);
    }

    [Command(requiresAuthority = false)]
    public void CMDCombat(uint playerID, uint attackerID, uint targetID)
    {
        gameServer.ResolveCombat(playerID,attackerID,targetID);
    }

    // END OF TURN, PASS THE ACTION TO THE OTHER PLAYER AND INCREASE TURN COUNTER
    // ME + YOU = 1 TURN 
    [Command(requiresAuthority = false)]
    public void CMDEndTurn(uint playerID)
    {
        gameServer.OnEndTurn(playerID);
        UIManager.Instance.SetDebugText("Player: "+playerTurn.ToString()+"\n"+"Turn: "+currentTurn);
    }

    [Command(requiresAuthority = false)]
    public void CMDCastCard(uint playerID, uint cardNetID)
    {
        gameServer.CastCard(playerID, cardNetID);
    }
    

    #endregion

    #region RPCS

    [ClientRpc]
    public void RPCAllPlayersConnected(string json)
    {
        gameClient.OnDeckSpawnComplete(json);
        CMDDrawInitialCards(NetworkClient.localPlayer.netId);
    }

    //DRAW INITIAL AMOUNT OF CARDS
    [ClientRpc]
    public void RPCDrawInitialCards(string json)
    {
        gameClient.DrawInitialCards(json);
    }

    //DELIVER CARD TO HAND IN BOTH CLIENTS
    [ClientRpc]
    public void RPCDrawCard(string json, bool isTurnStart)
    {
        gameClient.DrawCard(json, isTurnStart);
    }

    [ClientRpc]
    public void RPCPlayerTurn()
    {
        gameClient.PlayerTurn();
    }

    //DELIVER CARD TO THE FIELD AND ACTIVATE ITS ON CAST EFFECT ON BOTH CLIENTS
    [ClientRpc]
    public void RPCCastCard(uint playerID, int actionID, uint cardNetID)
    {
        gameClient.CastCard(playerID, actionID, cardNetID);
    }

    [ClientRpc]
    public void RPCSummonCard(uint playerID, int actionID, uint cardNetID)
    {
        gameClient.SummonCard(playerID, actionID, cardNetID);
    }

    [ClientRpc]
    public void RPCResolveCombat(uint playerID, int actionID, uint attackerID, uint targetID)
    {
        gameClient.ResolveCombat(playerID, actionID, attackerID, targetID);
    }

    #endregion
}
