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
    public Dictionary<uint, Dictionary<int, List<uint>>> cardsPlayedByPlayerAndTurn = new Dictionary<uint, Dictionary<int, List<uint>>>();

    public void OnPlayerConnected(uint playerID)
    {
    	if (!connectedPlayers.Contains(playerID))
        {
            connectedPlayers.Add(playerID);

            decksByPlayer.Add(playerID, new List<CardBehaviour>());

            fieldByPlayer.Add(playerID, new List<CardBehaviour>());
            graveyardByPlayer.Add(playerID, new List<CardBehaviour>());
            lifeByPlayer.Add(playerID, TCGConstants.PLAYER_INITIAL_LIFE);
            cardsPlayedByPlayerAndTurn.Add(playerID, new Dictionary<int, List<uint>>());
            cardsPlayedByPlayerAndTurn[playerID].Add(currentTurn, new List<uint>());

            if (connectedPlayers.Count == TCGConstants.MAX_PLAYERS_CARD_GAME)
            {
                //TODO: RANDOMIZE FIRST PLAYER
                playerTurn = connectedPlayers[0];

                // int diceRoll = Random.Range(1, 7);

                // Dictionary<uint, int> diceRollByPlayer = new Dictionary<uint, int>();
                // foreach (KeyValuePair<uint, int> current in diceRollByPlayer)
                // {
                //     if (current.Key != playerID)
                //     {
                //         while (current.Value == diceRoll)
                //         {
                //             diceRoll = Random.Range(1, 7);
                //         }
                //     }
                // }

                // diceRollByPlayer.Add(playerID, diceRoll);
                // diceRollByPlayer = diceRollByPlayer.OrderByDescending(key => key.Value).ToDictionary(key => key.Key, key => key.Value);
                // playerTurn = diceRollByPlayer.First().Key;

                EffectFactory.Instance.InitializeFactory();
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

            for (int i = 0; i < TCGConstants.MAX_CARDS_PER_DECK; i++)
            {
                GameObject card = Instantiate(TCGGameManager.Instance.cardPrefab, new Vector3(-8000, 0.1f, 0), Quaternion.identity);
                CardBehaviour cardBehaviour = card.GetComponent<CardBehaviour>();
                
                decksByPlayer[currentPlayer].Add(cardBehaviour);
                NetworkServer.Spawn(card, player.connectionToClient);

                cardBehaviour.cardID = Random.Range(0,100);
                // cardBehaviour.cardID = i;
                cardBehaviour.effects = "heal01";
                cardBehaviour.ownerID = currentPlayer;

                //TODO: remove this after card data is inserted correctly
                cardBehaviour.is_creature = true;
                cardBehaviour.attack = Random.Range(1,6);
                cardBehaviour.hit_points = Random.Range(1,6);
                cardBehaviour.originalHitpoints = cardBehaviour.hit_points;
                cardBehaviour.originalAttack = cardBehaviour.attack;
                
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


    public void ResolveCombat(uint playerID, uint attackerID, uint targetID)
    {
        if(!NetworkServer.spawned.ContainsKey(attackerID) && !NetworkServer.spawned.ContainsKey(targetID))
        {
            return;
        }

        CardBehaviour attacker_card = NetworkServer.spawned[attackerID].GetComponent<CardBehaviour>();
        CardBehaviour target_card = null;
        // PlayerLifeHitbox playerLife = null;
        bool isLife =  false;
        bool shouldSkip = false;

        NetworkServer.spawned[targetID].TryGetComponent(out target_card);

        // isLife = NetworkServer.spawned[targetID].TryGetComponent(out playerLife);
        // if(!isLife)
        // {
            

            // if(target != null)
            // {
            //     if(!shouldSkip)
            //     {
            //         shouldSkip = target.isDead;
            //     }
            // }
        // }


        if(attacker_card.double_hit && !cardsAttackedThisTurn.Contains(attackerID))
        {
            attacker_card.isAttackEnabled = true;
        }
        else if(cardsAttackedThisTurn.Contains(attackerID) && attacker_card.double_hit)
        {
            attacker_card.isAttackEnabled = false;
        }

        if(!attacker_card.double_hit)
        {
            attacker_card.isAttackEnabled = false;
        }

        if(!cardsAttackedThisTurn.Contains(attackerID))
        {
            cardsAttackedThisTurn.Add(attackerID);
        }

        if(!attacker_card.isDead && !target_card.isDead)
        {
            ActionChain.Instance.AddToChain(
                "CMDCombat",
                (int actionID)=>
                {
                    CardBehaviour attacker = attacker_card;
                    CardBehaviour target = target_card;
                    
                    bool shouldTrample = false;

                    // if(attacker.double_hit && !cardsAttackedThisTurn.Contains(attackerID))
                    // {
                    //     attacker.isAttackEnabled = true;
                    // }
                    // else if(cardsAttackedThisTurn.Contains(attackerID) && attacker.double_hit)
                    // {
                    //     attacker.isAttackEnabled = false;
                    // }

                    // if(!attacker.double_hit)
                    // {
                    //     attacker.isAttackEnabled = false;
                    // }

                    // if(!cardsAttackedThisTurn.Contains(attackerID))
                    // {
                    //     cardsAttackedThisTurn.Add(attackerID);
                    // }

                    uint oponentID = GetOponentID(playerID);

                    if (NetworkServer.spawned[targetID].TryGetComponent(out target))
                    {
                        int attackerAttack = attacker.attack;
                        int targetHitpoints = target.hit_points;
                        
                        if(target.TakeDamage(attacker.attack) == true)
                        {
                            if (attacker.death_touch && !target.barrier || target.isDead)
                            {
                                UniversalDeathResolver(target);
                            }
                        }

                        if(attacker.TakeDamage(target.attack) == true)
                        {
                            if(target.death_touch && !attacker.barrier || attacker.isDead)
                            {
                                UniversalDeathResolver(attacker);
                            }
                        }

                        if (!attacker.isDead)
                        {
                            if (attacker.trample && attackerAttack > targetHitpoints)
                            {
                                shouldTrample = true;
                                // lifeByPlayer[oponentID] -= 1;
                                // lifeHitboxByPlayer[oponentID].currentLife = lifeByPlayer[oponentID];

                                // CheckIsGameOver(playerID);
                            }
                        }

                        if (!shouldTrample)
                        {
                            TCGGameManager.Instance.RPCResolveCombat(playerID, actionID, attackerID, targetID);
                        }
                        else
                        {
                            // RPCResolveTrampleCombat(playerID, actionID, attackerID, targetID, lifeHitboxByPlayer[oponentID].netId);
                        }
                    }
                    // else if (NetworkServer.spawned[targetID].TryGetComponent(out playerLife))
                    // {
                    //     lifeByPlayer[oponentID] -= 1;

                    //     lifeHitboxByPlayer[oponentID].currentLife = lifeByPlayer[oponentID];

                    //     CheckIsGameOver(playerID);

                    //     RPCResolveDirectHit(lifeHitboxByPlayer[oponentID].netId, attackerID, actionID);
                    // }
                }
            );

            if(!ActionChain.Instance.isChainBusy)
            {
                ActionChain.Instance.ResolveChain();
            }
        }
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

        cardsPlayedByPlayerAndTurn[playerTurn][currentTurn] = new List<uint>();

        TCGGameManager.Instance.RPCPlayerTurn();
    }

    public void OnEndTurn(uint playerID)
    {
    	ActionChain.Instance.ResetTurnInactivity();
        
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

        if(!cardsPlayedByPlayerAndTurn[playerTurn].ContainsKey(currentTurn))
        {
            cardsPlayedByPlayerAndTurn[playerTurn][currentTurn] = new List<uint>();
        }

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

            //TODO: DISCARD EXTRA
            // TCGGameManager.Instance.RPCSendCardToGraveyard(jsonDiscard);
            
            TCGGameManager.Instance.RPCDrawCard(jsonDraw, isTurnStart);
        }
        else
        {
            // ServerGameOver(playerID);
        }
    }

    public void CastCard(uint playerID, uint cardNetID)
    {
        ActionChain.Instance.AddToChain(
            "CastCard",
            (int actionID)=>{
                NetworkIdentity netiden = NetworkServer.spawned[cardNetID];
                CardBehaviour card = netiden.GetComponent<CardBehaviour>();

                cardsPlayedByPlayerAndTurn[playerTurn][currentTurn].Add(cardNetID);

                if(card.is_creature)
                {
                    handsByPlayer[playerID].Remove(card);
                    fieldByPlayer[playerID].Add(card);
                    card.isOnField = true;
                    card.isDead = false;
                    card.isAttackEnabled = false;
                    card.isAttackEnabled = card.haste;

                    DelayedCardSummon(playerID, actionID, cardNetID);
                }else{
                    handsByPlayer[playerID].Remove(card);
                    graveyardByPlayer[playerID].Add(card);

                    TCGGameManager.Instance.RPCCastCard(playerID, actionID, cardNetID);
                }
            }
        );

        if(!ActionChain.Instance.isChainBusy)
        {
            ActionChain.Instance.ResolveChain();
        }
    }

    private async void DelayedCardSummon(uint playerID, int actionID, uint cardNetID)
    {
        await new WaitForSeconds(0.1f);
        TCGGameManager.Instance.RPCSummonCard(playerID, actionID, cardNetID);
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

    //SERVER ONLY
    public void UniversalDeathResolver(CardBehaviour target)
    {
        Debug.Log("UniversalDeathResolver");
        uint ownerID = target.ownerID;
        uint oponentID = GetOponentID(ownerID);

        target.isDead = true;
        target.isOnField = false;

        fieldByPlayer[ownerID].Remove(target);

        if(!target.isToken)
        {
            graveyardByPlayer[ownerID].Add(target);
        }
    }

    public void CreateBot()
    {
        GameObject bot = Instantiate(TCGGameManager.Instance.botPrefab);
        TCGBot tcgBot = bot.GetComponent<TCGBot>();

        NetworkServer.Spawn(bot);
    }

    public void CheckIsGameOver(uint playerID)
    {
        isGameOver = false;
        uint targetID = playerID;
        uint oponentID = GetOponentID(playerID);
        if(oponentID == 666)
            return;

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