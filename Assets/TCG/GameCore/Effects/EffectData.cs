using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Effect", order = 1)]
public class EffectData : ScriptableObject
{
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
    public int discard_self_deck;
    public int discard_oponent_deck;

    public int create_tokens;   
    public string token_skin;

    public int turn_counter;

    public string card_id;

    public bool on_cast;  
    public bool on_death;
    public bool on_start_turn;    
    public bool on_start_turn_graveyard;    
    public bool on_end_turn;
    public bool on_end_turn_graveyard;

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
     
    public bool is_aura_all; 
    public bool is_aura_friendly;   
    public bool is_aura_enemy;
    public int aura_priority; 

    public bool only_wounded;
    public bool creature_only;
    
    public bool destroy_target;
    public bool return_target_to_hand;  
    public bool search_in_deck;
    public bool search_in_graveyard;
    
    public bool use_category;
    public bool use_sinergy;

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

    public GameObject projectilePrefab;
    public GameObject feedbackPrefab;

}