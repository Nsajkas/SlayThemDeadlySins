using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Cards;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Cards;

[Pool(typeof(DeprecatedCardPool))] //TODO: change when you find a way to permanently increase cost
public class DiminishingReturns() : SlayThemDeadlySinsCard(0,
    CardType.Skill, CardRarity.Ancient,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("GoldGain", 35)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainGold((Decimal) this.DynamicVars["GoldGain"].IntValue, this.Owner);
        EnergyCost.UpgradeBy(1); //TODO: find a way to set permanently
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GoldGain"].UpgradeValueBy(15);
    }
}