using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Ancients;

public class TestAncient : CustomAncientModel
{
    protected override OptionPools MakeOptionPools => new OptionPools(
        MakePool(AncientOption<SharedSuffering>(),
            AncientOption<HatchlingEgg>(),
            AncientOption<RustyRoulette>(),
            AncientOption<CyberneticImplant>())
    );

    public override bool IsValidForAct(ActModel act)
    {
        return act.Index == 2;
    }
}