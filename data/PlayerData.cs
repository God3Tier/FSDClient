using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Client.data;

public class PlayerData
{
    private string Username { get; set; }
    private readonly System.Threading.Mutex Mutex = new();
    private readonly System.Threading.CancellationTokenSource _ctx = new();
    private int Elixer = 0;
    private string IconName { get; set; }
    private Sprite2D Sprite2D { get; } = new();
    private int Health { get; set; } = 250;
    private int Attack { get; } = 5;
    private List<CardData> CurrHand { get; } = new();
    private List<CardData> DeckCardDatas { get; }

    public PlayerData(string username, string iconName, List<CardData> deckCardDatas)
    {
        Username = username;
        IconName = iconName;
        DeckCardDatas = deckCardDatas;
        // TODO change this to the actual directory
        var texture2D = ResourceLoader.Load<Texture2D>(iconName);
        Sprite2D.Texture = texture2D;
        
        // YES FOR SURE THIS IS A GOOD IDEA 
        Task.Run(() =>
        {
            if (_ctx.IsCancellationRequested)
            {
                return;
            }
            syncIncreaseElixer();
        });
    }

    // This function should be thrown in a thread and forgotten later
    private async void syncIncreaseElixer()
    {
        Mutex.WaitOne();
        if (Elixer <= 7)
        {
            Elixer += 1;
        }
        Mutex.ReleaseMutex();
    }


    public void syncRemoveElixer(int cardCost)
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
    
    


    public void endGame()
    {
        _ctx.Cancel();
    }

}
