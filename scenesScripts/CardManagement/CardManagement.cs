using Godot;
using System;

public partial class CardManagement : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
		public void _on_back_button_pressed()
	{
		var GameStateManager = GetNode<GameStateManager>("/root/GameStateManager");
		GameStateManager.ChangeGameState(GameState.HOMESCREEN);
	}
	
		public void _on_card_popup_background_pressed()
	{
		close_card_popup();
	}	
	
	public void _on_close_card_button_pressed()
	{
		close_card_popup();
	}	
	
	public void close_card_popup()
	{
		Control CardPopupContainerNode = GetNode<Control>("CardPopupContainer");
		
		// Turn it OFF (make visible false)
		CardPopupContainerNode.Visible = false;
	}	
}
