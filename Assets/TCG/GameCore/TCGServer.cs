using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Mirror;
using MasterServerToolkit.MasterServer;

public class TCGServer:MonoBehaviour
{
	//Connected player id's
    public List<uint> connectedPlayers = new List<uint>();

    //This is used when we need confirmation from both players that an action was taken in both clients
    public List<uint> temporaryPhaseList = new List<uint>();

    //This is used to keep track that both players finished the current turn
    public List<uint> temporaryTurnCounter = new List<uint>();

    public Dictionary<uint, List<CardBehaviour>> decksByPlayer = new Dictionary<uint, List<CardBehaviour>>();
    public Dictionary<uint, List<CardBehaviour>> handsByPlayer = new Dictionary<uint, List<CardBehaviour>>();
    public Dictionary<uint, List<CardBehaviour>> graveyardByPlayer = new Dictionary<uint, List<CardBehaviour>>();
    public Dictionary<uint, List<CardBehaviour>> fieldByPlayer = new Dictionary<uint, List<CardBehaviour>>();
    public Dictionary<uint, int> lifeByPlayer = new Dictionary<uint, int>();
    public Dictionary<uint, Dictionary<ResourceType, int>> resourcesByPlayer = new Dictionary<uint, Dictionary<ResourceType, int>>();
    public List<uint> cardsAttackedThisTurn = new List<uint>();

    public void OnPlayerConnected(uint playerID)
    {
    	if (!connectedPlayers.Contains(playerID))
        {
            connectedPlayers.Add(playerID);

            decksByPlayer.Add(playerID, new List<CardBehaviour>());

            if (connectedPlayers.Count == TCGConstants.MAX_PLAYERS_CARD_GAME)
            {
                LoadCards();
            }
        }
    }

    public async void LoadCards()
    {
        Dictionary<uint, List<uint>> decks = new Dictionary<uint, List<uint>>(); 

        foreach (uint currentPlayer in connectedPlayers)
        {
            NetworkIdentity player = NetworkServer.spawned[currentPlayer];
            TCGPlayerManager playerManager = player.GetComponent<TCGPlayerManager>();

            for (int i = 0; i < TCGConstants.MAX_CARDS_PER_DECK; i++)
            {
                GameObject card = Instantiate(TCGGameManager.Instance.cardPrefab, new Vector3(-8000, 0.1f, 0), Quaternion.identity);
                CardBehaviour cardBehaviour = card.GetComponent<CardBehaviour>();
                
                decksByPlayer[currentPlayer].Add(cardBehaviour);
                NetworkServer.Spawn(card, player.connectionToClient);
                
                await new WaitForSeconds(TCGGameManager.Instance.cardSpawnDelay);
            }

            if (!decks.ContainsKey(currentPlayer))
            {
                decks.Add(currentPlayer, new List<uint>());
            }

            foreach (CardBehaviour current in decksByPlayer[currentPlayer])
            {
                decks[currentPlayer].Add(current.netId);
            }
        }

        string json = JsonConvert.SerializeObject(decks);

        //ALL PLAYERS CONNECTED
        TCGGameManager.Instance.RPCAllPlayersConnected(json);
    }

    public void DrawInitialCards(uint playerID)
    {
    	if(!PhaseCheck(playerID))
        {
            return;
        }

        List<uint> handCards = new List<uint>();

        foreach (uint currentPlayer in connectedPlayers)
        {
            handsByPlayer.Add(currentPlayer, new List<CardBehaviour>());

            int totalCards = decksByPlayer[currentPlayer].Count - 1;

            for (int i = totalCards; i > totalCards - TCGConstants.INITIAL_DRAW_AMOUNT; i--)
            {
                CardBehaviour card = decksByPlayer[currentPlayer][i];

                handsByPlayer[currentPlayer].Add(card);
                decksByPlayer[currentPlayer].Remove(card);

                handCards.Add(card.netId);
            }
        }

        string json = JsonConvert.SerializeObject(handCards);
        TCGGameManager.Instance.RPCDrawInitialCards(json);
    }

    public void OnEndTurn(uint playerID)
    {
    	// actionChain.ResetTurnInactivity();
        
        cardsAttackedThisTurn = new List<uint>();

        NetworkIdentity player = NetworkServer.spawned[playerID];
        TCGPlayerManager manager = player.GetComponent<TCGPlayerManager>();
        manager.OnTurnEnd();

        foreach (uint current in connectedPlayers)
        {
            if (current != playerID)
            {
                playerTurn = current;

                if (temporaryTurnCounter.Count == 0)
                {
                    currentTurn++;
                }

                temporaryTurnCounter.Add(playerTurn);

                if (temporaryTurnCounter.Count == TCGConstants.MAX_PLAYERS_CARD_GAME)
                {
                    temporaryTurnCounter = new List<uint>();
                }
                break;
            }
        }

        #if UNITY_EDITOR 
            //TODO: This is bad code for release and future testing.
            //Remove after multiple player mechanics phase
            if(temporaryTurnCounter.Count == connectedPlayers.Count)
            {
                temporaryTurnCounter = new List<uint>();
                currentTurn++;
            }
            string label = "Player: {0} Turn: {1}";
            UIManager.Instance.debugTurn.text = string.Format(label, playerTurn.ToString(), currentTurn.ToString());
        #endif

        DelayedTurnPass();
    }

