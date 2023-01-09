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

	public void OnGameStarted(string json)
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
                card.display.ToggleSleeve(false);
                card.isInteractable = true;
            }
            else
            {
                card.transform.SetParent(TCGArena.Instance.oponentHand);
            }
        }
	}

	public void PlayerTurn()
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