namespace FSDClient.card.mechanics.blue;

using FSDClient.card.display;
using System;


public partial class KrazyKraken : Card
{
    public void AttackOpponent(Card[][] OpponentBoard, Card[][] Board, ref int player1Health, ref int player2Health)
    {
        
        
        
        int randX = Random.Shared.Next(-2, 3);
        int randY = Random.Shared.Next(-2, 2);
        
        // TODO: add the null checker etc

        if (randX < 0)
        {
            randY = randX;
        }
        else if (randY < 0)
        {
            randX = randY;
        }


        if (randX == -2 && randY == -2)
        {
            player2Health -= Attack;
            Health -= 5;
        }
        else if (randX == -1 && randY == -1)
        {
            player1Health -= Attack;
        }
        else if (randX >= 2)
        {
            OpponentBoard[randX - 2][randY].Health -= Attack;
        }
        else
        {
            Board[randX - 2][randY].Health -= Attack;

        }
    }
}
