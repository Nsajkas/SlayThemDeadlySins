using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics.Ckitten;

[Pool(typeof(SharedRelicPool))]
public class BerserkersAxe() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != base.Owner.Creature)
        {
            return false;
        }
        if (card.Type == CardType.Attack)
        {
            if (modifiedCost > 0)
            {
                modifiedCost = originalCost - 1;
            }
        }
        else
        {
            modifiedCost = originalCost + 1;
        }

        return true;
    }
}