using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;
using TMPro;

public delegate void AnimationComplete();

public class TCGAnimations
{
	public static Sequence ShuffleDeck(List<CardBehaviour> currentPlayer, AnimationComplete animationComplete = null)
	{
		Sequence sequence = DOTween.Sequence();
		//TODO:
		return sequence;
	}

	public static Sequence AttackCard(Transform playerCard, Transform target, AnimationComplete onDamage = null, AnimationComplete animationComplete = null)
	{
		Vector3 originalPosition = playerCard.position;

		Sequence sequence = DOTween.Sequence();

		sequence.Append(playerCard.DOMove(target.position, 0.5f).OnComplete(
			()=>{
				onDamage();
			}
		));
		sequence.Append(playerCard.DOMove(originalPosition, 0.5f).OnComplete(
			()=>{
				animationComplete();
			}
		));

		//TODO:
		return sequence;
	}

	public static Sequence AttackLife(Transform playerCard, Transform targetPlayer, AnimationComplete onDamage = null, AnimationComplete animationComplete = null)
	{
		Sequence sequence = DOTween.Sequence();
		//TODO:
		return sequence;
	}

	public static Sequence ReturnToHand(List<CardBehaviour> cardsToAnimate, AnimationComplete animationComplete = null)
	{
		Sequence sequence = DOTween.Sequence();
		//TODO:
		return sequence;
	}

	public static Sequence DrawCard(CardBehaviour playerCardVisual, AnimationComplete animationComplete = null)
	{
		Sequence sequence = DOTween.Sequence();
		//TODO:
		return sequence;
		// Transform playerHand 
		// Transform deckPlace
	}

	public static Sequence DrawCard(List<CardBehaviour> cardsToAnimate, AnimationComplete animationComplete = null)
	{
		Sequence sequence = DOTween.Sequence();
		//TODO:
		return sequence;
	}

	public static Sequence DestroyCard(CardBehaviour card, AnimationComplete animationComplete = null)
	{
		Sequence sequence = DOTween.Sequence();
		//TODO:
		return sequence;
	}

	public static Sequence PutOnEvidenceForSeconds(CardBehaviour card, float seconds, AnimationComplete CastSpell = null, AnimationComplete animationComplete = null)
	{
		Sequence sequence = DOTween.Sequence();
		//TODO:
		return sequence;
	}

	public static Sequence ReturnCastToHand(CardBehaviour card, AnimationComplete animationComplete = null)
	{
		Sequence sequence = DOTween.Sequence();
		//TODO:
		return sequence;
	}

	public static Sequence SendToGraveyard(CardBehaviour card, AnimationComplete animationComplete = null)
	{
		Sequence sequence = DOTween.Sequence();
		//TODO:
		// Transform ownerGraveyard,
		return sequence;
	}

	public static Sequence FloatingText(GameObject prefab, Transform targetPosition, string value)
	{
		//TODO: FIX THIS
		GameObject go = GameObject.Instantiate(prefab, targetPosition.position, Quaternion.identity) as GameObject;
		go.GetComponent<TMP_Text>().text = value;

		Sequence sequence = DOTween.Sequence();

		Vector3 offset = new Vector3(0,0,-3f);
		sequence.Append(go.transform.DOMove(go.transform.position - offset, 0.5f).OnComplete(
			()=>{
				// GameObject.Destroy(go);
			}
		));

		return sequence;
	}
}