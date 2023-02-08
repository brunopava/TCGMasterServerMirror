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
    public string effect_id;

    public List<int> card_effects{
        get {
            List<int> list = new List<int>();

            string[] effects = effect_id.Split('_');
            foreach(string current in effects)
            {
                if(!string.IsNullOrEmpty(current))
                {
                    int id = 666;
                    if(int.TryParse(current, out id))
                    {
                        list.Add(id);
                    }
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
}
