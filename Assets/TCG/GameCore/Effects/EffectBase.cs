using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization;

public delegate void OnEffectApplied ();
public delegate void OnCancelCast ();

[Serializable]
public class EffectBase
{
    public int id;
    public string effect_name;
    public string description_label;

    //Stats Modifiers
    public int damage;
    public int heal;
    public int target_number;    
    public int plus_marker; 
    public int minus_marker;

    public int draw_cards;  
    public int self_discard_cards;  
    public int oponent_discard_cards;

    public int create_tokens;   
    public int token_skin;

    public int turn_counter;
    public int increase_costs;  
    public int decrease_costs;  
    public int increase_resources; 

    public int effect_id;
    public string card_id;

    public bool on_cast_effect;  
    public bool on_death_effect;
    public bool on_start_turn_effect;    
    public bool on_start_turn_graveyard;    
    public bool on_end_turn_effect;

    public bool on_hand_effect;
    public bool on_creature_death;
    public bool on_summon;
    public bool on_draw;
    public bool on_discard;

    public bool observe_oponent;
    public bool use_ally_count;
    public bool use_enemy_count;
    public bool use_player_hand_count;
    public bool use_enemy_hand_count;
     
    public bool is_aura; 
    public int aura_priority; 
    public bool is_aura_friendly;   
    public bool is_aura_enemy;
    
    public bool destroy_target;
    public bool return_target_to_hand;  
    public bool search_in_deck;
    public bool search_in_graveyard;
    public bool only_wounded;
    public int discard_self_deck;
    public int discard_oponent_deck;
    public bool use_category;
    public bool use_sinergy;
    public bool creature_only;
    public bool fury;
    public bool tank;
    public bool barrier;
    public bool haste;
    public bool death_touch;
    public bool double_hit; 
    public bool direct_hit;
    public bool trample;
    public bool silence;
    public bool imprison;
    public bool freeze_on_hit;
    public bool charm;

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

