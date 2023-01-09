using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Newtonsoft.Json;
using System.Threading.Tasks;
using DG.Tweening;
using System.Linq;

[System.Serializable]
public class Token {
    public int damage;
    public int hit_points;
    public int effect_id;
}

public class EffectServer : MonoBehaviour
{

	public void SummonToken(uint playerID, int effect_id, string jsonToken)
	{
		// TCGGameManager.Instance.actionChain.AddToChain(
		// "CMDSummonToken",
		// 	(int actionID)=>{
		// 		List<Token> token = JsonConvert.DeserializeObject<List<Token>>(jsonToken);
		// 		List<uint> cards = new List<uint>();

		// 		NetworkIdentity player = NetworkServer.spawned[playerID];
		// 		EffectBase effect = EffectFactory.Instance.allEffects[effect_id];

		// 		int num_tokens = effect.create_tokens;
		// 		int slotsLeft = Constants.MAX_CARDS_PER_FIELD - TCGGameManager.Instance.gameServer.fieldByPlayer[playerID].Count;
		// 		int token_count = 0;

		// 		if(num_tokens > 0 && slotsLeft >= num_tokens)
		// 		{
		// 			token_count = num_tokens;
		// 		}else if (num_tokens > 0 && slotsLeft < num_tokens)
		// 		{
		// 			token_count = slotsLeft;
		// 		}

		// 		for (int i = 0; i < token_count; i ++)
		// 		{
		// 			//criar o objeto CardBehaviour com a info da token
		// 			GameObject card = Instantiate(TCGGameManager.Instance.cardPrefab, new Vector2(0, 0), Quaternion.identity);
		// 			CardBehaviour cardBehaviour = card.GetComponent<CardBehaviour>();

		// 			cardBehaviour.ownerID = playerID;

		// 			NetworkServer.Spawn(card, player.connectionToClient);

		// 			cardBehaviour.InitializeToken(token[0]);

		// 			if(TCGGameManager.Instance.gameServer.cardsPlayedByPlayerAndTurn[playerID].ContainsKey(TCGGameManager.Instance.currentTurn))
		// 			{
		// 				TCGGameManager.Instance.gameServer.cardsPlayedByPlayerAndTurn[playerID][TCGGameManager.Instance.currentTurn].Add(cardBehaviour.netId);
		// 			}else{
		// 				Debug.LogError("FIODEL");
		// 			}

		// 			TCGGameManager.Instance.gameServer.fieldByPlayer[playerID].Add(cardBehaviour);

		// 			cardBehaviour.isOnField = true;
		// 			cardBehaviour.is_creature = true;
		// 			cardBehaviour.isAttackEnabled = false;
		// 			cardBehaviour.isToken = true;

		// 			cards.Add(cardBehaviour.netId);
		// 		}

		// 		string jsonEffect = JsonConvert.SerializeObject(effect);
		// 		string jsonCards = JsonConvert.SerializeObject(cards);

		// 		// Debug.Log(jsonCards);
		// 		// Debug.Log(jsonEffect);

		// 		SetBuffs(playerID, jsonCards, jsonEffect);

		// 		DelayedSummonToken(playerID, jsonToken, jsonCards, actionID);
		// 	}
		// );

		// if(!TCGGameManager.Instance.actionChain.isChainBusy)
		// {
		// 	TCGGameManager.Instance.actionChain.ResolveChain();
		// }
	}

	private async void DelayedSummonToken(uint playerID, string jsonToken, string jsonCards, int actionID)
    {
        await new WaitForSeconds(0.5f);
        EffectResolver.Instance.RPCSummonToken(playerID, jsonToken, jsonCards, actionID);
    }
}