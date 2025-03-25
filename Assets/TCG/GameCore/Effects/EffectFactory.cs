using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Newtonsoft.Json;

public class EffectFactory : NetworkBehaviour
{
    public static EffectFactory Instance;

    public List<EffectData> allEffects = new List<EffectData>();
    public Dictionary<string, EffectBase> effectPool = new Dictionary<string, EffectBase>();

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    [Command(requiresAuthority = false)]
    private void CMDInitialize()
    {
        RPCInitialize();
    }

    [ClientRpc]
    private void RPCInitialize()
    {
        EffectData effect = null;

        foreach(EffectData current in allEffects)
        {
            ParseEffect(current);
        }
    }

    public async void InitializeFactory()
    {
        CMDInitialize();
    }

    public EffectBase GetEffectDataByName(string effect_name)
    {
        return effectPool[effect_name];
    }

    private async void ParseEffect(EffectData effectData)
    {
        HealEffect heal = null;

        if(effectData.effect_name.Contains("heal"))
        {
            GameObject temp = new GameObject(effectData.effect_name);
            temp.transform.SetParent(transform);
            heal = temp.AddComponent<HealEffect>();
            heal.SetEffect(effectData);
            effectPool.Add(heal.data.effect_name, heal);
        }
    }


    [Command(requiresAuthority = false)]
    public void CMDCardHeal(uint playerID, string targetsJson, string effect_name)
    {
        EffectBase effect = GetEffectDataByName(effect_name);

        ActionChain.Instance.AddToChain(
            "CMDCardHeal",
            (int actionID)=>{
                List<uint> targets = JsonConvert.DeserializeObject<List<uint>>(targetsJson);

                foreach(uint current in targets)
                {
                    CardBehaviour target = NetworkServer.spawned[current].GetComponent<CardBehaviour>();
                    target.Heal(effect.data.heal);
                }

                RPCResolveCardHeal(playerID, actionID, effect_name, targetsJson, effect.data.heal);
            }
        );
        if(!ActionChain.Instance.isChainBusy)
        {
            ActionChain.Instance.ResolveChain();
        }
    }

    [ClientRpc]
    private void RPCResolveCardHeal(uint playerID, int actionID, string effect_name, string targetsJson, int amount)
    {
        EffectBase effect = GetEffectDataByName(effect_name);

        List<uint> targets = JsonConvert.DeserializeObject<List<uint>>(targetsJson);

        foreach(uint targetID in targets)
        {
            effect.DelayedDamageResolver(playerID, actionID, targetID, amount, "heal");
        }

        if(targets.Count == 0)
        {
            ActionChain.Instance.CMDCompleteAction(playerID, actionID);
        }
        
        // This completes the action in the chain
        effect.DelayedEffectResolver(playerID, targetsJson, actionID);
    }
}
