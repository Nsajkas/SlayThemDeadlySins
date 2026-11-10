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

[Pool(typeof(DeprecatedCardPool))] //TODO: change when you find a way to permanently increase cost
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
            _currentCost = value;
            string msg = "Setting energy current cost from " + DynamicVars.Energy.BaseValue + " to " + _currentCost;
            DynamicVars.Energy.BaseValue = _currentCost;
            MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, msg, 0);
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("GoldGain", 35), new EnergyVar(CurrentCost), new IntVar("Increase", 1M)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    [SavedProperty]
    public int IncreasedCost
    {
        get
        {
            return _increasedCost;
        }
        set
        {
            AssertMutable();
            _increasedCost = value;
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PlayerCmd.GainGold((Decimal) this.DynamicVars["GoldGain"].IntValue, this.Owner);
        int intValue = DynamicVars["Increase"].IntValue;

        BuffFromPlay(intValue);
        (DeckVersion as DiminishingReturns)?.BuffFromPlay(intValue);
    }

    private void BuffFromPlay(int extraCost)
    {
        IncreasedCost += extraCost;
        UpdateCost();
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GoldGain"].UpgradeValueBy(15);
    }

    protected override void AfterDowngraded()
    {
        UpdateCost(); 
    }
    
    private void UpdateCost()
    {
        CurrentCost = IncreasedCost;;
    }
}