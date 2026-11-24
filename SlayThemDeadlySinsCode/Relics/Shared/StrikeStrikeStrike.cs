using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Cards;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class StrikeStrikeStrike() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    private List<CardModel> _availableOptions = [];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromCardWithCardHoverTips<PaperDefend>();

    //doing it once to avoid recalculating - it should never change within once fight
    public List<CardModel> AvailableOptions
    {
        get
        {
            if (!_availableOptions.Any())
            {
                AssertMutable();
                _availableOptions =
                    Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                        .Where((CardModel c) => c.Rarity != CardRarity.Basic && c.Tags.Contains(CardTag.Strike)).ToList();
                _availableOptions.AddRange(ModelDb.CardPool<ColorlessCardPool>().
                    GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint).
                    Where((CardModel c) => c.Tags.Contains(CardTag.Strike)));
            }

            return _availableOptions;
        }
    }

    
    
    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        //TODO: cover the paper defend stuff
        if (card.Rarity != CardRarity.Basic || card.Owner != Owner || card.Pile.Type != PileType.Hand)
            return;

        if (card.Tags.Contains(CardTag.Strike))
        {
            if (!AvailableOptions.Any())
            {
                MainFile.Logger.LogMessage(LogLevel.Error, LogType.Actions, "StrikeStrikeStrike: No available card transformation options", 0);
                return;
            }

            List<CardModel> options = CardFactory.GetDistinctForCombat(Owner, AvailableOptions, 1, Owner.RunState.Rng.CombatCardGeneration).ToList();

            if (options.Count == 0)
                return;
        
            Flash();
        
            CardModel targetCard = options[0];
            if (card.IsUpgraded && targetCard.IsUpgradable)
            {
                CardCmd.Upgrade(targetCard);
            }
            
            await CardCmd.Transform(card, targetCard);
        }
        else if (card.Tags.Contains(CardTag.Defend))
        {
            Flash();

            CardPileAddResult? res = await CardCmd.TransformTo<PaperDefend>(card);
            if (card.IsUpgraded && res.HasValue)
            {
                if (res.Value.cardAdded.IsUpgradable)
                {
                    CardCmd.Upgrade(res.Value.cardAdded);
                }
            }
        }
    }
}