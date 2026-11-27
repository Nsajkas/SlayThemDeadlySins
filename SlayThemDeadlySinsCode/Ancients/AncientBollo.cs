using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using SlayThemDeadlySins.SlayThemDeadlySins.images.relics;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics.Bollo;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Ancients;

public class Bollo : CustomAncientModel
{
    protected override OptionPools MakeOptionPools => new(
        MakePool(AncientOption<MiscalculatedGamble>(),
                            AncientOption<DealersChoice>()),
        MakePool(AncientOption<OrigamiUmbrella>(),
            AncientOption<BlankCheque>(),
            AncientOption<TimeShare>(),
            AncientOption<GildedPeriapt>()),
        MakePool(AncientOption<RustyRoulette>(),
            AncientOption<BlindBet>(),
            AncientOption<GoldenSpoon>()));

    public override Color DialogueColor => new("800020");

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 2;
    }
}