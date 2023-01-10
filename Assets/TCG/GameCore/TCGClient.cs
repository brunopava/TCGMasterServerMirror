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

	public void OnDeckSpawnComplete(string json)
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

        // card.SetCardInfo();
        card.display.ToggleSleeve();
        card.isInteractable = false;
        card.isAttackEnabled = false;
        // card.isAttackEnabled = card.haste;

        card.transform.SetParent(null);
        card.transform.rotation = Quaternion.Euler(Vector3.zero);

        if (!card.hasAuthority)
        {
            
            card.transform.SetParent(TCGArena.Instance.oponentField);

            
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
            card.transform.SetParent(TCGArena.Instance.playerField);
        }  

        ActionChain.Instance.CMDCompleteAction(NetworkClient.localPlayer.netId, actionID);
    }

    public void SummonCard(uint playerID, int actionID, uint cardNetID)
    {
        NetworkIdentity netiden = NetworkClient.spawned[cardNetID];
        CardBehaviour card = netiden.GetComponent<CardBehaviour>();
        
        card.transform.rotation = Quaternion.Euler(Vector3.zero);
        
        if (card.hasAuthority)
        {
            card.transform.SetParent(TCGArena.Instance.playerField);
            // card.SetCardInfo();
            card.isInteractable = true;
            // AudioManager.Instance.PlayDropCardSound();

            // List<CardBehaviour> arena = UIGameArena.Instance.GetPlayerField();
            // foreach(CardBehaviour currentCard in arena)
            // {
            //     if(card != currentCard)
            //     {
            //         foreach(int effect_id in currentCard.card_effects)
            //         {
            //             if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
            //             {
            //                 CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];
                            
            //                 if(effect.on_summon && !effect.observe_oponent)
            //                 {
            //                     effect.OnSummon(
            //                         currentCard,
            //                         () => {
            //                             Debug.Log("Casted OnSummon: " + effect.effect_name);
            //                         }
            //                     );
            //                 }
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

            //             if(effect.on_summon && effect.observe_oponent)
            //             {
            //                 effect.OnSummon(
            //                     currentCard,
            //                     () => {
            //                         Debug.Log("Casted OnSummon: " + effect.effect_name);
            //                     }
            //                 );
            //             }
            //         }
            //     }
            // }
        }
        else
        {
            card.transform.SetParent(TCGArena.Instance.oponentField);
            // card.SetCardInfo();            
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

        // influenceManager.CMDEnableInfluence(NetworkClient.localPlayer.netId); 
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