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
    
    public float cardSpawnDelay = 0.1f;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    [Command(requiresAuthority = false)]
    public void CMDPlayerConnected(uint playerID)
    {
        Debug.Log("CMDPlayerConnected: " + playerID.ToString());
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
                GameObject card = Instantiate(cardPrefab, new Vector3(-8000, 0.1f, 0), Quaternion.identity);
                CardBehaviour cardBehaviour = card.GetComponent<CardBehaviour>();
                
                decksByPlayer[currentPlayer].Add(cardBehaviour);
                NetworkServer.Spawn(card, player.connectionToClient);
                
                await new WaitForSeconds(cardSpawnDelay);
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
        RPCAllPlayersConnected(json);
    }

    [ClientRpc]
    private void RPCAllPlayersConnected(string json)
    {
        Dictionary<uint, List<uint>> decks = JsonConvert.DeserializeObject<Dictionary<uint, List<uint>>>(json);

        List<CardBehaviour> playerCards = new List<CardBehaviour>();
        List<CardBehaviour> oponentCards = new List<CardBehaviour>();

        foreach (KeyValuePair<uint, List<uint>> currentDeck in decks)
        {
            foreach (uint current in currentDeck.Value)
            {
                if (NetworkClient.spawned.ContainsKey(current))
                {
                    NetworkIdentity cardIdentity = NetworkClient.spawned[current];
                    CardBehaviour card = cardIdentity.GetComponent<CardBehaviour>();

                    if (card != null)
                    {
                        if (card.hasAuthority)
                        {
                            card.transform.SetParent(TCGArena.Instance.playerDeck);
                            playerCards.Add(card);
                        }
                        else
                        {
                            card.transform.SetParent(TCGArena.Instance.oponentDeck);
                            oponentCards.Add(card);
                        }
                    }
                }
            }
        }

        UIManager.Instance.OnGameStarted();

        CMDDrawInitialCards(NetworkClient.localPlayer.netId);
    }

    [Command(requiresAuthority = false)]
    public void CMDDrawInitialCards(uint playerID)
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
        RPCDrawInitialCards(json);
    }

    [ClientRpc]
    private void RPCDrawInitialCards(string json)
    {
        List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

        //TODO: ANIMATE CARDS
        // List<CardBehaviour> playerCardsToAnimate = new List<CardBehaviour>();
        // List<CardBehaviour> oponentCardsToAnimate = new List<CardBehaviour>();

        foreach (uint current in cards)
        {
            NetworkIdentity netiden = NetworkClient.spawned[current];
            CardBehaviour card = netiden.GetComponent<CardBehaviour>();

            if (card.hasAuthority)
            {
                card.transform.SetParent(TCGArena.Instance.playerHand);
                card.display.ToggleSleeve(false);
                card.isInteractable = true;
            }
            else
            {
                card.transform.SetParent(TCGArena.Instance.oponentHand);
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CMDStartMatch(uint playerID)
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
        
        RPCPlayerTurn();
    }

    [ClientRpc]
    private void RPCPlayerTurn()
    {
        if (IsMyTurn())
        {
            NetworkIdentity player = NetworkClient.spawned[playerTurn];
            TCGPlayerManager manager = player.GetComponent<TCGPlayerManager>();
            manager.OnStartTurn();
        }
        else
        {
            // UIGameArena.Instance.DisableEndTurnButton();
            // UIGameArena.Instance.DisableResourcesButtons();
        }
    }

    //LOGICAL END OF TURN, PASS THE ACTION TO THE OTHER PLAYER AND INCREASE TURN COUNTER
    [Command(requiresAuthority = false)]
    public void CMDEndTurn(uint playerID)
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

    //LOGICAL DRAW OF CARDS
    [Command(requiresAuthority = false)]
    public void CMDDrawCard(uint playerID, int drawAmount, bool isTurnStart, bool isOponent)
    {
        uint targetID = playerID;
        if(isOponent)
        {
            targetID = TCGGameManager.Instance.GetOponentID(playerID);
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

    //DELIVER CARD TO HAND IN BOTH CLIENTS
    [ClientRpc]
    public void RPCDrawCard(string json, bool isTurnStart)
    {
        List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

        List<CardBehaviour> playerCardsToAnimate = new List<CardBehaviour>();
        List<CardBehaviour> oponentCardsToAnimate = new List<CardBehaviour>();

        foreach (uint current in cards)
        {
            NetworkIdentity netiden = NetworkClient.spawned[current];
            CardBehaviour card = netiden.GetComponent<CardBehaviour>();

            if (card.hasAuthority)
            {
                card.transform.SetParent(TCGArena.Instance.playerHand);
                card.isInteractable = true;
            }
            else
            {
                card.transform.SetParent(TCGArena.Instance.oponentHand);
            }
        }
    }

    //SERVER ONLY
    private async void DelayedTurnPass()
    {
        await new WaitForSeconds(0.5f);
        RPCPlayerTurn();
    }

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

    //CLIENT ONLY
    public bool IsMyTurn()
    {
        return NetworkClient.localPlayer.netId == playerTurn;
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
}
