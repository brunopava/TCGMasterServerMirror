using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Newtonsoft.Json;

public class HealEffect : EffectBase
{
    public override void OnSummon(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) 
    { 
        if(data.on_summon)
        {
            ApplyEffect(source, onEffectApplied, onCancel);
        }
    }

    public override void OnCast(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) 
    {
    	if(data.on_cast)
        {
            ApplyEffect(source, onEffectApplied, onCancel);
        }
    }

    public override void OnEndTurn(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) 
    {
        if(data.on_end_turn)
        {
            ApplyEffect(source, onEffectApplied, onCancel);
        }
    }

    public override void OnStartTurn(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) 
    {
        if(data.on_start_turn)
        {
            ApplyEffect(source, onEffectApplied, onCancel);
        }
    }

    public override void OnDie(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) 
    {
        if(data.on_death)
        {
            ApplyEffect(source, onEffectApplied, onCancel);
        }
    }

    public override void OnCreatureDeath(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null) 
    {
        if(data.on_creature_death)
        {
            ApplyEffect(source, onEffectApplied, onCancel);
        }
    }

    private void ApplyEffect(ITarget source, OnEffectApplied onEffectApplied = null, OnCancelCast onCancel = null)
    {
        Debug.Log("ApplyEffect");
        if(data.target_number > 0)
        {
            TargetSystem.Instance.isOponentOnly = false;
            TargetSystem.Instance.BeginTargeting(
                source, 
                (ITarget source, List<ITarget> targets)=>
                {
                    List<uint> cards = new List<uint>();
                    foreach(ITarget current in targets)
                    {
                        MonoBehaviour currentTarget = (current as MonoBehaviour);
                        cards.Add(currentTarget.GetComponent<CardBehaviour>().netId);
                    }
 
                    EffectFactory.Instance.CMDCardHeal(NetworkClient.connection.identity.netId, JsonConvert.SerializeObject(cards), data.effect_name);

                    if(onEffectApplied != null)
                    {
                        Debug.Log("onEffectApplied");
                        onEffectApplied();
                    }
                },
                (string json)=>
                {
                    if(onCancel != null)
                    {
                        onCancel();
                    }
                },
                data.target_number
            );
        }
        if(data.is_aura_friendly)
        {
            List<CardBehaviour> arena = TCGArena.Instance.GetPlayerField();
            List<uint> cards = new List<uint>();
            foreach(CardBehaviour current in arena)
            {
                cards.Add(current.netId);
            }
            EffectFactory.Instance.CMDCardHeal(NetworkClient.connection.identity.netId, JsonConvert.SerializeObject(cards), data.effect_name);
        }
    }
}