using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Newtonsoft.Json;

public delegate void OnEffectApplied ();
public delegate void OnCancelCast ();

public class EffectBase : MonoBehaviour
{
    public EffectData data;

    public virtual void OnCast(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) { }
    public virtual void OnDie(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) { }
    public virtual void OnStartTurn(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) { }
    public virtual void OnEndTurn(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) { }
    public virtual void OnStartTurnGraveyard(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) { }
    public virtual void OnCreatureDeath(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) { }
    public virtual void OnDiscard(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) { }
    public virtual void OnDraw(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) { }
    public virtual void OnSummon(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) { }
    public virtual void OnDamage(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) { }
    public virtual void OnHeal(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) { }

    public void SetEffect(EffectData effectData)
    {
        data = effectData;
    }


    public async void DelayedEffectResolver(uint playerID, string json, int actionID)
    {
        await new WaitForSeconds(1.5f);

        List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

        foreach (uint current in cards)
        {
            if(NetworkClient.spawned.ContainsKey(current))
            {
                CardBehaviour card = NetworkClient.spawned[current].GetComponent<CardBehaviour>();
                card.display.SetIsPlayable(false);

                if (card.isDead)
                {
                    // TODO: CLEAR STATS IF NEEDED
                    // ParticleManager.Instance.RemoveSleepParticle(card.netId);

                    // Sequence die = AnimationManager.AnimateCardDeath(
                    //     card, 
                    //     () => 
                    //     {
                    //         OnCardDeath(playerID, card);
                    //     }
                    // );
                    // die.Play();
                }else{
                    card.UpdateData();
                }
            }
        }

        // if (TCGGameManager.Instance.IsMyTurn())
        // {
        //     // TODO:
        //     // NetworkClient.localPlayer.GetComponent<TCGPlayerManager>().CMDUpdateFieldPlayability(false);
        // }

        ActionChain.Instance.CMDCompleteAction(NetworkClient.localPlayer.netId, actionID);
    }

    public async void DelayedDamageResolver(uint playerID, int actionID, uint targetID, int amount, string source)
    {
        CardBehaviour target = NetworkClient.spawned[targetID].GetComponent<CardBehaviour>();

        switch(source)
        {
            case("damage"):
                Debug.Log("damage");
                // Instantiate(ParticleManager.Instance.damageAimParticle, target.transform);
                if(amount!=0)
                {
                    FloatingText.Instance.AnimateFloatingText(
                        Camera.main.WorldToScreenPoint(target.transform.position), 
                        amount);
                }
            break;

            case("heal"):
                Debug.Log("heal");
                // Instantiate(ParticleManager.Instance.healingParticle1, target.transform);
                if(amount!=0)
                {
                    FloatingText.Instance.AnimateFloatingText(
                        Camera.main.WorldToScreenPoint(target.transform.position), 
                        amount);    
                }
            break;
        }
    
        await new WaitForSeconds(1f);

        target.UpdateData();
        target.display.SetIsPlayable(false);
    }
}
