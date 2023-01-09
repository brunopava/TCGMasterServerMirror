using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Newtonsoft.Json;
using System.Threading.Tasks;
using DG.Tweening;
using System.Linq;

// [System.Serializable]
// public class Token {
//     public int damage;
//     public int hit_points;
//     public int effect_id;
// }

public class EffectResolver : NetworkBehaviour
{
    private EffectServer serverEffect;
    private EffectClient clientEffect;

    public static EffectResolver Instance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        serverEffect = GetComponent<EffectServer>();
        clientEffect = GetComponent<EffectClient>();
    }

    [Command(requiresAuthority = false)]
    public void CMDSummonToken(uint playerID, int effect_id, string jsonToken)
    {
       serverEffect.SummonToken(playerID,effect_id,jsonToken);
    }

    [ClientRpc]
    public void RPCSummonToken(uint playerID, string jsonToken, string jsonCards, int actionID)
    {
        clientEffect.SummonToken(playerID,jsonToken,jsonCards,actionID);
    }

    // [Command(requiresAuthority = false)]
    // public void CMDUnsummon(uint playerID, uint sourceId, string jsonCards)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDUnsummon",
    //         (int actionID)=>{

    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);

    //             List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(jsonCards);

    //             foreach (uint current in cards)
    //             {
    //                 CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();

    //                 if(!card.isToken)
    //                 {
    //                     card.attack = card.originalAttack;
    //                     card.hit_points = card.originalHitpoints;
    //                     card.maxHp = card.originalHitpoints;
    //                     card.ClearStats();

    //                     if(CardGameManager.Instance.fieldByPlayer[oponentID].Contains(card))
    //                     {
    //                         Debug.Log("RETURNED ENEMY CARD TO HAND");
    //                         CardGameManager.Instance.fieldByPlayer[oponentID].Remove(card);
    //                         CardGameManager.Instance.handsByPlayer[oponentID].Add(card);    
    //                     }

    //                     if(CardGameManager.Instance.fieldByPlayer[playerID].Contains(card))
    //                     {
    //                         Debug.Log("RETURNED MY OWN CARD TO HAND");
    //                         CardGameManager.Instance.fieldByPlayer[playerID].Remove(card);
    //                         CardGameManager.Instance.handsByPlayer[playerID].Add(card);  
    //                     }
    //                 }
    //             }

    //             RPCUnsummon(playerID, oponentID, sourceId, jsonCards, actionID);

    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [ClientRpc]
    // private void RPCUnsummon(uint playerID, uint oponentID, uint sourceId, string jsonCards, int actionID)
    // {
    //     List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(jsonCards);

    //     List<CardBehaviour> cardBehaviours = new List<CardBehaviour>();

    //     foreach (uint current in cards)
    //     {
    //         CardBehaviour card = NetworkClient.spawned[current].GetComponent<CardBehaviour>();
    //         cardBehaviours.Add(card);
    //     }

    //     Sequence sequence = AnimationManager.ReturnToHand(cardBehaviours,
    //         ()=>{


    //             foreach(CardBehaviour card in cardBehaviours)
    //             {                 
    //                 if (card.hasAuthority)
    //                 {
    //                     if(card.cardVisual.sleepEffect != null)
    //                     {
    //                         card.cardVisual.sleepEffect.SetActive(false);
    //                     }
    //                     card.isDraggable = true;
    //                     card.cardVisual.SetIsPlayable(false);
    //                 }
    //             }

    //             if(CardGameManager.Instance.IsMyTurn())
    //             {
    //                 NetworkClient.localPlayer.GetComponent<CardGamePlayerManager>().CMDUpdateHandPlayability();
    //             }
    //         }
    //     );
    //     sequence.Play();

    //     CardGameManager.Instance.CMDCompleteAction(NetworkClient.localPlayer.netId, actionID);
    // }

    // [ClientRpc]
    // public void RPCEffectResolver(uint playerID, string json, int actionID)
    // {
    //     DelayedEffectResolver(playerID, json, actionID);
    // }

    // //CLIENT ONLY (THIS IS THE ONLY DEATH RESOLVER ON THE GAME)
    // public async void DelayedEffectResolver(uint playerID, string json, int actionID)
    // {
    //     await new WaitForSeconds(0.5f);
    //     List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

    //     List<uint> actors = new List<uint>();
    //     List<uint> actors_complete = new List<uint>();

    //     foreach (uint current in cards)
    //     {
    //         if(NetworkClient.spawned.ContainsKey(current))
    //         {
    //             CardBehaviour card = NetworkClient.spawned[current].GetComponent<CardBehaviour>();
    //             card.cardVisual.SetIsPlayable(false);

    //             if (card.isDead)
    //             {
    //                 actors.Add(card.netId);
    //                 ParticleManager.Instance.RemoveSleepParticle(card.netId);

    //                 Sequence die = AnimationManager.AnimateCardDeath(
    //                     card, 
    //                     () => 
    //                     {
    //                         OnCardDeath(playerID, card);
    //                         actors_complete.Add(card.netId);
    //                     }
    //                 );
    //                 die.Play();
    //             }else{
    //                 card.SetCardInfo(!card.hasAuthority);
    //             }
    //         }
    //     }

    //     if (CardGameManager.Instance.IsMyTurn())
    //     {
    //         NetworkClient.localPlayer.GetComponent<CardGamePlayerManager>().CMDUpdateFieldPlayability(false);
    //     }

    //     // await new WaitWhile(()=> actors.Count != actors_complete.Count);

    //     CardGameManager.Instance.CMDCompleteAction(NetworkClient.localPlayer.netId, actionID);
    // }

    // //CLIENT ONLY
    // public void OnCardDeath(uint playerID, CardBehaviour card)
    // {
    //     card.SetCardInfo();

    //     //On complete do this: 
    //     if (card.hasAuthority)
    //     {
    //         foreach(int effect_id in card.card_effects)
    //         {
    //             if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
    //             {
    //                 CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];   
    //                 if (effect.on_death_effect)
    //                 {
    //                     effect.OnDie(
    //                         card,
    //                         () => {
    //                             Instantiate(ParticleManager.Instance.deathrattleParticle,card.GetComponent<RectTransform>().position, Quaternion.identity);
    //                             Debug.Log("Casted OnDie: " + effect.effect_name);
    //                         }
    //                     );
    //                 }

    //                 if(effect.is_aura)
    //                 {
    //                     if(effect is DecreaseCostEffect)
    //                     {
    //                         DecreaseCostEffect auraEffect = (effect as DecreaseCostEffect);
                            
    //                         auraEffect.OnDie(card, 
    //                             ()=>{
    //                                 Debug.Log("Casted OnDie: " + auraEffect.effect_name);
    //                             }
    //                         );
    //                     }else if(effect is IncreaseCostEffect)
    //                     {
    //                         IncreaseCostEffect auraEffect = (effect as IncreaseCostEffect);
                            
    //                         auraEffect.OnDie(card, 
    //                             ()=>{
    //                                 Debug.Log("Casted OnDie: " + auraEffect.effect_name);
    //                             }
    //                         );
    //                     }
    //                 }
    //             }
    //         }

    //         if(!card.isToken)
    //         {
    //             card.transform.SetParent(UIGameArena.Instance.playerGraveyard);
    //         }else{
    //             CardGameManager.Instance.CMDDestroyToken(playerID, card.netId);
    //         }
    //     }
    //     else
    //     {
    //         card.transform.SetParent(UIGameArena.Instance.oponentGraveyard);
    //     }

    //     List<CardBehaviour> field = UIGameArena.Instance.GetPlayerField();
    //     foreach(CardBehaviour current in field)
    //     {
    //         if(current.hasAuthority)
    //         {
    //             foreach(int effect_id in current.card_effects)
    //             {
    //                 if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
    //                 {
    //                     CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //                     if(effect.on_creature_death)
    //                     {
    //                         effect.OnCreatureDeath(
    //                             current,
    //                             () => {
    //                                 Debug.Log("Casted OnCreatureDeath: " + effect.effect_name + "___" + NetworkClient.localPlayer.netId);
    //                             }
    //                         );
    //                     }
    //                 }
    //             }
    //         }
    //     }

    //     if(card != null && !card.isToken)
    //     {
    //         card.Reset();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDPauseChain(uint playerID)
    // {
    //     CardGameManager.Instance.actionChain.Pause();
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDPlayChain(uint playerID)
    // {
    //     CardGameManager.Instance.actionChain.Play();
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDGiveEffect(uint playerID, uint sourceId, int effect_id)
    // {
    //     Debug.Log("CMDGiveEffect");

    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDGiveEffect",
    //         (int actionID)=>{
    //             List<uint> cards = new List<uint>();
    //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //             if(effect.is_aura)
    //             {
    //                 Debug.Log(effect.is_aura_friendly);
    //                 Debug.Log(effect.is_aura_enemy);

    //                 if(effect.is_aura_friendly)
    //                 {
    //                     List<CardBehaviour> playerField = UIGameArena.Instance.GetPlayerField();
    //                     foreach(CardBehaviour current in playerField)
    //                     {
    //                         List<int> effects = current.card_effects;
    //                         effects.Add(effect.effect_id);
    //                         string combindedString = string.Join("_", effects);
    //                         current.effect_id = combindedString;
    //                     }
    //                 }

    //                 if(effect.is_aura_enemy)
    //                 {
    //                     List<CardBehaviour> oponentField = UIGameArena.Instance.GetOponentField();
    //                     foreach(CardBehaviour current in oponentField)
    //                     {
    //                         List<int> effects = current.card_effects;
    //                         effects.Add(effect.effect_id);
    //                         string combindedString = string.Join("_", effects);
    //                         current.effect_id = combindedString;
    //                     }
    //                 }
    //             }

    //             string json = JsonConvert.SerializeObject(cards);
    //             RPCEffectResolver(playerID, json, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDDecreaseCosts(uint playerID, int effect_id)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDDecreaseCosts",
    //         (int actionID)=>{
    //             List<uint> cards = new List<uint>();
    //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //             foreach(CardBehaviour card in CardGameManager.Instance.handsByPlayer[playerID])
    //             {
    //                 card.cost_alteration = card.cost_alteration + (effect.decrease_costs * -1); 
    //                 cards.Add(card.netId);          
    //             }

    //             string json = JsonConvert.SerializeObject(cards);
    //             RPCEffectResolver(playerID, json, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDResetDecrese(uint playerID, int effect_id)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDResetDecrese",
    //         (int actionID)=>{
    //             List<uint> cards = new List<uint>();
    //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //             foreach(CardBehaviour card in CardGameManager.Instance.handsByPlayer[playerID])
    //             {
    //                 card.cost_alteration = card.cost_alteration + (effect.decrease_costs);
    //                 cards.Add(card.netId);
    //             }

    //             string json = JsonConvert.SerializeObject(cards);
    //             RPCEffectResolver(playerID, json, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDIncreaseCosts(uint playerID, int effect_id)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDIncreaseCosts",
    //         (int actionID)=>{
    //             List<uint> cards = new List<uint>();
    //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);

    //             foreach(CardBehaviour card in CardGameManager.Instance.handsByPlayer[oponentID])
    //             {
    //                 card.cost_alteration = card.cost_alteration + (effect.increase_costs);
    //                 cards.Add(card.netId);
    //             }

    //             string json = JsonConvert.SerializeObject(cards);
    //             RPCEffectResolver(playerID, json, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
        
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDResetIncrease(uint playerID, int effect_id)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDResetIncrease",
    //         (int actionID)=>{
    //             List<uint> cards = new List<uint>();
    //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);

    //             foreach(CardBehaviour card in CardGameManager.Instance.handsByPlayer[oponentID])
    //             {
    //                 card.cost_alteration = card.cost_alteration + (effect.decrease_costs*-1);
    //                 cards.Add(card.netId);
    //             }

    //             string json = JsonConvert.SerializeObject(cards);
    //             RPCEffectResolver(playerID, json, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }   
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDResolveDoubleStats(uint playerID, string json, int effect_id)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDResolveDoubleStats",
    //         (int actionID)=>
    //         {
    //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //             List<EffectData> effectDatas = new List<EffectData>();

    //             List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

    //             foreach(uint cardID in cards)
    //             {
    //                 CardBehaviour card = NetworkServer.spawned[cardID].GetComponent<CardBehaviour>();

    //                 int currentAttack = card.attack;
    //                 int currentLife = card.hit_points;

    //                 if(effect.damage > 0)
    //                 {
    //                     card.attack = card.attack * 2;
    //                 }

    //                 if(effect.heal > 0)
    //                 {
    //                     card.hit_points = card.hit_points * 2;
    //                 }
                    
    //                 EffectData data = new EffectData();
    //                 data.cardID = card.netId;
    //                 data.isHeal = card.hit_points > currentLife;
    //                 data.isDamage = card.attack > currentAttack;
    //                 data.isMinusDamage = card.attack < currentAttack;
    //                 data.isMinusHealth = card.hit_points < currentLife;

    //                 effectDatas.Add(data);
    //             }

    //             string jsonData = JsonConvert.SerializeObject(effectDatas);
                
    //             RPCEffectResolver(playerID, json, actionID);
    //             RPCResolveEffectData(playerID, jsonData);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }        
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDResolveEquality(uint playerID, string json, int effect_id)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDResolveEquality",
    //         (int actionID)=>
    //         {
    //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //             List<EffectData> effectDatas = new List<EffectData>();

    //             List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

    //             foreach(uint cardID in cards)
    //             {
    //                 CardBehaviour card = NetworkServer.spawned[cardID].GetComponent<CardBehaviour>();

    //                 int currentAttack = card.attack;
    //                 int currentLife = card.hit_points;

    //                 if(effect.heal > 0)
    //                 {
    //                     card.attack = card.hit_points;
    //                 }

    //                 if(effect.damage > 0)
    //                 {
    //                     card.hit_points = card.attack;
    //                 }
                    
    //                 EffectData data = new EffectData();
    //                 data.cardID = card.netId;
    //                 data.isHeal = card.hit_points > currentLife;
    //                 data.isDamage = card.attack > currentAttack;
    //                 data.isMinusDamage = card.attack < currentAttack;
    //                 data.isMinusHealth = card.hit_points < currentLife;

    //                 effectDatas.Add(data);
    //             }

    //             string jsonData = JsonConvert.SerializeObject(effectDatas);
                
    //             RPCEffectResolver(playerID, json, actionID);
    //             RPCResolveEffectData(playerID, jsonData);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }        
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDResolvePlusBuff(uint playerID, string json, int effect_id)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDResolvePlusBuff",
    //         (int actionID)=>{
    //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //             List<EffectData> effectDatas = PlusBuff(playerID, json, effect_id);
    //             string jsonData = JsonConvert.SerializeObject(effectDatas);

    //             RPCEffectResolver(playerID, json, actionID);
    //             RPCResolveEffectData(playerID, jsonData);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }        
    // }

    // private List<EffectData> PlusBuff(uint playerID, string json, int effect_id)
    // {
    //     List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);
    //     List<EffectData> effectDatas = new List<EffectData>();

    //     uint oponentID = CardGameManager.Instance.GetOponentID(playerID);

    //     CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //     int plus_marker = effect.plus_marker;
    //     int minus_marker = effect.minus_marker;
    //     int damage = effect.damage;
    //     int heal = effect.heal;

    //     if(effect.use_enemy_count)
    //     {
    //         List<CardBehaviour> oponentField = UIGameArena.Instance.GetOponentField();
    //         plus_marker = plus_marker * oponentField.Count;
    //         heal = heal * oponentField.Count;
    //     }

    //     if(effect.use_ally_count)
    //     {
    //         List<CardBehaviour> playerField = UIGameArena.Instance.GetPlayerField();
    //         plus_marker = plus_marker * playerField.Count;
    //     }

    //     foreach (uint current in cards)
    //     {
    //         if(NetworkServer.spawned.ContainsKey(current))
    //         {
    //             CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();

    //             //pega original
    //             int currentAttack = card.attack;
    //             int currentLife = card.hit_points;

    //             if(plus_marker > 0)
    //             {
    //                 card.attack += plus_marker;
    //                 card.hit_points += plus_marker;
    //                 card.maxHp += plus_marker;
    //             }

    //             if(minus_marker >0)
    //             {
    //                 card.attack -= minus_marker;
    //                 card.hit_points -= minus_marker;
    //                 card.maxHp -= minus_marker;
    //             }


    //             if(damage != 0)
    //             {
    //                 card.hit_points -= damage;
    //             }
    //             else
    //             {
    //                 card.hit_points += heal;
    //                 card.maxHp += heal;
    //             }
    //             if(plus_marker != 0 && damage != 0)
    //             {
    //                 card.maxHp -= damage;
    //             }

    //             EffectData data = new EffectData();
    //             data.cardID = card.netId;
    //             data.isHeal = card.hit_points > currentLife;
    //             data.isDamage = card.attack > currentAttack;
    //             data.isMinusDamage = card.attack < currentAttack;
    //             data.isMinusHealth = card.hit_points < currentLife;

    //             effectDatas.Add(data);

    //             if (card.hit_points <= 0)
    //             {
    //                 card.hit_points = 0;
    //                 CardGameManager.Instance.UniversalDeathResolver(card);
    //             }

    //             if (card.attack < 0)
    //             {
    //                 card.attack = 0;
    //             }
    //         }
    //     }

    //     return effectDatas;
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDResolveFreezing(uint playerID, string json)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDResolveFreezing",
    //         (int actionID)=>{
    //             List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

    //             foreach (uint current in cards)
    //             {
    //                 CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();
    //                 card.frozenCounter = 1;
    //                 //TODO: FREEZE COUNTER NUMBER BASED ON EFFECT turn_counter
    //             }

    //             RPCEffectResolver(playerID, json, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }  
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDResolveFreezeEnemyField(uint playerID)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDResolveFreezeEnemyField",
    //         (int actionID)=>{
    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);

    //             List<uint> cards = new List<uint>();
    //             foreach(CardBehaviour card in CardGameManager.Instance.fieldByPlayer[oponentID])
    //             {
    //                 card.frozenCounter = 1;
    //                 cards.Add(card.netId);
    //             }

    //             string json = JsonConvert.SerializeObject(cards);
    //             RPCEffectResolver(playerID, json, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [ClientRpc]
    // private void RPCResolveEffectData(uint playerID, string jsonData)
    // {
    //     List<EffectData> cards = JsonConvert.DeserializeObject<List<EffectData>>(jsonData);

    //     // Debug.Log("RPC resolve Effect Data");
    //     foreach (EffectData data in cards)
    //     {
    //         if(NetworkClient.spawned.ContainsKey(data.cardID))
    //         {
    //             CardBehaviour target = NetworkClient.spawned[data.cardID].GetComponent<CardBehaviour>();

    //             // Debug.Log("RPC resolve Effect Data" + JsonConvert.SerializeObject(data).ToString());
    //             if (data.isHeal)
    //             {
    //                 NetworkClient.spawned[data.cardID].GetComponent<CardBehaviour>();
    //                 Instantiate(ParticleManager.Instance.plusHealthParticle, target.cardVisual.hitPointText.GetComponent<RectTransform>().position, Quaternion.identity);                
    //             }

    //             if (data.isDamage)
    //             {
    //                 NetworkClient.spawned[data.cardID].GetComponent<CardBehaviour>();
    //                 Instantiate(ParticleManager.Instance.plusAttackPartcle, target.cardVisual.attackText.GetComponent<RectTransform>().position, Quaternion.identity);
    //             }

    //             if (data.isMinusHealth)
    //             {                
    //                 NetworkClient.spawned[data.cardID].GetComponent<CardBehaviour>();
    //                 Instantiate(ParticleManager.Instance.weakenedDefenseParticle, target.cardVisual.hitPointText.GetComponent<RectTransform>().position, Quaternion.identity);
    //             }

    //             if (data.isMinusDamage)
    //             {
    //                 NetworkClient.spawned[data.cardID].GetComponent<CardBehaviour>();
    //                 Instantiate(ParticleManager.Instance.weakenedAttackParticle, target.cardVisual.attackText.GetComponent<RectTransform>().position, Quaternion.identity);
    //             }
    //         }
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDResourcesEffect(uint playerID, CardClass cardClass, int amount, bool isIncrease)
    // {
    //     if (isIncrease)
    //     {
    //         CardGameManager.Instance.resourcesByPlayer[playerID][cardClass] += amount;
    //     }
    //     else
    //     {
    //         CardGameManager.Instance.resourcesByPlayer[playerID][cardClass] -= amount;
    //     }
    //     //TODO: FEEDBACK
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDAuraDamageEffect(uint playerID, uint sourceId, int amount, bool is_aura_friendly)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDAuraDamageEffect",
    //         (int actionID)=>{
    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);
    //             List<CardBehaviour> oponentCards = CardGameManager.Instance.fieldByPlayer[oponentID];
    //             List<uint> targets = new List<uint>();

    //             for(int i = oponentCards.Count -1; i > -1; i--)
    //             {
    //                 CardBehaviour card = oponentCards[i];

    //                 card.TakeDamage(amount);    

    //                 if (card.isDead)
    //                 {
    //                     CardGameManager.Instance.UniversalDeathResolver(card);
    //                 }   
    //                 targets.Add(card.netId);
    //             }
                
    //             if(is_aura_friendly)
    //             {
    //                 List<CardBehaviour> playerCards = CardGameManager.Instance.fieldByPlayer[playerID];

    //                 for(int j = playerCards.Count -1; j > -1; j--)
    //                 {
    //                     CardBehaviour card = playerCards[j];
                        
    //                     if(card.netId != sourceId)
    //                     {
    //                         card.TakeDamage(amount);    

    //                         if (card.isDead)
    //                         {
    //                             CardGameManager.Instance.UniversalDeathResolver(card);
    //                         }

    //                         targets.Add(card.netId);  
    //                     }
    //                 }
    //             }

    //             string targetsJson = JsonConvert.SerializeObject(targets);
    //             CardGameManager.Instance.RPCResolveCardDamage(playerID, actionID, targetsJson, amount);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDAuraDamageToFullHpOnly(uint playerID, uint sourceId, int amount, bool is_aura_friendly)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDAuraDamageToFullHpOnly",
    //         (int actionID)=>{
    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);
    //             List<uint> targets = new List<uint>();
    //             List<CardBehaviour> oponentCards = CardGameManager.Instance.fieldByPlayer[oponentID];

    //             for(int i = oponentCards.Count -1; i > -1; i--)
    //             {
    //                 CardBehaviour card = oponentCards[i];
    //                 if(card.hit_points == card.maxHp)
    //                 {
    //                     card.TakeDamage(amount);
    //                     if (card.isDead)
    //                     {
    //                         CardGameManager.Instance.UniversalDeathResolver(card);
    //                     }   
    //                     targets.Add(card.netId);
    //                 }
    //             }
                
    //             if(is_aura_friendly)
    //             {
    //                 List<CardBehaviour> playerCards = CardGameManager.Instance.fieldByPlayer[playerID];

    //                 for(int j = playerCards.Count -1; j > -1; j--)
    //                 {
    //                     CardBehaviour card = playerCards[j];
                        
    //                     if(card.netId != sourceId)
    //                     {
    //                         if(card.hit_points == card.maxHp)
    //                         {
    //                             card.TakeDamage(amount);
    //                             if (card.isDead)
    //                             {
    //                                 CardGameManager.Instance.UniversalDeathResolver(card);
    //                             }
    //                             targets.Add(card.netId);
    //                         }
    //                     }
    //                 }
    //             }
    //             string targetsJson = JsonConvert.SerializeObject(targets);
    //             CardGameManager.Instance.RPCResolveCardDamage(playerID, actionID, targetsJson, amount);    
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDDrawSpecific(uint playerID, string json)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDDrawSpecific",
    //         (int actionID)=>{
    //             DrawSpecific(playerID, json, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDOponentDiscard(uint playerID, int effect_id)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDOponentDiscard",
    //         (int actionID)=>
    //         {
    //             CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];
    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);

    //             List<CardBehaviour> cards = CardGameManager.Instance.handsByPlayer[oponentID];

    //             List<CardBehaviour> shuffledList = cards.OrderBy( x => Random.value ).ToList( );

    //             List<uint> chosen = new List<uint>();

    //             for(int i = 0; i < effect.oponent_discard_cards; i++)
    //             {
    //                 chosen.Add(shuffledList[i].netId);
    //             }

    //             string jsonDiscard = JsonConvert.SerializeObject(chosen);
    //             CardGameManager.Instance.RPCSendCardToGraveyard(jsonDiscard);
    //             RPCResolveActionChain(actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // //SERVER ONLY
    // private void DrawSpecific(uint playerID, string json, int actionID)
    // {
    //     bool isHandFull = false;

    //     List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

    //     List<uint> drawedCards = new List<uint>();
    //     List<uint> discardedCards = new List<uint>();

    //     foreach (uint current in cards)
    //     {
    //         CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();

    //         if(CardGameManager.Instance.decksByPlayer[playerID].Contains(card))
    //         {
    //             CardGameManager.Instance.decksByPlayer[playerID].Remove(card);
    //         }

    //         if (CardGameManager.Instance.handsByPlayer[playerID].Count >= Constants.MAX_CARDS_PER_HAND)
    //         {
    //             isHandFull = true;
    //             CardGameManager.Instance.graveyardByPlayer[playerID].Add(card);
    //             discardedCards.Add(card.netId);
    //         }else{
    //             CardGameManager.Instance.handsByPlayer[playerID].Add(card);
    //             drawedCards.Add(card.netId);
    //         }
    //     }
        
    //     string jsonDraw = JsonConvert.SerializeObject(drawedCards);
    //     string jsonDiscard = JsonConvert.SerializeObject(discardedCards);

    //     CardGameManager.Instance.RPCSendCardToGraveyard(jsonDiscard);

    //     CardGameManager.Instance.RPCDrawCard(jsonDraw, false);

    //     RPCResolveActionChain(actionID);
    // }

    // [ClientRpc]
    // private void RPCResolveActionChain(int actionID)
    // {
    //     CardGameManager.Instance.CMDCompleteAction(NetworkClient.localPlayer.netId, actionID);
    // }

    // //SERVER ONLY
    // private void DiscardSpecific(uint playerID, string json)
    // {
    //     Debug.Log("DiscardSpecific");
    //     List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

    //     foreach(uint current in cards)
    //     {
    //         CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();
    //         CardGameManager.Instance.decksByPlayer[playerID].Remove(card);
    //         CardGameManager.Instance.graveyardByPlayer[playerID].Add(card);
    //     }
        
    //     CardGameManager.Instance.RPCSendCardToGraveyard(json);
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDInvokeSpecific(uint playerID, string json)
    // {
    //     List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

    //     foreach(uint current in cards)
    //     {
    //         CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();            
    //         CardGameManager.Instance.CMDSummonCard(playerID, card.netId);
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDDrawDiscard(uint playerID, int effect_id, string jsonDraw, string jsonDiscard)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDDrawDiscard",
    //         (int actionID)=>{

    //             DrawSpecific(playerID, jsonDraw, actionID);
    //             DiscardSpecific(playerID, jsonDiscard);

    //             if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
    //             {
    //                 CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //                 if(effect.discard_oponent_deck > 0)
    //                 {
    //                     List<uint> discardedCards = new List<uint>();
    //                     uint oponentID = CardGameManager.Instance.GetOponentID(playerID);
    //                     List<CardBehaviour> cards = CardGameManager.Instance.decksByPlayer[oponentID];

    //                     for(int i = 0; i < effect.discard_oponent_deck; i++)
    //                     {
    //                         if(i < cards.Count)
    //                         {
    //                             discardedCards.Add(cards[i].netId);
    //                         }
    //                     }

    //                     string discard = JsonConvert.SerializeObject(discardedCards);
    //                     DiscardSpecific(oponentID, discard);
    //                 }
    //                 if(effect.discard_self_deck > 0)
    //                 {
    //                     List<uint> discardedCards = new List<uint>();
    //                     List<CardBehaviour> cards = CardGameManager.Instance.decksByPlayer[playerID];

    //                     for(int i = 0; i < effect.discard_self_deck; i++)
    //                     {
    //                         if(i < cards.Count)
    //                         {
    //                             discardedCards.Add(cards[i].netId);
    //                         }
    //                     }
    //                     string discard = JsonConvert.SerializeObject(discardedCards);
    //                     DiscardSpecific(playerID, discard);
    //                 }
    //             }
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDSetStats(uint playerID, string json, int health, int attack)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDSetStats",
    //         (int actionID)=>{

    //             SetStats(playerID, json, health, attack);

    //             RPCEffectResolver(playerID, json, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDAuraSetStatsEffect(uint playerID, uint sourceId, string jsonEffect)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDAuraSetStatsEffect",
    //         (int actionID)=>{
    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);
        
    //             List<uint> cards = new List<uint>();

    //             CardEffectBase stained = JsonConvert.DeserializeObject<CardEffectBase>(jsonEffect);
    //             CardEffectBase effect = EffectFactory.Instance.allEffects[stained.id];

    //             if(effect.is_aura_enemy)
    //             {
    //                 foreach(CardBehaviour card in CardGameManager.Instance.fieldByPlayer[oponentID])
    //                 {
    //                     cards.Add(card.netId);
    //                 }
    //             }
    //             if(effect.is_aura_friendly)
    //             {
    //                 foreach(CardBehaviour card in CardGameManager.Instance.fieldByPlayer[playerID])
    //                 {
    //                     if(card.netId != sourceId)
    //                     {
    //                         cards.Add(card.netId);
    //                     }
    //                 }
    //             }
    //             string json  = JsonConvert.SerializeObject(cards);

    //             SetStats(playerID, json, effect.heal, effect.damage);
                
    //             RPCEffectResolver(playerID, json, actionID);
                
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // private void SetStats(uint playerID, string json, int health, int attack)
    // {
    //     List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

    //     foreach(uint current in cards)
    //     {
    //         CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();
    //         if(attack!=666){
    //             card.attack = attack;
    //             card.originalAttack = attack;
    //         }
    //         if(health!=666){
    //             card.hit_points = health;
    //             card.originalHitpoints = health;
    //             card.maxHp = health;
    //         }
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDSetBuffs(uint playerID, string json, string jsonEffect)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDSetBuffs",
    //         (int actionID)=>{

    //             SetBuffs(playerID, json, jsonEffect);

    //             RPCEffectResolver(playerID, json, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDAuraSetBuffsEffect(uint playerID, uint sourceId, string jsonEffect)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDAuraSetBuffsEffect",
    //         (int actionID)=>{
    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);
        
    //             List<uint> cards = new List<uint>();

    //             CardEffectBase stained = JsonConvert.DeserializeObject<CardEffectBase>(jsonEffect);
    //             CardEffectBase effect = EffectFactory.Instance.allEffects[stained.id];

    //             if(effect.is_aura_enemy)
    //             {
    //                 foreach(CardBehaviour card in CardGameManager.Instance.fieldByPlayer[oponentID])
    //                 {
    //                     cards.Add(card.netId);
    //                 }
    //             }
                
    //             if(effect.is_aura_friendly)
    //             {
    //                 foreach(CardBehaviour card in CardGameManager.Instance.fieldByPlayer[playerID])
    //                 {
    //                     if(card.netId != sourceId)
    //                     {
    //                         cards.Add(card.netId);
    //                     }
    //                 }
    //             }

    //             string json  = JsonConvert.SerializeObject(cards);
                
    //             SetBuffs(playerID, json, jsonEffect);
                
    //             RPCEffectResolver(playerID, json, actionID);
                
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }   
    // }

    // private void SetBuffs(uint playerID, string json, string jsonEffect)
    // {
    //     List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);
    //     CardEffectBase stained = JsonConvert.DeserializeObject<CardEffectBase>(jsonEffect);
    //     CardEffectBase effect = EffectFactory.Instance.allEffects[stained.id];

    //     foreach(uint current in cards)
    //     {
    //         CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();
    //         if (!card.barrier)
    //         {
    //             card.barrier = effect.barrier;
    //         }
    //         if (!card.tank)
    //         {
    //             card.tank = effect.tank;
    //         }
    //         if(!card.freeze_on_hit)
    //         {
    //             card.freeze_on_hit = effect.freeze_on_hit;
    //         }
    //         if(!card.haste)
    //         {
    //             card.haste = effect.haste;
    //             if(!CardGameManager.Instance.cardsAttackedThisTurn.Contains(card.netId) && card.attack > 0)
    //             {
    //                 card.isAttackEnabled = true;
    //             }
    //         }
    //         if(!card.trample)
    //         {
    //             card.trample = effect.trample;
    //         }
    //         if(!card.direct_hit)
    //         {
    //             card.direct_hit = effect.direct_hit;
    //         }
    //         if(!card.double_hit)
    //         {
    //             card.double_hit = effect.double_hit;
    //         }
    //         if(!card.death_touch)
    //         {
    //             card.death_touch = effect.death_touch;
    //         }
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDGiveCharmTo(uint playerID, string json, uint source)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDGiveCharmTo",
    //         (int actionID)=>{
    //             List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

    //             foreach (uint current in cards)
    //             {
    //                 CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();
    //                 card.charm = true;
    //                 card.charm_id = source;
    //             }

    //             RPCEffectResolver(playerID, json, actionID);   
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDSacrificeBuff(uint playerID, uint cardID, string json, string jsonEffect)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDSacrificeBuff",
    //         (int actionID)=>{
    //             List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);
    //             CardEffectBase effect = JsonConvert.DeserializeObject<CardEffectBase>(jsonEffect);

    //             List<uint> allCards = new List<uint>();
    //             foreach(uint current in cards)
    //             {
    //                 CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();
    //                 CardGameManager.Instance.UniversalDeathResolver(card);
    //                 allCards.Add(current);
    //             }

    //             List<uint> buffedCards = new List<uint>();
    //             buffedCards.Add(cardID);
    //             allCards.Add(cardID);

    //             string buffs = JsonConvert.SerializeObject(buffedCards);
    //             List<EffectData> effectDatas = PlusBuff(playerID, buffs, effect.id);

    //             string all = JsonConvert.SerializeObject(allCards);
    //             RPCEffectResolver(playerID, all, actionID);

    //             string jsonData = JsonConvert.SerializeObject(effectDatas);
    //             RPCResolveEffectData(playerID, jsonData);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDSilence(uint playerID, string json)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDSilence",
    //         (int actionID)=>{
    //             List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);

    //             foreach(uint current in cards)
    //             {
    //                 CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();
    //                 card.barrier = false;
    //                 card.tank = false;
    //                 card.freeze_on_hit = false;
    //                 card.haste = false;
    //                 card.trample = false;
    //                 card.direct_hit = false;
    //                 card.double_hit = false;
    //                 card.death_touch = false;
    //                 if(card.attack > card.originalAttack)
    //                 {
    //                     card.attack = card.originalAttack;
    //                 }
    //                 if(card.hit_points > card.originalHitpoints)
    //                 {
    //                     card.hit_points = card.originalHitpoints;
    //                     card.maxHp = card.originalHitpoints; 
    //                 }

    //                 card.effect_id = "69";
    //             }

    //             RPCEffectResolver(playerID, json, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDRemoveBuffs(uint playerID, uint sourceId, int effect_id)
    // {
    //      CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDRemoveBuffs",
    //         (int actionID)=>{
    //             List<uint> cards = new List<uint>();
    //             List<EffectData> effectDatas = new List<EffectData>();

    //             CardBehaviour sourceCard = NetworkServer.spawned[sourceId].GetComponent<CardBehaviour>();

    //             if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
    //             {
    //                 CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //                 if(effect.is_aura_enemy)
    //                 {
    //                     List<CardBehaviour> oponentField = UIGameArena.Instance.GetOponentField();
    //                     foreach(CardBehaviour current in oponentField)
    //                     {
    //                         if(current.barrier && effect.barrier)
    //                         {
    //                             current.barrier = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.tank  && effect.tank)
    //                         {
    //                             current.tank = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.direct_hit  && effect.direct_hit)
    //                         {
    //                             current.direct_hit = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.double_hit  && effect.double_hit)
    //                         {
    //                             current.double_hit = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.trample  && effect.trample)
    //                         {
    //                             current.trample = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.haste  && effect.haste)
    //                         {
    //                             current.haste = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.freeze_on_hit  && effect.freeze_on_hit)
    //                         {
    //                             current.freeze_on_hit = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.death_touch  && effect.death_touch)
    //                         {
    //                             current.death_touch = false;
    //                             cards.Add(current.netId);
    //                         }
    //                     }
    //                 }

    //                 if(effect.is_aura_friendly)
    //                 {
    //                     List<CardBehaviour> playerField = UIGameArena.Instance.GetPlayerField();
    //                     foreach(CardBehaviour current in playerField)
    //                     {
    //                         if(current.barrier && effect.barrier)
    //                         {
    //                             current.barrier = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.tank  && effect.tank)
    //                         {
    //                             current.tank = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.direct_hit  && effect.direct_hit)
    //                         {
    //                             current.direct_hit = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.double_hit  && effect.double_hit)
    //                         {
    //                             current.double_hit = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.trample  && effect.trample)
    //                         {
    //                             current.trample = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.haste  && effect.haste)
    //                         {
    //                             current.haste = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.freeze_on_hit  && effect.freeze_on_hit)
    //                         {
    //                             current.freeze_on_hit = false;
    //                             cards.Add(current.netId);
    //                         }

    //                         if(current.death_touch  && effect.death_touch)
    //                         {
    //                             current.death_touch = false;
    //                             cards.Add(current.netId);
    //                         }
    //                     }
    //                 }

    //                 if(effect.plus_marker > 0)
    //                 {
    //                     int currentAttack = sourceCard.attack;
    //                     int currentLife = sourceCard.hit_points;

    //                     sourceCard.attack += (effect.plus_marker * cards.Count);
    //                     sourceCard.hit_points += (effect.plus_marker * cards.Count);
    //                     sourceCard.maxHp += (effect.plus_marker * cards.Count);

    //                     EffectData data = new EffectData();
    //                     data.cardID = sourceCard.netId;
    //                     data.isHeal = sourceCard.hit_points > currentLife;
    //                     data.isDamage = sourceCard.attack > currentAttack;
    //                     data.isMinusDamage = sourceCard.attack < currentAttack;
    //                     data.isMinusHealth = sourceCard.hit_points < currentLife;

    //                     cards.Add(sourceCard.netId);
    //                     effectDatas.Add(data);
    //                 }
    //             }

    //             string jsonData = JsonConvert.SerializeObject(effectDatas);
    //             string jsonCards = JsonConvert.SerializeObject(cards);

    //             RPCResolveEffectData(playerID, jsonData);
    //             RPCEffectResolver(playerID, jsonCards, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }   
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDDestroyTarget(uint playerID, uint sourceId, string json, int effect_id)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDDestroyTarget",
    //         (int actionID)=>{
    //             List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(json);
    //             List<EffectData> effectDatas = new List<EffectData>();

    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);

    //             CardBehaviour sourceCard = NetworkServer.spawned[sourceId].GetComponent<CardBehaviour>();

    //             foreach(uint current in cards)
    //             {
    //                 CardBehaviour card = NetworkServer.spawned[current].GetComponent<CardBehaviour>();

    //                 if(CardGameManager.Instance.fieldByPlayer[playerID].Contains(card))
    //                 {
    //                     CardGameManager.Instance.UniversalDeathResolver(card);
    //                 }
    //                 else if(CardGameManager.Instance.fieldByPlayer[oponentID].Contains(card))
    //                 {
    //                     CardGameManager.Instance.UniversalDeathResolver(card);
    //                 }
    //             }

    //             if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
    //             {
    //                 CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //                 if(effect.plus_marker > 0)
    //                 {
    //                     int currentAttack = sourceCard.attack;
    //                     int currentLife = sourceCard.hit_points;

    //                     sourceCard.attack += (effect.plus_marker * cards.Count);
    //                     sourceCard.hit_points += (effect.plus_marker * cards.Count);
    //                     sourceCard.maxHp += (effect.plus_marker * cards.Count);

    //                     EffectData data = new EffectData();
    //                     data.cardID = sourceCard.netId;
    //                     data.isHeal = sourceCard.hit_points > currentLife;
    //                     data.isDamage = sourceCard.attack > currentAttack;
    //                     data.isMinusDamage = sourceCard.attack < currentAttack;
    //                     data.isMinusHealth = sourceCard.hit_points < currentLife;

    //                     cards.Add(sourceCard.netId);
    //                     effectDatas.Add(data);
    //                 }
    //             }

    //             string jsonData = JsonConvert.SerializeObject(effectDatas);
    //             string jsonCards = JsonConvert.SerializeObject(cards);

    //             RPCResolveEffectData(playerID, jsonData);
    //             RPCEffectResolver(playerID, jsonCards, actionID);
    //         }
    //     );
    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }   
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDAuraDestroy(uint playerID, uint cardID)
    // {
    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDAuraDestroy",
    //         (int actionID)=>{
    //             List<uint> cards = new List<uint>();
    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);

    //             foreach(CardBehaviour current in CardGameManager.Instance.fieldByPlayer[playerID])
    //             {
    //                 cards.Add(current.netId);
    //             }

    //             foreach(CardBehaviour current in CardGameManager.Instance.fieldByPlayer[oponentID])
    //             {
    //                 cards.Add(current.netId);
    //             }

    //             foreach(uint id in cards)
    //             {
    //                 CardBehaviour current = NetworkServer.spawned[id].GetComponent<CardBehaviour>();
    //                 CardGameManager.Instance.UniversalDeathResolver(current);
    //             }

    //             string json  = JsonConvert.SerializeObject(cards);

    //             RPCEffectResolver(playerID, json, actionID);
    //         }
    //     );

    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     }   
    // }

    // [Command(requiresAuthority = false)]
    // public void CMDMaterialize(uint playerID, int effect_id)
    // {
    //     CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

    //     ResourcesManager.Instance.LoadCard(effect.card_id);

    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "DelayedLoadCard",
    //         (int actionID)=>{
    //             DelayedLoadCard(effect_id, actionID);
    //         }
    //     );

    //     CardGameManager.Instance.actionChain.AddToChain(
    //         "CMDMaterialize",
    //         (int actionID)=>{
    //             uint oponentID = CardGameManager.Instance.GetOponentID(playerID);

    //             uint currentPlayer = playerID;
    //             List<uint> cards = new List<uint>();

    //             for(int i = 0; i < effect.target_number; i ++)
    //             {
    //                 GameObject card = Instantiate(CardGameManager.Instance.cardPrefab, new Vector2(-8000, 0), Quaternion.identity);
    //                 CardBehaviour cardBehaviour = card.GetComponent<CardBehaviour>();

    //                 if(effect.is_aura_friendly)
    //                 {
    //                     currentPlayer = playerID;
    //                 }else if(effect.is_aura_enemy)
    //                 {
    //                     currentPlayer = oponentID;
    //                 }

    //                 NetworkIdentity player = NetworkServer.spawned[currentPlayer];
    //                 CardGamePlayerManager playerManager = player.GetComponent<CardGamePlayerManager>();

    //                 cardBehaviour.cardID = effect.card_id;
    //                 cardBehaviour.ownerID = currentPlayer;

    //                 NetworkServer.Spawn(card, player.connectionToClient);
                    
    //                 cardBehaviour.InitializeCard();

    //                 cards.Add(cardBehaviour.netId);
    //             }

    //             string json = JsonConvert.SerializeObject(cards);

    //             DrawSpecific(currentPlayer, json, actionID);
    //         }
    //     );

    //     if(!CardGameManager.Instance.actionChain.isChainBusy)
    //     {
    //         CardGameManager.Instance.actionChain.ResolveChain();
    //     } 
    // }

    // public async void DelayedLoadCard(int effect_id, int actionID)
    // {
    //     CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];
    //     RPCLoadCard(effect.card_id);
    //     await new WaitForSeconds(0.5f);
    //     RPCResolveActionChain(actionID);
    // }

    // [ClientRpc]
    // public void RPCLoadCard(string cardID)
    // {
    //     // Debug.Log("DelayedLoadCard");
    //     ResourcesManager.Instance.LoadCard(cardID);
    // }

    // public IEnumerator SummonDelay(CardBehaviour card)
    // {
    //     yield return new WaitForSeconds(0.3f);
    //     Instantiate(ParticleManager.Instance.summonParticle, card.GetComponent<RectTransform>().position, Quaternion.identity);
    // }

    // public class EffectData
    // {
    //     public uint cardID;
    //     public bool isDamage;
    //     public bool isHeal;
    //     public bool isMinusDamage;
    //     public bool isMinusHealth;
    // }

}
