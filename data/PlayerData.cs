using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using static Godot.GD;

namespace FSDClient.data;

public class PlayerData
{
    private string Username { get; set; }
    
    // TODO: Not sure whether this is standard to keep it individual or to create an abstracted class
    //       that acts as a wrapper around the data but ts looks ugly
    private readonly System.Threading.Mutex Mutex = new();
    public int Elixer { get; private set; } = 0;
    private readonly System.Threading.CancellationTokenSource _ctx = new();
    private string IconName { get; set; }
    private bool MainPlayer { get; set; }
    private Sprite2D Sprite2D { get; }
    // Numbers to be confirmed. Will place a proper named thing once everything has been sorted out 
    private int Health { get; set; } = 250;
    private int Attack { get; } = 5;
    public List<CardData> CurrHand { get; } = new();
    private List<CardData> DeckCardDatas { get; }

    public PlayerData(string username, string iconName, List<CardData> deckCardDatas, bool mainPlayer)
    {
        Username = username;
        IconName = iconName;
        DeckCardDatas = deckCardDatas;
        MainPlayer = mainPlayer;
        
        
        Sprite2D = new Sprite2D();
        // TODO change this to the actual directory
        // var texture2D = ResourceLoader.Load<Texture2D>(iconName);
        // Sprite2D.Texture = texture2D;
        
    }

    public void SpawnElixer()
    {
        if (!MainPlayer)
        {
            return;
        }

        Task.Run(() =>
        {
            while (!_ctx.IsCancellationRequested)
            {
                // TODO: Numbers unconfirmed please try again soon
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
