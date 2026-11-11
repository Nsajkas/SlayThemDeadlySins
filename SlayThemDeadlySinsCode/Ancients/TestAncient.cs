using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using SlayThemDeadlySins.SlayThemDeadlySins.images.relics;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Ancients;

public class TestAncient : CustomAncientModel
{
    protected override OptionPools MakeOptionPools => new OptionPools(
        MakePool(AncientOption<SharedSuffering>(),
            AncientOption<HatchlingEgg>(),
            AncientOption<RustyRoulette>(),
            AncientOption<CyberneticImplant>(),
            AncientOption<SellUpgradesRelic>(),
            AncientOption<IronTsunami>(),
            AncientOption<MiscalculatedGamble>(),
            AncientOption<RustyRoulette>())
    );

    public override bool IsValidForAct(ActModel act)
    {
        return false;
    }
}