using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory : Singleton<EffectFactory>
{
    public Dictionary<int, EffectBase> allEffects = new Dictionary<int, EffectBase>();
    
    //THIS IS OPTIONAL FOR
    // private void Awake()
    // {
    //     if(Instance != this)
    //     {
    //         GameObject.Destroy(gameObject);
    //         return;
    //     }

    //     DontDestroyOnLoad(gameObject);
    // }

    public void LoadEffects()
    {

    }

    public void UnloadEffects()
    {
        //TODO: Unload when unused
    }

    public EffectBase GetEffectByName(string effect_name)
    {
        EffectBase effect = null;

        foreach(KeyValuePair<int, EffectBase> current in allEffects)
        {
            if(current.Value.effect_name == effect_name)
            {
                effect = current.Value;
                break;
            }
        }

        return effect;
    }

    private void ParseEffect(Effect current)
    {
        // HealEffect heal = null;
        // DamageEffect damage = null;
        // FreezeCardEffect freezeCard = null;
        // PlussBuffEffect plus_buff = null;
        // DrawDiscardEffect draw = null;
        // ReturnToHandEffect return_hand = null;
        // CreateTokenEffect create_token = null;
        // AuraPlusEffect aura_plus = null;
        // ResourcesEffect resources_effect = null;
        // AuraDamageEffect aura_damage = null;
        // SearchEffect search = null;
        // DestroyTargetEffect destroy = null;
        // SilenceEffect silence = null;
        // SetStatsEffect set_stats = null;
        // SetBuffsEffect set_buffs = null;
        // DecreaseCostEffect decrease_cost = null;
        // IncreaseCostEffect increase_cost = null;
        // SacrificeBuffEffect sacrifice_buff = null;
        // CharmEffect charm = null;
        // ReinvokeEffect reinvoke = null;
        // MaterializeEffect materialize = null;
        // ConvertInfluenceEffect convert_influence = null;
        // NegateInfluenceEffect negate_influence = null;
        // EqualityEffect equality = null;
        // CopyEffect copy = null;
        // CostModifierEffect cost_modifier = null;
        // RemoveBuffEffect remove_buff = null;
        // GiveEffect give_effect = null;
        // DoubleStatsEffect double_stats = null;
        // ImprisonEffect imprison = null;
        // NeutralEffect neutral = null;

        // switch(current.effect_name)
        // {
        //     case("heal"):
        //         heal = new HealEffect();
        //         heal.SetEffect(current);
        //         allEffects.Add(heal.id, heal);
        //         break;
        //     case("damage"):
        //         damage = new DamageEffect();
        //         damage.SetEffect(current);
        //         allEffects.Add(damage.id, damage);
        //         break;
        //     case("draw"):
        //         draw = new DrawDiscardEffect();
        //         draw.SetEffect(current);
        //         allEffects.Add(draw.id, draw);
        //         break;
            
        //     case("counter+/-"):
        //         plus_buff = new PlussBuffEffect();
        //         plus_buff.SetEffect(current);
        //         allEffects.Add(plus_buff.id, plus_buff);
        //         break;

        //     case("token"):
        //         create_token = new CreateTokenEffect();
        //         create_token.SetEffect(current);
        //         allEffects.Add(create_token.id, create_token);
        //         break;
        //     case("aura_plus"):
        //         aura_plus = new AuraPlusEffect();
        //         aura_plus.SetEffect(current);
        //         allEffects.Add(aura_plus.id, aura_plus);
        //         break;
        //     case("unsummon"):
        //         return_hand = new ReturnToHandEffect();
        //         return_hand.SetEffect(current);
        //         allEffects.Add(return_hand.id, return_hand);
        //         break;
        //     case("freeze_card"):
        //         freezeCard = new FreezeCardEffect();
        //         freezeCard.SetEffect(current);
        //         allEffects.Add(freezeCard.id, freezeCard);
        //         break;
        //     case ("resouce_buff"):
        //         resources_effect = new ResourcesEffect();
        //         resources_effect.SetEffect(current);
        //         allEffects.Add(resources_effect.id, resources_effect);
        //         break;
        //     case ("aura_damage"):
        //         aura_damage = new AuraDamageEffect();
        //         aura_damage.SetEffect(current);
        //         allEffects.Add(aura_damage.id,aura_damage);
        //         break;
        //     case("influence_spirituality_1"):
        //         create_token = new CreateTokenEffect();
        //         create_token.SetEffect(current);
        //         allEffects.Add(create_token.id, create_token);
        //         break;
        //     case("influence_spirituality_2"):
        //         create_token = new CreateTokenEffect();
        //         create_token.SetEffect(current);
        //         allEffects.Add(create_token.id, create_token); 
        //         break;
        //     case("influence_spirituality_3"):
        //         create_token = new CreateTokenEffect();
        //         create_token.SetEffect(current);
        //         allEffects.Add(create_token.id, create_token);     
        //         break;
        //     case("influence_sport_1"):
        //         plus_buff = new PlussBuffEffect();
        //         plus_buff.SetEffect(current);
        //         allEffects.Add(plus_buff.id, plus_buff);
        //         break;
        //     case("influence_sport_2"):
        //         plus_buff = new PlussBuffEffect();
        //         plus_buff.SetEffect(current);
        //         allEffects.Add(plus_buff.id, plus_buff);
        //         break;
        //     case("influence_sport_3"):
        //         plus_buff = new PlussBuffEffect();
        //         plus_buff.SetEffect(current);
        //         allEffects.Add(plus_buff.id, plus_buff);
        //         break;
        //     case("influence_art_1"):
        //         damage = new DamageEffect();
        //         damage.SetEffect(current);
        //         allEffects.Add(damage.id, damage);
        //         break;
        //     case("influence_art_2"):
        //         damage = new DamageEffect();
        //         damage.SetEffect(current);
        //         allEffects.Add(damage.id, damage);
        //         break;
        //     case("influence_art_3"):
        //         damage = new DamageEffect();
        //         damage.SetEffect(current);
        //         allEffects.Add(damage.id, damage);
        //         break;
        //     case("influence_politic_1"):
        //         draw = new DrawDiscardEffect();
        //         draw.SetEffect(current);
        //         allEffects.Add(draw.id, draw);
        //         break;
        //     case("influence_politic_2"):
        //         draw = new DrawDiscardEffect();
        //         draw.SetEffect(current);
        //         allEffects.Add(draw.id, draw);
        //         break;
        //     case("influence_politic_3"):
        //         draw = new DrawDiscardEffect();
        //         draw.SetEffect(current);
        //         allEffects.Add(draw.id, draw);
        //         break;
        //     case("influence_science_1"):
        //         plus_buff = new PlussBuffEffect();
        //         plus_buff.SetEffect(current);
        //         allEffects.Add(plus_buff.id, plus_buff);
        //         break;
        //     case("influence_science_2"):
        //         plus_buff = new PlussBuffEffect();
        //         plus_buff.SetEffect(current);
        //         allEffects.Add(plus_buff.id, plus_buff);
        //         break;
        //     case("influence_science_3"):
        //         plus_buff = new PlussBuffEffect();
        //         plus_buff.SetEffect(current);
        //         allEffects.Add(plus_buff.id, plus_buff);
        //         break;
        //     case("search"):
        //         search = new SearchEffect();
        //         search.SetEffect(current);
        //         allEffects.Add(search.id, search);
        //         break;
        //     case("destroy"):
        //         destroy = new DestroyTargetEffect();
        //         destroy.SetEffect(current);
        //         allEffects.Add(destroy.id, destroy);
        //         break;
        //     case("set_stats"):
        //         set_stats = new SetStatsEffect();
        //         set_stats.SetEffect(current);
        //         allEffects.Add(set_stats.id, set_stats);
        //         break;
        //     case("silence"):
        //         silence = new SilenceEffect();
        //         silence.SetEffect(current);
        //         allEffects.Add(silence.id, silence);
        //         break;
        //     case("set_buffs"):
        //         set_buffs = new SetBuffsEffect();
        //         set_buffs.SetEffect(current);
        //         allEffects.Add(set_buffs.id, set_buffs);
        //         break;

        //     case("increase_cost"):
        //         increase_cost = new IncreaseCostEffect();
        //         increase_cost.SetEffect(current);
        //         allEffects.Add(increase_cost.id, increase_cost);
        //         break;

        //     case("decrease_cost"):
        //         decrease_cost = new DecreaseCostEffect();
        //         decrease_cost.SetEffect(current);
        //         allEffects.Add(decrease_cost.id, decrease_cost);
        //         break;
        //     case("sacrifice_buff"):
        //         sacrifice_buff = new SacrificeBuffEffect();
        //         sacrifice_buff.SetEffect(current);
        //         allEffects.Add(sacrifice_buff.id, sacrifice_buff);
        //         break;
        //     case("charm"):
        //         charm = new CharmEffect();
        //         charm.SetEffect(current);
        //         allEffects.Add(charm.id, charm);
        //         break;
        //     case("reinvoke"):
        //         reinvoke = new ReinvokeEffect();
        //         reinvoke.SetEffect(current);
        //         allEffects.Add(reinvoke.id, reinvoke);
        //         break;

        //     //TODO: 
        //     case("convert_influence"):
        //         convert_influence = new ConvertInfluenceEffect();
        //         convert_influence.SetEffect(current);
        //         allEffects.Add(convert_influence.id, convert_influence);
        //         break;
        //     case("negate_influence"):
        //         negate_influence = new NegateInfluenceEffect();
        //         negate_influence.SetEffect(current);
        //         allEffects.Add(negate_influence.id, negate_influence);
        //         break;
        //     case("equality"):
        //         equality = new EqualityEffect();
        //         equality.SetEffect(current);
        //         allEffects.Add(equality.id, equality);
        //         break;
        //     case("copy"):
        //         copy = new CopyEffect();
        //         copy.SetEffect(current);
        //         allEffects.Add(copy.id, copy);
        //         break;
        //     case("materialize"):
        //         materialize = new MaterializeEffect();
        //         materialize.SetEffect(current);
        //         allEffects.Add(materialize.id, materialize);
        //         break;

        //     case("cost_modifier"):
        //         cost_modifier = new CostModifierEffect();
        //         cost_modifier.SetEffect(current);
        //         allEffects.Add(cost_modifier.id, cost_modifier);
        //         break;

        //     case("remove_buff"):
        //         remove_buff = new RemoveBuffEffect();
        //         remove_buff.SetEffect(current);
        //         allEffects.Add(remove_buff.id, remove_buff);
        //         break;  

        //     case("give_effect"):
        //         give_effect = new GiveEffect();
        //         give_effect.SetEffect(current);
        //         allEffects.Add(give_effect.id, give_effect);
        //         break;
        //     case("double_stats"):
        //         double_stats = new DoubleStatsEffect();
        //         double_stats.SetEffect(current);
        //         allEffects.Add(double_stats.id, double_stats);
        //         break;
        //     case("imprison"):
        //         imprison = new ImprisonEffect();
        //         imprison.SetEffect(current);
        //         allEffects.Add(imprison.id, imprison);
        //         break;
        //     case("neutral"):
        //         neutral = new NeutralEffect();
        //         neutral.SetEffect(current);
        //         allEffects.Add(neutral.id, neutral);
        //         break;

        // }
    }
}
