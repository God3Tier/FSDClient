namespace FSDClient.battlefield.response;

using System.Text.Json;
using System.Text.Json.Serialization;

public class AttackEvent
{
    [JsonPropertyName("attacker_card_id")]
    private int AttackerCardID;
    [JsonPropertyName("attacker_row")]
    private int AttackerRow;
    [JsonPropertyName("attacker_col")]
    private int AttackerCol;
    [JsonPropertyName("target_card_id")]
    private int TargetCardID;
    [JsonPropertyName("target_row")]
    private int TargetRow;
    [JsonPropertyName("target_col")]
    private int TargetCol;
    [JsonPropertyName("damage")]
    private int Damage;
    [JsonPropertyName("counter_damage")]
    private int CounterDamage;
    [JsonPropertyName("target_is_leader")]
    private bool TargetIsLeader;
    
}

public class BoardCardView
{
    [JsonPropertyName("card_id")]
    private int CardID;
    [JsonPropertyName("card_name")]
    private int CardName;
    [JsonPropertyName("colour")]
    private int Colour;
    [JsonPropertyName("current_health")]
    private int CurrentHealth;
    [JsonPropertyName("max_health")]
    private int MaxHealth;
    [JsonPropertyName("card_attack")]
    private int CardAttack;
    [JsonPropertyName("charge_ticks_remaining")]
    private int ChargeTicksRemaining;
    [JsonPropertyName("charge_ticks_total")]
    private int ChargeTicksTotal;
    [JsonPropertyName("row")]
    private int Row;
    [JsonPropertyName("col")]
    private int Col;
}

public class TickUpdater
{
    [JsonPropertyName("milli_elixer")]
    private int MilliElixir;

    [JsonPropertyName("elixer")]
    private int Elixir;

    [JsonPropertyName("elixer_cap")]
    private int ElixirCap;

    [JsonPropertyName("your_board")]
    private BoardCardView[] YourBoard;

    [JsonPropertyName("enemy_board")]
    private BoardCardView[] EnemyBoard;

    [JsonPropertyName("your_hp")]
    private int YourHp;

    [JsonPropertyName("enemy_hp")]
    private int EnemyHp;

    [JsonPropertyName("leader_atck")]
    private int LeaderAtk;
    
    [JsonPropertyName("phase")]
    private string Phase;
    
    [JsonPropertyName("round_number")]
    private int RoundNumber;
    
    [JsonPropertyName("attack_log")]
    public  AttackEvent[] AttackEvent;
}
