namespace FSDClient.card;

using Godot;
using System;

using FSDClient.builder;
using FSDClient.resource;
using FSDClient.autoLoad;
using FSDClient.card.display;
using System.Collections.Generic;
using FSDClient.card.mechanics.grey;
using FSDClient.card.mechanics.red;
using FSDClient.card.mechanics.blue;
using FSDClient.card.mechanics.green;

public class CardBulder
{
    private static readonly CardStatsTable Reference = GD.Load<ResourceManager>("res://resource/ResourceManager.cs").CardStatsTable;
    private static readonly PackedScene packedScene = ResourceLoader.Load<PackedScene>("res://scenes/gameComponents/Card.tscn");
    private static readonly Dictionary<int, Func<PackedScene, Card>> IdToIntialiser = new Dictionary<int, Func<PackedScene, Card>>
    {
        [1] = (packedScene) => packedScene.Instantiate<Pig>(),
        [2] = (packedScene) => packedScene.Instantiate<Farmer>(),
        [3] = (packedScene) => packedScene.Instantiate<Mercenary>(),
        [4] = (packedScene) => packedScene.Instantiate<TownGuard>(),
        [5] = (packedScene) => packedScene.Instantiate<TownHero>(),
        [6] = (packedScene) => packedScene.Instantiate<TravellingMerchant>(),
        [7] = (packedScene) => packedScene.Instantiate<Barbarian>(),
        [8] = (packedScene) => packedScene.Instantiate<Dwarf>(),
        [9] = (packedScene) => packedScene.Instantiate<Bombadier>(),
        [10] = (packedScene) => packedScene.Instantiate<Ninja>(),
        [11] = (packedScene) => packedScene.Instantiate<Apache>(),
        [12] = (packedScene) => packedScene.Instantiate<Dinosaur>(),
        [13] = (packedScene) => packedScene.Instantiate<Penguin>(),
        [14] = (packedScene) => packedScene.Instantiate<ApprenticeMagician>(),
        [15] = (packedScene) => packedScene.Instantiate<KrazyKraken>(),
        [16] = (packedScene) => packedScene.Instantiate<Pufferfish>(),
        [17] = (packedScene) => packedScene.Instantiate<MagicSwordman>(),
        [18] = (packedScene) => packedScene.Instantiate<BigWhale>(),
        [19] = (packedScene) => packedScene.Instantiate<SwampOgre>(),
        [20] = (packedScene) => packedScene.Instantiate<Nymph>(),
        [21] = (packedScene) => packedScene.Instantiate<LivingTree>(),
        [22] = (packedScene) => packedScene.Instantiate<Dryad>(),
        [23] = (packedScene) => packedScene.Instantiate<AlphaWolf>(),
        [24] = (packedScene) => packedScene.Instantiate<Quetzalcoatl>(),
        [37] = (packedScene) => packedScene.Instantiate<Technoblade>(),
        [38] = (packedScene) => packedScene.Instantiate<Wolf>(),
    };
    

    
    public static Card GenerateCard(int cardID)
    {
        var stats = Reference.cardInfo[cardID];
        var card = IdToIntialiser[cardID](packedScene);
        var textures = Builder.BuildCard(stats);
        card.LoadDataTexture(textures);
        return card;
    }
}
