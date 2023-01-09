using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCGArena : Singleton<TCGArena>
{
    public Transform playerField;
    public Transform oponentField;
    public Transform playerDeck;
    public Transform oponentDeck;
    public Transform playerHand;
    public Transform oponentHand;
    public Transform playerGraveyard;
    public Transform oponentGraveyard;

    public List<CardBehaviour> GetPlayerField() { return GetChildrenCards(playerField); }
    public List<CardBehaviour> GetPlayerHand() { return GetChildrenCards(playerHand); }
    public List<CardBehaviour> GetPlayerGraveyard() { return GetChildrenCards(playerGraveyard); }
    public List<CardBehaviour> GetPlayerDeck() { return GetChildrenCards(playerDeck); }

    public List<CardBehaviour> GetOponentField() { return GetChildrenCards(oponentField); }
    public List<CardBehaviour> GetOponentHand() { return GetChildrenCards(oponentHand); }
    public List<CardBehaviour> GetOponentGraveyard() { return GetChildrenCards(oponentGraveyard); }
    public List<CardBehaviour> GetOponentDeck() { return GetChildrenCards(oponentDeck); }

    private List<CardBehaviour> GetChildrenCards(Transform target)
    {
        List<CardBehaviour> cards = new List<CardBehaviour>();

        for(int i = 0; i < target.childCount; i++)
        {
            CardBehaviour current = target.GetChild(i).GetComponent<CardBehaviour>();
            cards.Add(current);
        }
        return cards;
    }
}
