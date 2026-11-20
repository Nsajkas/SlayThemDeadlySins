using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class GoldenSpoon() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar("MaxCards", 5), new IntVar("GoldGain", 75)];

    public override async Task AfterObtained()
    {
        List<CardModel> availableUpgradedCards = this.Owner.Deck.Cards.Where<CardModel>((Func<CardModel, bool>) (c => c.IsUpgraded)).ToList<CardModel>();
        if (availableUpgradedCards.Count <= 0)
            return;
        
        decimal numCards = DynamicVars["MaxCards"].BaseValue > availableUpgradedCards.Count  ? availableUpgradedCards.Count : DynamicVars["MaxCards"].BaseValue;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, (int)numCards);
        IEnumerable<CardModel> selected = (await CardSelectCmd.FromDeckGeneric(this.Owner, prefs, (Func<CardModel, bool>)(c => c.IsUpgraded)));

        var numDowngraded = 0;
        foreach (CardModel upgradedCard in selected)
        {
            CardCmd.Downgrade(upgradedCard);
            CardCmd.Preview(upgradedCard, style: CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.3f, 0.5f);
            numDowngraded += 1;
        }
        await PlayerCmd.GainGold((Decimal) this.DynamicVars["GoldGain"].BaseValue*numDowngraded, this.Owner);
    }
}