    public void SetEffect(Effect _effect)
    {
        effect_name = _effect.effect_name;
        effect_name = _effect.description_label;

        int.TryParse(_effect.id, out id);
        int.TryParse(_effect.damage, out damage);
        int.TryParse(_effect.heal, out heal);
        int.TryParse(_effect.target_number, out target_number);
        int.TryParse(_effect.plus_marker, out plus_marker);
        int.TryParse(_effect.minus_marker, out minus_marker);
        int.TryParse(_effect.draw_cards, out draw_cards);
        int.TryParse(_effect.self_discard_cards, out self_discard_cards);
        int.TryParse(_effect.oponent_discard_cards, out oponent_discard_cards);
        int.TryParse(_effect.create_tokens, out create_tokens);
        int.TryParse(_effect.turn_counter, out turn_counter);
        int.TryParse(_effect.increase_costs, out increase_costs);
        int.TryParse(_effect.decrease_costs, out decrease_costs);
        int.TryParse(_effect.increase_resources, out increase_resources);
        int.TryParse(_effect.discard_self_deck, out discard_self_deck);
        int.TryParse(_effect.discard_oponent_deck, out discard_oponent_deck);
        int.TryParse(_effect.effect_id, out effect_id);
        int.TryParse(_effect.token_skin, out token_skin);

        card_id = _effect.card_id;

        int _observe_oponent = 666;
        int.TryParse(_effect.observe_oponent, out _observe_oponent);
        observe_oponent = Convert.ToBoolean(_observe_oponent);

        int _on_cast_effect = 666;
        int.TryParse(_effect.on_cast_effect, out _on_cast_effect);
        on_cast_effect = Convert.ToBoolean(_on_cast_effect);

        int _on_death_effect = 666;
        int.TryParse(_effect.on_death_effect, out _on_death_effect);
        on_death_effect = Convert.ToBoolean(_on_death_effect);

        int _on_start_turn_effect = 666;
        int.TryParse(_effect.on_start_turn_effect, out _on_start_turn_effect);
        on_start_turn_effect = Convert.ToBoolean(_on_start_turn_effect);

        int _on_start_turn_graveyard = 666;
        int.TryParse(_effect.on_start_turn_graveyard, out _on_start_turn_graveyard);
        on_start_turn_graveyard = Convert.ToBoolean(_on_start_turn_graveyard);

        int _on_end_turn_effect = 666;
        int.TryParse(_effect.on_end_turn_effect, out _on_end_turn_effect);
        on_end_turn_effect = Convert.ToBoolean(_on_end_turn_effect);

        int _is_aura = 666;
        int.TryParse(_effect.is_aura, out _is_aura);
        is_aura = Convert.ToBoolean(_is_aura);

        int.TryParse(_effect.aura_priority, out aura_priority);

        int _is_aura_friendly = 666;
        int.TryParse(_effect.is_aura_friendly, out _is_aura_friendly);
        is_aura_friendly = Convert.ToBoolean(_is_aura_friendly);

        int _is_aura_enemy = 666;
        int.TryParse(_effect.is_aura_enemy, out _is_aura_enemy);
        is_aura_enemy = Convert.ToBoolean(_is_aura_enemy);

        int _destroy_target = 666;
        int.TryParse(_effect.destroy_target, out _destroy_target);
        destroy_target = Convert.ToBoolean(_destroy_target);

        int _return_target_to_hand = 666;
        int.TryParse(_effect.return_target_to_hand, out _return_target_to_hand);
        return_target_to_hand = Convert.ToBoolean(_return_target_to_hand);

        int _search_in_deck = 666;
        int.TryParse(_effect.search_in_deck, out _search_in_deck);
        search_in_deck = Convert.ToBoolean(_search_in_deck);

        int _search_in_graveyard = 666;
        int.TryParse(_effect.search_in_graveyard, out _search_in_graveyard);
        search_in_graveyard = Convert.ToBoolean(_search_in_graveyard);

        int _only_wounded = 666;
        int.TryParse(_effect.only_wounded, out _only_wounded);
        only_wounded = Convert.ToBoolean(_only_wounded);

        int _use_category = 666;
        int.TryParse(_effect.use_category, out _use_category);
        use_category = Convert.ToBoolean(_use_category);

        int _use_sinergy = 666;
        int.TryParse(_effect.use_sinergy, out _use_sinergy);
        use_sinergy = Convert.ToBoolean(_use_sinergy);

        int _creature_only = 666;
        int.TryParse(_effect.creature_only, out _creature_only);
        creature_only = Convert.ToBoolean(_creature_only);

        int _fury = 666;
        int.TryParse(_effect.fury, out _fury);
        fury = Convert.ToBoolean(_fury);

        int _tank = 666;
        int.TryParse(_effect.tank, out _tank);
        tank = Convert.ToBoolean(_tank);

        int _barrier = 666;
        int.TryParse(_effect.barrier, out _barrier);
        barrier = Convert.ToBoolean(_barrier);

        int _haste = 666;
        int.TryParse(_effect.haste, out _haste);
        haste = Convert.ToBoolean(_haste);

        int _death_touch = 666;
        int.TryParse(_effect.death_touch, out _death_touch);
        death_touch = Convert.ToBoolean(_death_touch);

        int _double_hit = 666;
        int.TryParse(_effect.double_hit, out _double_hit);
        double_hit = Convert.ToBoolean(_double_hit);

        int _direct_hit = 666;
        int.TryParse(_effect.direct_hit, out _direct_hit);
        direct_hit = Convert.ToBoolean(_direct_hit);

        int _trample = 666;
        int.TryParse(_effect.trample, out _trample);
        trample = Convert.ToBoolean(_trample);

        int _silence = 666;
        int.TryParse(_effect.silence, out _silence);
        silence = Convert.ToBoolean(_silence);

        int _imprison = 666;
        int.TryParse(_effect.imprison, out _imprison);
        imprison = Convert.ToBoolean(_imprison);

        int _freeze_on_hit = 666;
        int.TryParse(_effect.freeze_on_hit, out _freeze_on_hit);
        freeze_on_hit = Convert.ToBoolean(_freeze_on_hit);

        int _charm = 666;
        int.TryParse(_effect.charm, out _charm);
        charm = Convert.ToBoolean(_charm);

        int _on_hand_effect = 666;
        int.TryParse(_effect.on_hand_effect, out _on_hand_effect);
        on_hand_effect = Convert.ToBoolean(_on_hand_effect);

        int _on_creature_death = 666;
        int.TryParse(_effect.on_creature_death, out _on_creature_death);
        on_creature_death = Convert.ToBoolean(_on_creature_death);

        int _on_summon = 666;
        int.TryParse(_effect.on_summon, out _on_summon);
        on_summon = Convert.ToBoolean(_on_summon);

        int _on_draw = 666;
        int.TryParse(_effect.on_draw, out _on_draw);
        on_draw = Convert.ToBoolean(_on_draw);

        int _on_discard = 666;
        int.TryParse(_effect.on_discard, out _on_discard);
        on_discard = Convert.ToBoolean(_on_discard);

        int _use_ally_count = 666;
        int.TryParse(_effect.use_ally_count, out _use_ally_count);
        use_ally_count = Convert.ToBoolean(_use_ally_count);

        int _use_enemy_count = 666;
        int.TryParse(_effect.use_enemy_count, out _use_enemy_count);
        use_enemy_count = Convert.ToBoolean(_use_enemy_count);

        int _use_player_hand_count = 666;
        int.TryParse(_effect.use_player_hand_count, out _use_player_hand_count);
        use_player_hand_count = Convert.ToBoolean(_use_player_hand_count);

        int _use_enemy_hand_count = 666;
        int.TryParse(_effect.use_enemy_hand_count, out _use_enemy_hand_count);
        use_enemy_hand_count = Convert.ToBoolean(_use_enemy_hand_count);
    }
}

