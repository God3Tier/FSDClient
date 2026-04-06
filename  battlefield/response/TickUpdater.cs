namespace FSDClient.battlefield.response;

using System.Text.Json;
using System.Text.Json.Serialization;

public class HandCardView
{
    [JsonPropertyName("card_id")]
    public int CardID { get; set; }
    [JsonPropertyName("card_name")]
    public string CardName { get; set; }
    [JsonPropertyName("colour")]
    public string Colour { get; set; }
    [JsonPropertyName("current_health")]
    public int CurrentHealth { get; set; }
    [JsonPropertyName("max_health")]
    public int MaxHealth { get; set; }
    [JsonPropertyName("card_attack")]
    public int CardAttack { get; set; }
    [JsonPropertyName("charge_ticks_remaining")]
    public int ChargeTicksRemaining { get; set; }
    [JsonPropertyName("charge_ticks_total")]
    public int ChargeTicksTotal { get; set; }
    [JsonPropertyName("row")]
    public int Row { get; set; }
    [JsonPropertyName("col")]
    public int Col { get; set; }
}

public class AttackEvent
{
    [JsonPropertyName("attacker_id")]
    public long AttackerId { get; set; }
    [JsonPropertyName("attacker_card_id")]
    public int AttackerCardID { get; set; }
    [JsonPropertyName("attacker_row")]
    public int AttackerRow { get; set; }
    [JsonPropertyName("attacker_col")]
    public int AttackerCol { get; set; }
    [JsonPropertyName("target_card_id")]
    public int TargetCardID { get; set; }
    [JsonPropertyName("target_row")]
    public int TargetRow { get; set; }
    [JsonPropertyName("target_col")]
    public int TargetCol { get; set; }
    [JsonPropertyName("damage")]
    public int Damage { get; set; }
    [JsonPropertyName("counter_damage")]
    public int CounterDamage { get; set; }
    [JsonPropertyName("target_is_leader")]
    public bool TargetIsLeader { get; set; }

}

// public class BoardCardView
// {
//     [JsonPropertyName("card_id")]
//     public int CardID { get; set; }
//     [JsonPropertyName("card_name")]
//     public int CardName { get; set; }
//     [JsonPropertyName("colour")]
//     public int Colour { get; set; }
//     [JsonPropertyName("current_health")]
//     public int CurrentHealth { get; set; }
//     [JsonPropertyName("max_health")]
//     public int MaxHealth { get; set; }
//     [JsonPropertyName("card_attack")]
//     public int CardAttack { get; set; }
//     [JsonPropertyName("charge_ticks_remaining")]
//     public int ChargeTicksRemaining { get; set; }
//     [JsonPropertyName("charge_ticks_total")]
//     public int ChargeTicksTotal { get; set; }
//     [JsonPropertyName("row")]
//     public int Row { get; set; }
//     [JsonPropertyName("col")]
//     public int Col { get; set; }
// }

public class BoardCardView
{
    [JsonPropertyName("card_id")]
    public int CardID { get; set; }

    [JsonPropertyName("card_name")]
    public string CardName { get; set; }  // was int, must be string

    [JsonPropertyName("colour")]
    public string Colour { get; set; }    // was int, must be string

    [JsonPropertyName("mana_cost")]       // server sends "mana_cost" not "current_health" etc
    public int ManaCost { get; set; }

    [JsonPropertyName("attack")]
    public int Attack { get; set; }

    [JsonPropertyName("hp")]
    public int Hp { get; set; }

    [JsonPropertyName("row")]
    public int Row { get; set; }

    [JsonPropertyName("col")]
    public int Col { get; set; }
}

public class TickUpdater
{
    [JsonPropertyName("milli_elixer")]
    public int MilliElixir { get; set; }

    [JsonPropertyName("elixer")]
    public int Elixir { get; set; }

    [JsonPropertyName("elixer_cap")]
    public int ElixirCap { get; set; }

    [JsonPropertyName("your_board")]
    public BoardCardView[] YourBoard { get; set; }

    [JsonPropertyName("enemy_board")]
    public BoardCardView[] EnemyBoard { get; set; }

    [JsonPropertyName("your_hp")]
    public int YourHp { get; set; }

    [JsonPropertyName("enemy_hp")]
    public int EnemyHp { get; set; }

    [JsonPropertyName("leader_atck")]
    public int LeaderAtk { get; set; }

    [JsonPropertyName("draw_pile")]
    public BoardCardView[] DrawPile { get; set; }

    [JsonPropertyName("hand")]
    public BoardCardView[] Hand { get; set; }

    [JsonPropertyName("phase")]
    public string Phase { get; set; }

    [JsonPropertyName("round_number")]
    public int RoundNumber { get; set; }

    [JsonPropertyName("winner_id")]
    public int WinnerID { get; set; }

    [JsonPropertyName("attack_log")]
    public AttackEvent[] AttackEvent;
}
