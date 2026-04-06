namespace FSDClient.battlefield
{
	// TODO: Need to implement this properly after the demo
	public class AttackEvent
	{
		public int attacker_id { get; set; }
		public int attacker_card_id { get; set; }
		public int attacker_row { get; set; }
		public int attacker_col { get; set; }
		public int target_card_id { get; set; }
		public int target_row { get; set; }
		public int target_col { get; set; }
		public int damage { get; set; }
		public int counter_damage { get; set; }
		public bool target_is_leader { get; set; }
	}
}