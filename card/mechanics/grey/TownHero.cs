namespace FSDClient.card.mechanice.grey;

using FSDClient.card.display;
using FSDClient.battlefield.handManagement;
using Godot;

public partial class TownHero : Card
{

    public void SpawnCard(Card[][] OpponentBoard, Card[][] Board, BattleSlot battleslot)
    {
        GD.Print("Updating Board");
        int amountGrey = 0;

        for (int i = 0; i < Board.Length; i++)
        {
            for (int j = 0; j < Board[i].Length; j++)
            {
                if (Board[i][j] != null && Board[i][j].Colour == "grey")
                {
                    Board[i][j] = null;
                    amountGrey += 1;
                }
            }
            ;
        }

        battleslot.Card.Attack += amountGrey * 5;
        ((RichTextLabel)battleslot.Card.FindChild("Attack", true)).Text = battleslot.Card.Attack.ToString();

        battleslot.Card.Health += amountGrey * 5;
        ((RichTextLabel)battleslot.Card.FindChild("Health", true)).Text = battleslot.Card.Health.ToString();

        Board[battleslot.x][battleslot.y] = battleslot.Card;
        battleslot.Card.EnterBattlefield();
    }

}
