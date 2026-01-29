using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using static Godot.GD;

namespace Client.data;

public class PlayerData
{
    private string Username { get; set; }
    private readonly System.Threading.Mutex Mutex = new();
    private readonly System.Threading.CancellationTokenSource _ctx = new();
    public int Elixer { get; private set; } = 0;
    private string IconName { get; set; }
    private Sprite2D Sprite2D { get; }
    private int Health { get; set; } = 250;
    private int Attack { get; } = 5;
    private List<CardData> CurrHand { get; } = new();
    private List<CardData> DeckCardDatas { get; }

    public PlayerData(string username, string iconName, List<CardData> deckCardDatas)
    {
        Username = username;
        IconName = iconName;
        DeckCardDatas = deckCardDatas;
        Sprite2D = new Sprite2D();
        // TODO change this to the actual directory
        // var texture2D = ResourceLoader.Load<Texture2D>(iconName);
        // Sprite2D.Texture = texture2D;

        // YES FOR SURE THIS IS A GOOD IDEA
        Task.Run(() =>
        {
            while (!_ctx.IsCancellationRequested)
            {
                Thread.Sleep(200);
                SyncIncreaseElixer();
            }

        });
    }

    // This function should be thrown in a thread and forgotten later
    private async void SyncIncreaseElixer()
    {
        Mutex.WaitOne();
        if (Elixer <= 7)
        {
            Elixer += 1;
        }
        Mutex.ReleaseMutex();
    }


    public void SyncRemoveElixer(int cardCost)
    {
        Mutex.WaitOne();
        if (Elixer < cardCost)
        {
            Mutex.ReleaseMutex();
            // Throw some sort of Exception
            return;
        }
        else
        {
            Elixer -= cardCost;
        }
        Mutex.ReleaseMutex();
    }




    public void EndGame()
    {
        _ctx.Cancel();
    }

}
