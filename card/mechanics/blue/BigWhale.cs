namespace FSDClient.card.mechanics.blue;

using FSDClient.card.display;

public partial class BigWhale : Card
{
    public void OnDamaged(Card[][] OpponentBoard, Card[][] Board, int damageTaken,  int attackX, int attackY)
    {
        // Yes this means essentially whoever plays Kyogre last will win the reflection insanity
        OpponentBoard[attackX][attackY].OnDamaged(OpponentBoard, Board, damageTaken, attackX, attackY);
        base.OnDamaged(OpponentBoard, Board, damageTaken, attackX, attackY);
    }
}
