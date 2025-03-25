using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Mirror;
using DG.Tweening;

public class TCGClient:MonoBehaviour
{
	public Transform gameArena;

    public Dictionary<int, Texture> cardsSprites;
    public void LoadCards()
    {
        cardsSprites = new Dictionary<int, Texture>();

        Texture[] sprites = Resources.LoadAll<Texture>("Images/cards");

        for(int i = 0; i < sprites.Length; i++)
        {
            cardsSprites.Add(i, sprites[i]);
        }
        // foreach(KeyValuePair<int, Texture> current in cardsSprites)
        // {
        //     Debug.Log(current.Value.name);
        //     // string identifier = current.name.Split('_')[0];
        //     // int id = 0;
        //     // if(int.TryParse(identifier, out id))
        //     // {
                
        //     // }
        // }
    }

	public async void OnDeckSpawnComplete(string json)
	{
        LoadCards();

        await new WaitForSeconds(0.5f);

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
                    card.UpdateData();

                    // Debug.Log(card.cardID);
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

                    // because cards are outside folders in resources
                    if(cardsSprites.ContainsKey(card.cardID))
                    {
                        card.display.SetCardImage(cardsSprites[card.cardID]); 
                    }
                }
            }
        }

        await new WaitForSeconds(0.5f);

        UIManager.Instance.OnGameStarted();

        await new WaitForSeconds(0.5f);
        TCGGameManager.Instance.CMDDrawInitialCards(NetworkClient.localPlayer.netId);
        if(TCGGameManager.Instance.isBotGame)
        {
            uint botID = TCGGameManager.Instance.gameServer.GetOponentID(NetworkClient.localPlayer.netId);
            TCGGameManager.Instance.CMDDrawInitialCards(botID);
        }
	}

	public void DrawInitialCards(string json)
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
                card.display.ToggleSleeve();
                card.isInteractable = true;
            }
            else
            {
                card.transform.SetParent(TCGArena.Instance.oponentHand);
            }
        }

        TCGGameManager.Instance.CMDStartMatch(NetworkClient.localPlayer.netId);
	}

	public void PlayerTurn()
	{
        string label = "Player: {0} Turn: {1}";
        UIManager.Instance.debugTurn.text = string.Format(label, playerTurn.ToString(), currentTurn.ToString());
        UIManager.Instance.SetEndTurnButtonEnable(IsMyTurn());

		if (IsMyTurn())
        {
            NetworkIdentity player = NetworkClient.spawned[playerTurn];
            TCGPlayerManager manager = player.GetComponent<TCGPlayerManager>();
            manager.OnStartTurn();
        }
	}

	public void DrawCard(string json, bool isTurnStart)
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
                card.display.ToggleSleeve();
            }
            else
            {
                card.transform.SetParent(TCGArena.Instance.oponentHand);
            }
        }
	}

    public void CastCard(uint playerID, int actionID, uint cardNetID)
    {
        NetworkIdentity netiden = NetworkClient.spawned[cardNetID];
        CardBehaviour card = netiden.GetComponent<CardBehaviour>();

        card.display.ToggleSleeve();
        card.isInteractable = false;

        card.transform.SetParent(null);
        card.transform.rotation = Quaternion.Euler(Vector3.zero);

        if (!card.hasAuthority)
        {
            
            //TODO:
            // card.transform.SetParent(TCGArena.Instance.oponentField);

            
            Sequence cast = TCGAnimations.PutOnEvidenceForSeconds(
                card, 
                2f, 
                ()=>{
                    //evidence comlete
                },
                ()=>{
                    //cast complete
                }
            );
            cast.Play();
        }else{
            //TODO:
            // card.transform.SetParent(TCGArena.Instance.playerField);
        }  

        ActionChain.Instance.CMDCompleteAction(NetworkClient.localPlayer.netId, actionID);
    }

    public void SummonCard(uint playerID, int actionID, uint cardNetID)
    {
        NetworkIdentity netiden = NetworkClient.spawned[cardNetID];
        CardBehaviour card = netiden.GetComponent<CardBehaviour>();
        
        card.transform.rotation = Quaternion.Euler(Vector3.zero);
        card.UpdateData();
        card.display.ToggleSleeve();

        if (card.hasAuthority)
        {
            card.transform.SetParent(TCGArena.Instance.playerField);
            card.isInteractable = true;
            // AudioManager.Instance.PlayDropCardSound();

            foreach(string effect_id in card.card_effects)
            {
                if (EffectFactory.Instance.effectPool.ContainsKey(effect_id))
                {
                    EffectBase effect = EffectFactory.Instance.effectPool[effect_id];
                    
                    if(effect.data.on_summon && !effect.data.observe_oponent)
                    {
                        Debug.Log("Casted OnSummon: " + effect.data.effect_name);
                        effect.OnSummon(
                            card,
                            () => {
                                Debug.Log("Casted OnSummon: " + effect.data.effect_name);
                            }
                        );
                    }
                }
            }
        }
        else
        {
            card.transform.SetParent(TCGArena.Instance.oponentField);          
        }

        // if (NetworkClient.localPlayer.netId == playerID)
        // {
        //     if(card.is_choice)
        //     {
        //         if(card.hasAuthority)
        //         {
        //             card.OpenPopupEffectChoice(
        //                 (int effect_selected)=>{

        //                     card.chosen_effect_id = effect_selected;

        //                     if (EffectFactory.Instance.allEffects.ContainsKey(card.chosen_effect_id))
        //                     {
        //                         CardEffectBase effect = EffectFactory.Instance.allEffects[card.chosen_effect_id];

        //                         if (effect.on_cast_effect)
        //                         {  
        //                             effect.OnCast(
        //                                 card,
        //                                 () =>
        //                                 {
        //                                     Debug.Log("Casted OnCast: " + effect.effect_name);
        //                                 }
        //                             );
        //                         }
        //                     }
        //                 }
        //             );
        //         }
        //     }else{
        //         foreach(int effect_id in card.card_effects)
        //         {
        //             if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
        //             {
        //                 CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

        //                 if (effect.on_cast_effect)
        //                 {  
        //                     effect.OnCast(
        //                         card,
        //                         () =>
        //                         {
        //                             Debug.Log("Casted OnCast: " + effect.effect_name);
        //                         }
        //                     );
        //                 }
        //             }
        //         }
        //     }

        //     for(int i = 0; i<UIGameArena.Instance.playerField.childCount; i++)
        //     {
        //         CardBehaviour current  = UIGameArena.Instance.playerField.GetChild(i).GetComponent<CardBehaviour>();

        //         foreach(int effect_id in current.card_effects)
        //         {
        //             if(EffectFactory.Instance.allEffects.ContainsKey(effect_id) && current.netId != cardNetID)
        //             {
        //                 CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];
                        
        //                 if(effect.on_cast_effect && effect.is_aura && effect is AuraPlusEffect)
        //                 {
        //                     AuraPlusEffect auraEffect = (effect as AuraPlusEffect);
        //                     List<ITarget> targets = new List<ITarget>();
        //                     targets.Add(card);

        //                     auraEffect.OnLateSummon(targets, 
        //                         ()=>{
        //                             Debug.Log("Casted OnLateSummon: " + auraEffect.effect_name);
        //                         }
        //                     );
        //                 }
        //             }
        //         }
        //     }
        // }

        ActionChain.Instance.CMDCompleteAction(NetworkClient.localPlayer.netId, actionID);
    }

    public void SendCardToGraveyard(string json)
    {
        List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

        foreach (uint current in cards)
        {
            NetworkIdentity netiden = NetworkClient.spawned[current];
            CardBehaviour card = netiden.GetComponent<CardBehaviour>();
            card.transform.SetParent(gameArena);
            card.display.ToggleSleeve();

            Sequence SendToGraveyard = TCGAnimations.SendToGraveyard(
                card, 
                () =>
                {
                    //EU DESCARTANDO
                    if (card.hasAuthority)
                    {
                        // card.transform.SetParent(UIGameArena.Instance.playerGraveyard);

                        // List<CardBehaviour> arena = UIGameArena.Instance.GetPlayerField();
                        // foreach(CardBehaviour currentCard in arena)
                        // {
                        //     foreach(int effect_id in currentCard.card_effects)
                        //     {
                        //         if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
                        //         {
                        //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

                        //             if(effect.on_discard && !effect.observe_oponent)
                        //             {
                        //                 effect.OnDiscard(
                        //                     currentCard,
                        //                     () => {
                        //                         Debug.Log("Casted OnDiscard: " + effect.effect_name);
                        //                     }
                        //                 );
                        //             }
                        //         }
                        //     }
                        // }

                        // List<CardBehaviour> oponentArena = UIGameArena.Instance.GetOponentField();
                        // foreach(CardBehaviour currentCard in oponentArena)
                        // {
                        //     foreach(int effect_id in currentCard.card_effects)
                        //     {
                        //         if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
                        //         {
                        //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

                        //             if(effect.on_discard && effect.observe_oponent)
                        //             {
                        //                 effect.OnDiscard(
                        //                     currentCard,
                        //                     () => {
                        //                         Debug.Log("Casted OnDiscard: " + effect.effect_name);
                        //                     }
                        //                 );
                        //             }
                        //         }
                        //     }
                        // }
                    }
                    //ELE DISCARTANDO
                    else
                    {
                        // card.transform.SetParent(UIGameArena.Instance.oponentGraveyard);
                    }
                }
            ); 

            SendToGraveyard.Play();
        }
    }

    public void ResolveCombat(uint playerID, int actionID, uint attackerID, uint targetID)
    {
        Debug.Log("ResolveCombat");

        CardBehaviour attacker = NetworkClient.spawned[attackerID].GetComponent<CardBehaviour>();
        CardBehaviour target = NetworkClient.spawned[targetID].GetComponent<CardBehaviour>();

        attacker.display.SetIsPlayable(false);

        int damage = attacker.attack * -1;
        int damageTaken = target.attack * -1;

        //TODO: ATTACK LIFE
        // Sequence attack = TCGAnimations.AttackLife(

        Sequence attack = TCGAnimations.AttackCard(
            attacker.display.transform, 
            target.display.transform,
            () =>
            {
                // if(damage > 0)
                // {
                //     TCGAnimations.FloatingText(
                //          target.display.hit_points.gameObject,
                //          target.display.hit_points.transform,
                //          damage.ToString());
                // }
                // if(damageTaken > 0)
                // {
                //     TCGAnimations.FloatingText(
                //          attacker.display.hit_points.gameObject,
                //          attacker.display.hit_points.transform,
                //          damageTaken.ToString());
                // }
            },
            () =>
            {
                DelayedAttackResolver(attacker, target, actionID);
            }
        );
        attack.Play();

        //TODO: DOUBLE HIT
        // if(attacker.hasAuthority && attacker.double_hit)
        // {
        //     attacker.cardVisual.SetIsPlayable(attacker.isAttackEnabled);
        // }
    }

    //Combat needs a delay to resolve some stuff and check if cards died
    //CLIENT
    private async void DelayedAttackResolver(CardBehaviour attacker, CardBehaviour target, int actionID)
    {
        await new WaitForSeconds(0.1f);

        uint playerID = NetworkClient.localPlayer.netId;

        attacker.UpdateData();
        target.UpdateData();

        attacker.display.SetIsPlayable(false);
        target.display.SetIsPlayable(false);

        await new WaitForSeconds(0.5f);

        List<uint> cards = new List<uint>();
        cards.Add(attacker.netId);
        cards.Add(target.netId);
        string json = JsonConvert.SerializeObject(cards);

        if (attacker.hasAuthority && attacker.isDead)
        {
            attacker.transform.SetParent(TCGArena.Instance.playerGraveyard);
        }
        else if(attacker.isDead)
        {
            attacker.transform.SetParent(TCGArena.Instance.oponentGraveyard);
        }

        if (target.hasAuthority && target.isDead)
        {
            target.transform.SetParent(TCGArena.Instance.playerGraveyard);
        }
        else if(target.isDead)
        {
            target.transform.SetParent(TCGArena.Instance.oponentGraveyard);
        }

        await new WaitForSeconds(0.1f);
        
        // TODO:
        // effectResolver.DelayedEffectResolver(playerID, json, actionID);

        ActionChain.Instance.CMDCompleteAction(playerID, actionID);
    }

	//CLIENT ONLY
    public bool IsMyTurn()
    {
        return NetworkClient.localPlayer.netId == playerTurn;
    }

    public uint playerTurn {
    	get {
    		return TCGGameManager.Instance.playerTurn;
    	}
    }

    public int currentTurn{
    	get {
    		return TCGGameManager.Instance.currentTurn;
    	}
    }
	public bool isGameOver{
    	get {
    		return TCGGameManager.Instance.isGameOver;
    	}
    }

	public uint victoriousPlayer{
    	get {
    		return TCGGameManager.Instance.victoriousPlayer;
    	}
    }
	public uint defeatedPlayer{
    	get {
    		return TCGGameManager.Instance.defeatedPlayer;
    	}
    }
}