    //SERVER ONLY
    private async void DelayedTurnPass()
    {
        await new WaitForSeconds(0.5f);
        TCGGameManager.Instance.RPCPlayerTurn();
    }

    public void DrawCard(uint playerID, int drawAmount, bool isTurnStart, bool isOponent)
    {
    	uint targetID = playerID;
        if(isOponent)
        {
            targetID = GetOponentID(playerID);
        }

        List<uint> draw_cards = new List<uint>();
        List<uint> discard_cards = new List<uint>();

        CheckIsGameOver(playerID);

        if (!isGameOver)
        {
            int totalCards = decksByPlayer[targetID].Count - 1;

            for (int i = totalCards; i > totalCards - drawAmount; i--)
            {
                CardBehaviour card = decksByPlayer[targetID][i];
                decksByPlayer[targetID].Remove(card);
                if (handsByPlayer[targetID].Count >= TCGConstants.MAX_CARDS_PER_HAND)
                {
                    graveyardByPlayer[targetID].Add(card);
                    discard_cards.Add(card.netId);
                }
                else
                {
                    handsByPlayer[targetID].Add(card);
                    draw_cards.Add(card.netId);
                }
            }

            string jsonDraw = JsonConvert.SerializeObject(draw_cards);
            string jsonDiscard = JsonConvert.SerializeObject(discard_cards);

            // RPCSendCardToGraveyard(jsonDiscard);
            // RPCDrawCard(jsonDraw, isTurnStart);
        }
        else
        {
            // ServerGameOver(playerID);
        }
    }

    public void OnStartMatch(uint playerID)
    {
    	// actionChain.GameStarted(
        //     ()=>{
        //         CMDEndTurn(playerTurn);
        //     }
        // );

        if(!PhaseCheck(playerID))
        {
            return;
        }

        temporaryTurnCounter = new List<uint>();

        if (!temporaryTurnCounter.Contains(playerTurn))
        {
            temporaryTurnCounter.Add(playerTurn);
        }
        
        TCGGameManager.Instance.RPCPlayerTurn();
    }

    // UTILS ===============

    //SERVER ONLY
    public bool PhaseCheck(uint playerID)
    {
        if (!temporaryPhaseList.Contains(playerID))
        {
            temporaryPhaseList.Add(playerID);
        }

        if (temporaryPhaseList.Count == TCGConstants.MAX_PLAYERS_CARD_GAME)
        {
            temporaryPhaseList = new List<uint>();
            return true;
        }

        return false;
    }

    //SERVER ONLY
    public uint GetOponentID(uint playerID)
    {
        uint oponentID = 666;
        foreach (KeyValuePair<uint, List<CardBehaviour>> current in fieldByPlayer)
        {
            if (current.Key != playerID)
            {
                oponentID = current.Key;
                break;
            }
        }
        return oponentID;
    }

    public void CheckIsGameOver(uint playerID)
    {
        isGameOver = false;
        uint targetID = playerID;
        uint oponentID = GetOponentID(playerID);

        if (decksByPlayer[playerID].Count <= 0)
        {
            print("OUT OF DECK");
            isGameOver = true;
            targetID = GetOponentID(playerID);
        }

        if (decksByPlayer[oponentID].Count <= 0)
        {
            print("OUT OF DECK 2");
            isGameOver = true;
            targetID = GetOponentID(oponentID);
        }

        if (lifeByPlayer[playerID] == 0)
        {
            print("NO LIFE");
            isGameOver = true;
            targetID = oponentID;
        }

        if (lifeByPlayer[oponentID] == 0)
        {
            print("NO LIFE 2");
            isGameOver = true;
            targetID = playerID;
        }

        if(isGameOver)
        {
            print("GAME OVER!");
            foreach (uint current in connectedPlayers)
            {
                if (current != playerID)
                {
                    victoriousPlayer = current;
                }
                else
                {
                    defeatedPlayer = current;
                }
            }
        }
    }


    public uint playerTurn {
    	get {
    		return TCGGameManager.Instance.playerTurn;
    	}
    	set{
    		TCGGameManager.Instance.playerTurn = value;
    	}
    }

    public int currentTurn{
    	get {
    		return TCGGameManager.Instance.currentTurn;
    	}
    	set{
    		TCGGameManager.Instance.currentTurn = value;
    	}
    }
	public bool isGameOver{
    	get {
    		return TCGGameManager.Instance.isGameOver;
    	}
    	set{
    		TCGGameManager.Instance.isGameOver = value;
    	}
    }

	public uint victoriousPlayer{
    	get {
    		return TCGGameManager.Instance.victoriousPlayer;
    	}
    	set{
    		TCGGameManager.Instance.victoriousPlayer = value;
    	}
    }
	public uint defeatedPlayer{
    	get {
    		return TCGGameManager.Instance.defeatedPlayer;
    	}
    	set{
    		TCGGameManager.Instance.defeatedPlayer = value;
    	}
    }
}