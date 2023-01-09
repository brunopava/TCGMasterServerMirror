using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;

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
		Sequence sequence = DOTween.Sequence();
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
}