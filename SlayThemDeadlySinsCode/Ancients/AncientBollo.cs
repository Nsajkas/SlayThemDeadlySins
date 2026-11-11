using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using SlayThemDeadlySins.SlayThemDeadlySins.images.relics;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Ancients;

public class Bollo : CustomAncientModel
{
    protected override OptionPools MakeOptionPools => new(
        MakePool( AncientOption<MiscalculatedGamble>()),//first pool is reward-related things
        MakePool(AncientOption<OrigamiUmbrella>()),//second pool is gold/shop-related
        MakePool(AncientOption<RustyRoulette>())); //third pool is misc
    
    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 2;
    }
}