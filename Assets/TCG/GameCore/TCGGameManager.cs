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
    public GameObject botPrefab;

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
    public TCGClient gameClient;

    public bool isBotGame = true;
    public bool isBotCreated = false;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        gameServer = GetComponent<TCGServer>();
        gameClient = GetComponent<TCGClient>();
    }

    public override void OnStartServer()
    {
        if(isBotGame && !isBotCreated)
        {
            gameServer.CreateBot();
            isBotCreated = true;
        }
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
        Debug.Log("CMDStartMatch: " + playerID.ToString());
        gameServer.OnStartMatch(playerID);
    }

    [Command(requiresAuthority = false)]
    public void CMDDrawInitialCards(uint playerID)
    {
        Debug.Log("CMDDrawInitialCards: " + playerID.ToString());
        gameServer.DrawInitialCards(playerID);
    }

    [Command(requiresAuthority = false)]
    public void CMDDrawCard(uint playerID, int drawAmount, bool isTurnStart, bool isOponent)
    {
        Debug.Log("CMDDrawCard: " + playerID.ToString());
        gameServer.DrawCard(playerID,drawAmount,isTurnStart,isOponent);
    }

    [Command(requiresAuthority = false)]
    public void CMDCombat(uint playerID, uint attackerID, uint targetID)
    {
        Debug.Log("CMDCombat");
        gameServer.ResolveCombat(playerID,attackerID,targetID);
    }
    
    [Command(requiresAuthority = false)]
    public void CMDEndTurn(uint playerID)
    {
        gameServer.OnEndTurn(playerID);
        UIManager.Instance.SetDebugText("Player: "+playerTurn.ToString()+"\n"+"Turn: "+currentTurn);
    }

    [Command(requiresAuthority = false)]
    public void CMDCastCard(uint playerID, uint cardNetID)
    {
        // Debug.Log(playerID);
        // Debug.Log(cardNetID);
        gameServer.CastCard(playerID, cardNetID);
    }
    

    #endregion

    #region RPCS

    [ClientRpc]
    public void RPCAllPlayersConnected(string json)
    {
        gameClient.OnDeckSpawnComplete(json);
    }

    [ClientRpc]
    public void RPCDrawInitialCards(string json)
    {
        gameClient.DrawInitialCards(json);
    }

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

    [ClientRpc]
    public void RPCCastCard(uint playerID, int actionID, uint cardNetID)
    {
        Debug.Log("RPCCastCard");
        gameClient.CastCard(playerID, actionID, cardNetID);
    }

    [ClientRpc]
    public void RPCSummonCard(uint playerID, int actionID, uint cardNetID)
    {
        Debug.Log("RPCSummonCard");
        gameClient.SummonCard(playerID, actionID, cardNetID);
    }

    [ClientRpc]
    public void RPCResolveCombat(uint playerID, int actionID, uint attackerID, uint targetID)
    {
        gameClient.ResolveCombat(playerID, actionID, attackerID, targetID);
    }

    #endregion
}
