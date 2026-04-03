namespace FSDClient.card.mechanics.green;

using FSDClient.card.display;
using System;

public partial class SwampOgre : Card
{

    private bool bounceAll = false;

    public void LoadDataTexture(CardViewTextures cardViewTextures)
    {
        if (Random.Shared.Next(1, 2049) == 2048)
        {
            bounceAll = true;
        }
        base.LoadDataTexture(cardViewTextures);
    }

    public void AttackOpponent(Card[][] OpponentBoard, Card[][] Board, ref int player1Health, ref int player2Health)
    {
        
        if (bounceAll) {
            // Perform bounce card action. Will settle once Jared finishes his part
        }

        base.AttackOpponent(OpponentBoard, Board, ref player1Health, ref player2Health);
    }
}
