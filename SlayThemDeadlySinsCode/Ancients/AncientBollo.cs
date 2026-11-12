using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using SlayThemDeadlySins.SlayThemDeadlySins.images.relics;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Ancients;

public class Bollo : CustomAncientModel
{
    private EventOption GildedPeriaptOption => RelicOption<GildedPeriapt>();
    private const int _BIG_WEIGHT = 5;
    
    private static AncientOption AncientOptionConditionally(AncientOption option, bool condition)
    {
        
        
        return condition ? null : option;
    }

    private int GetWeightForGildedPeriapt(Player player)
    {
        if(player == null)
        {
            MainFile.Logger.LogMessage(LogLevel.Error, LogType.Actions, "Bollo: Owner missing from ancient", 0);
            return 0;
        }
        else if (player.Deck == null)
        {
            MainFile.Logger.LogMessage(LogLevel.Error, LogType.Actions, "Bollo: Deck Missing from owner", 0);
            return 0;
        }
        else if (player.Deck.Cards == null)
        {
            MainFile.Logger.LogMessage(LogLevel.Error, LogType.Actions, "Bollo: Deck Cards Missing from owner", 0);
            return 0;
        }

        IReadOnlyList<CardModel> cards = player.Deck.Cards;
        
        if (cards.Count((CardModel c) => c is Greed) > 0)
        {
            MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, "GildedPeriapt: Greed count in player's deck is positive" , 0);
            return _BIG_WEIGHT;
        }

        return 0;
    }

    protected override OptionPools MakeOptionPools => new(
        MakePool( AncientOption<MiscalculatedGamble>()),//first pool is reward-related things
        MakePool(AncientOption<OrigamiUmbrella>()),//second pool is gold/shop-related
        MakePool(AncientOption<RustyRoulette>(), 
            AncientOption<GildedPeriapt>(weight: GetWeightForGildedPeriapt(this.Owner)))); //third pool is misc 

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var OptionsList = base.GenerateInitialOptions();

        // OptionPools.AllOptions.AddItem(GildedPeriaptOption) - maybe like this
        
        return OptionsList;
        
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 2;
    }
}