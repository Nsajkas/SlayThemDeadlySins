using BaseLib.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Cards;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySins.images.relics;

[Pool(typeof(SharedRelicPool))]
public class MiscalculatedGamble() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Chance", 13M)];
    //TODO: find a way to add a HoverTip for Purge as well
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromCardWithCardHoverTips<CalculatedGamble>();

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        CardPile? pile = card.Pile;
        if (pile != null && pile.Type == PileType.Deck && card.Owner == Owner)
        {
            var chanceNum = this.Owner.RunState.Rng.Niche.NextInt(DynamicVars["Chance"].IntValue);
            if (chanceNum == 0)
            {
                await CardCmd.Transform(card, CreatePurgingCalculatedGambleFromOriginal(card));
            }
        }
    }

    private CardModel CreatePurgingCalculatedGambleFromOriginal(CardModel original)
    {
        CardModel cardModel = Owner.RunState.CreateCard<CalculatedGamble>(Owner);
        cardModel.AddKeyword(BaseLibKeywords.Purge);
        if (original.IsUpgraded && cardModel.IsUpgradable)
        {
            CardCmd.Upgrade(cardModel);
        }
        if (original.Enchantment != null)
        {
            EnchantmentModel enchantmentModel = (EnchantmentModel)original.Enchantment.MutableClone();
            if (enchantmentModel.CanEnchant(cardModel))
            {
                CardCmd.Enchant(enchantmentModel, cardModel, enchantmentModel.Amount);
            }
        }
        return cardModel;
    }

}