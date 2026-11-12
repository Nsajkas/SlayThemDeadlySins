using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Cards;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Cards;

[Pool(typeof(ColorlessCardPool))]
public class DiminishingReturns() : SlayThemDeadlySinsCard(0,
    CardType.Skill, CardRarity.Ancient,
    TargetType.Self)
{
    
    private int _currentCost = 0;
    private int _increasedCost;

    [SavedProperty]
    public int CurrentCost
    {
        get
        {
            return _currentCost;
        }
        set
        {
            //A bit of a hacky way to keep the cost saved
            AssertMutable();
            int diff = value - _currentCost;
            _currentCost = value;
            if (diff > 0)
            {
                EnergyCost.UpgradeBy(diff);
                EnergyCost.FinalizeUpgrade(); 
            }
            else
            {
                MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, "DiminishingReturns: Tried to increase cost to a lower cost", 0);
            }
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("GoldGain", 25), new EnergyVar(CurrentCost), new IntVar("Increase", 1M)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainGold((Decimal) DynamicVars["GoldGain"].IntValue, Owner);
        int intValue = DynamicVars["Increase"].IntValue;

        BuffFromPlay(intValue);
        (DeckVersion as DiminishingReturns)?.BuffFromPlay(1);
    }

    private void BuffFromPlay(int extraCost)
    {
        CurrentCost +=extraCost;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GoldGain"].UpgradeValueBy(20);
    }
}