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
            AssertMutable();
            int diff = value - _currentCost;
            _currentCost = value;
            string msg = "Setting energy current cost from " + EnergyCost + " to " + _currentCost + " diff is " + diff;
            MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, msg, 0);
            if (diff > 0)
            {
                EnergyCost.UpgradeBy(diff);
                EnergyCost.FinalizeUpgrade(); 
            }
            else
            {
                MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, "Tried to increase cost to a lower cost", 0);
            }
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("GoldGain", 35), new EnergyVar(CurrentCost), new IntVar("Increase", 1M)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainGold((Decimal) this.DynamicVars["GoldGain"].IntValue, this.Owner);
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
        DynamicVars["GoldGain"].UpgradeValueBy(15);
    }
}