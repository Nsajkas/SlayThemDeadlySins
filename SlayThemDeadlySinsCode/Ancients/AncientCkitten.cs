using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Models;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics.Ckitten;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Ancients;

public class Ckitten : CustomAncientModel
{
    protected override OptionPools MakeOptionPools => new(
        MakePool(AncientOption<BerserkersAxe>(), AncientOption<StormOfClaws>()),
        MakePool(AncientOption<Fishbone>()),
        MakePool(AncientOption<CatEars>()));

    public override Color DialogueColor => new("C8A2C8");
    //TODO: also set relic choice button color
    
    
    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 2 || act.ActNumber() == 3;
    }
}