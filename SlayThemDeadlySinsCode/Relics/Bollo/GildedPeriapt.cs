using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Cards;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class GildedPeriapt() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient; //BOLLO

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Count", 2M), new GoldVar("GoldGained", 85)];
    
    public override bool HasUponPickupEffect => true;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromCardWithCardHoverTips<Greed>();

    public override async Task AfterObtained()
    {
        List<CardPileAddResult> results = new List<CardPileAddResult>();
        for (int i = 0; i < DynamicVars["Count"].IntValue; i++)
        {
            CardModel card = Owner.RunState.CreateCard<Greed>(Owner);
            results.Add(await CardPileCmd.Add(card, PileType.Deck));
            MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, "Gilded Periapt loop: i = " + i, 0);
        }
        CardCmd.PreviewCardPileAdd(results, 2f);
        MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, "Gilded Periapt: tried to add " + results.Count + " Greeds to deck", 0);
        MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, "Gilded Periapt: dynamicVars-count is " + DynamicVars["Count"], 0);
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        CardPile? pile = card.Pile;
        if (pile != null && pile.Type == PileType.Deck && card.Owner == Owner && card.Type == CardType.Curse)
        {
            Flash();
            await PlayerCmd.GainGold(DynamicVars["GoldGained"].IntValue, Owner);
        }
    }
}