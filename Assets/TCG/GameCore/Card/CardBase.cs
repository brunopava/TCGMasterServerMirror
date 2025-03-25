using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CardBase : NetworkBehaviour
{
	[SyncVar]
    public int cardID;
    [SyncVar]
    public uint ownerID;
    [SyncVar]
    public string effects;

    public List<string> card_effects{
        get {
            List<string> list = new List<string>();

            string[] splitEffects = effects.Split('_');
            foreach(string current in splitEffects)
            {
                if(!string.IsNullOrEmpty(current))
                {
                    list.Add(current);
                }
            }
            return list;
        }
    }

	[SyncVar]
    public bool is_creature;
    [SyncVar]
    public bool is_choice;
    [SyncVar]
    public int chosen_effect_id = 666;
    [SyncVar]
    public int cost;
    [SyncVar]
    public int cost_alteration;
    [SyncVar]
    public bool isDead = false;
    [SyncVar]
    public bool isOnField = false;
	[SyncVar]
    public bool tank = false;
    [SyncVar]
    public bool barrier = false;
    [SyncVar]
    public bool haste = false;
    [SyncVar]
    public bool trample = false;
    [SyncVar]
    public bool direct_hit = false;
    [SyncVar]
    public bool double_hit = false;
    [SyncVar]
    public bool death_touch = false;
    [SyncVar]
    public bool isToken = false;
    [SyncVar]
    public bool isAttackEnabled = false;
    
    [SyncVar]
    public int maxHp;
    [SyncVar]
    public int originalHitpoints;
    [SyncVar]
    public int originalAttack;
    [SyncVar]
    public int attack;
    [SyncVar]
    public int hit_points;

    public bool CheckResources(int resources)
    {
       int alteredCost = GetAlteredCost();
       
       if(alteredCost<=resources)
       {
       		return true;
       }
       return false;
    }

    public int GetAlteredCost()
    {
        int alteredCost = cost + cost_alteration;
        if(alteredCost < 0)
        {
            alteredCost = 0;
        }
        return alteredCost;
    }

    //SERVER ONLY
    public bool TakeDamage(int amount)
    {
        if(amount <= 0)
        {
            return false;
        }

        if(barrier)
        {
            barrier = false;
            return false;
        }

        hit_points -= amount;
        
        if (hit_points <= 0)
        {
            isDead = true;
            hit_points = 0;
        }

        return true;
    }

    //SERVER ONLY
    public void Heal(int amount)
    {
        if(hit_points >= maxHp)
        {
            return;
        }

        if((hit_points+amount) >= maxHp)
        {
            hit_points = maxHp;
        }else{
            hit_points += amount;
        }
    }

    //SERVER ONLY
    public void ClearStats()
    {
        isDead = false;
        isOnField = false;
        barrier = false;
        tank = false;
        haste = false;
        trample = false;
        direct_hit = false;
        double_hit = false;
        death_touch = false;
        isToken = false;
    }


    [Command(requiresAuthority = false)]
    public void CMDOnCardDeath()
    {

    }

    [ClientRpc]
    public void RPCOnCardDeath(uint playerID)
    {
        // UpdateData();

        // //On complete do this: 
        // if (hasAuthority)
        // {
        //     foreach(int effect_id in card_effects)
        //     {
        //         if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
        //         {
        //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];   
        //             if (effect.on_death_effect)
        //             {
        //                 effect.OnDie(
        //                     card,
        //                     () => {
        //                         Instantiate(ParticleManager.Instance.deathrattleParticle,GetComponent<RectTransform>().position, Quaternion.identity);
        //                         Debug.Log("Casted OnDie: " + effect.effect_name);
        //                     }
        //                 );
        //             }

        //             if(effect.is_aura)
        //             {
        //                 if(effect is DecreaseCostEffect)
        //                 {
        //                     DecreaseCostEffect auraEffect = (effect as DecreaseCostEffect);
                            
        //                     auraEffect.OnDie(card, 
        //                         ()=>{
        //                             Debug.Log("Casted OnDie: " + auraEffect.effect_name);
        //                         }
        //                     );
        //                 }else if(effect is IncreaseCostEffect)
        //                 {
        //                     IncreaseCostEffect auraEffect = (effect as IncreaseCostEffect);
                            
        //                     auraEffect.OnDie(card, 
        //                         ()=>{
        //                             Debug.Log("Casted OnDie: " + auraEffect.effect_name);
        //                         }
        //                     );
        //                 }
        //             }
        //         }
        //     }

        //     if(!isToken)
        //     {
        //         transform.SetParent(UIGameArena.Instance.playerGraveyard);
        //     }else{
        //         CardGameManager.Instance.CMDDestroyToken(playerID, netId);
        //     }
        // }
        // else
        // {
        //     transform.SetParent(UIGameArena.Instance.oponentGraveyard);
        // }

        // List<CardBehaviour> field = UIGameArena.Instance.GetPlayerField();
        // foreach(CardBehaviour current in field)
        // {
        //     if(current.hasAuthority)
        //     {
        //         foreach(int effect_id in current.card_effects)
        //         {
        //             if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
        //             {
        //                 CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

        //                 if(effect.on_creature_death)
        //                 {
        //                     effect.OnCreatureDeath(
        //                         current,
        //                         () => {
        //                             Debug.Log("Casted OnCreatureDeath: " + effect.effect_name + "___" + NetworkClient.localPlayer.netId);
        //                         }
        //                     );
        //                 }
        //             }
        //         }
        //     }
        // }

        // if(card != null && !isToken)
        // {
        //     Reset();
        // }
    }
}
