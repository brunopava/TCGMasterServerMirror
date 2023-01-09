using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Newtonsoft.Json;
using System.Threading.Tasks;
using DG.Tweening;
using System.Linq;

public class EffectClient : MonoBehaviour
{
    public void SummonToken(uint playerID, string jsonToken, string jsonCards, int actionID)
    {
        // List<uint> cards = JsonConvert.DeserializeObject<List<uint>>(jsonCards);
        // List<Token> token = JsonConvert.DeserializeObject<List<Token>>(jsonToken);

        // for (int i = 0; i < cards.Count; i++)
        // {
        //     NetworkIdentity netiden = NetworkClient.spawned[cards[i]];
        //     CardBehaviour card = netiden.GetComponent<CardBehaviour>();

        //     if (card.hasAuthority)
        //     {
        //         card.transform.SetParent(UIGameArena.Instance.playerField);
        //         card.SetCardInfo();
        //         card.isInteractable = true;
        //         card.isDraggable = false;

        //         foreach(int effect_id in card.card_effects)
        //         {
        //             if (EffectFactory.Instance.allEffects.ContainsKey(effect_id))
        //             {
        //                 CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];

        //                 if (effect.on_cast_effect)
        //                 {
        //                     effect.OnCast(
        //                         card,
        //                         () =>
        //                         {
        //                             Debug.Log("Casted: " + effect.effect_name);
        //                         }
        //                     );
        //                 }
        //             }
        //         }

        //         for(int j = 0; j<UIGameArena.Instance.playerField.childCount; j++)
        //         {
        //             CardBehaviour current  = UIGameArena.Instance.playerField.GetChild(j).GetComponent<CardBehaviour>();

        //             foreach(int effect_id in current.card_effects)
        //             {
        //                 if(EffectFactory.Instance.allEffects.ContainsKey(effect_id) && current.netId != card.netId)
        //                 {
        //                     CardEffectBase effect = EffectFactory.Instance.allEffects[effect_id];
                            
        //                     if(effect.is_aura && effect is AuraPlusEffect)
        //                     {
        //                         AuraPlusEffect auraEffect = (effect as AuraPlusEffect);
        //                         List<ITarget> targets = new List<ITarget>();
        //                         targets.Add(card);
        //                         auraEffect.OnLateSummon(targets, 
        //                             ()=>{
        //                                 Debug.Log("Casted: " + auraEffect.effect_name);
        //                             }
        //                         );
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     else
        //     {
        //         card.transform.SetParent(UIGameArena.Instance.oponentField);
        //         card.SetCardInfo();
        //     }
        //     StartCoroutine(SummonDelay(card));
        // }

        // CardGameManager.Instance.CMDCompleteAction(NetworkClient.localPlayer.netId, actionID);
    }
}