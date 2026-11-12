using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class RustyRoulette() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient; //Bollo
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("DrawPlus", 2), new IntVar("DrawMinus", 1)];

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (this.Owner != player)
            return 0;
        var isPlus = player.RunState.Rng.Niche.NextBool();
        if (isPlus)
        {
            return count + DynamicVars["DrawPlus"].BaseValue;
        }
        else
        {
            return count - DynamicVars["DrawMinus"].BaseValue;
        }
    }
}