[System.Serializable]
public class Effect
{
    public string id {get;set;}
    public string effect_name {get;set;}
    public string description_label {get;set;}
    public string damage {get;set;}
    public string heal {get;set;}
    public string target_number {get;set;}    
    public string plus_marker {get;set;} 
    public string minus_marker {get;set;}    
    public string draw_cards {get;set;}  
    public string self_discard_cards {get;set;}  
    public string oponent_discard_cards {get;set;}   
    public string create_tokens {get;set;}   
    public string token_skin {get;set;}   
    public string turn_counter {get;set;}    
    public string increase_costs {get;set;}  
    public string decrease_costs {get;set;}  
    public string increase_resources {get;set;}
    public string on_cast_effect {get;set;}  
    public string on_death_effect {get;set;}
    public string on_start_turn_effect {get;set;}    
    public string on_start_turn_graveyard {get;set;}    
    public string on_end_turn_effect {get;set;}   
    public string on_hand_effect {get;set;}  
    public string on_creature_death {get;set;}    
    public string use_ally_count {get;set;}    
    public string use_enemy_count {get;set;}    
    public string use_player_hand_count {get;set;}    
    public string use_enemy_hand_count {get;set;}    
    public string effect_id {get;set;}    
    public string card_id {get;set;}    
    public string observe_oponent {get;set;}    
    public string on_summon {get;set;}    
    public string on_draw {get;set;}    
    public string on_discard {get;set;}    
    public string is_aura {get;set;} 
    public string aura_priority {get;set;} 
    public string is_aura_friendly {get;set;}   
    public string is_aura_enemy {get;set;}   
    public string destroy_target {get;set;}
    public string return_target_to_hand {get;set;}  
    public string search_in_deck {get;set;}
    public string search_in_graveyard {get;set;}
    public string only_wounded {get;set;}
    public string discard_self_deck {get;set;}
    public string discard_oponent_deck {get;set;}
    public string use_category {get;set;}
    public string use_sinergy {get;set;}
    public string creature_only {get;set;}
    public string fury {get;set;}
    public string tank {get;set;}
    public string barrier {get;set;}
    public string haste {get;set;}
    public string death_touch {get;set;} 
    public string double_hit {get;set;} 
    public string direct_hit {get;set;}
    public string trample {get;set;}
    public string silence {get;set;}
    public string imprison {get;set;}   
    public string freeze_on_hit {get;set;}
    public string charm {get;set;}
}