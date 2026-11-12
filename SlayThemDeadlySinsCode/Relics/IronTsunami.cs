using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Cards;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class IronTsunami() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon; //TODO: move to one of the ancients

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Count", 2M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromCardWithCardHoverTips<IronWaveveveve>();

    public override async Task AfterObtained()
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 2)
            { Cancelable = false, RequireManualConfirmation = true };

        List<CardTransformation> transformations = (await CardSelectCmd.FromDeckForTransformation(base.Owner, prefs, (CardModel c) 
            => new CardTransformation(c, CreateIronWaveveveveFromoriginal(c, isForPreview: true)))).Select((CardModel original) 
            => new CardTransformation(original, CreateIronWaveveveveFromoriginal(original, isForPreview: false))).ToList();
        
        await CardCmd.Transform(transformations, base.Owner.PlayerRng.Transformations);
    }

    private CardModel CreateIronWaveveveveFromoriginal(CardModel original, bool isForPreview)
    {
        CardModel cardModel = (isForPreview ? ModelDb.Card<IronWaveveveve>().ToMutable() : base.Owner.RunState.CreateCard<IronWaveveveve>(base.Owner));
        if (original.IsUpgraded && cardModel.IsUpgradable)
        {
            if (isForPreview)
            {
                cardModel.UpgradeInternal();
            }
            else
            {
                CardCmd.Upgrade(cardModel);
            }
        }
        if (original.Enchantment != null)
        {
            EnchantmentModel enchantmentModel = (EnchantmentModel)original.Enchantment.MutableClone();
            if (enchantmentModel.CanEnchant(cardModel))
            {
                if (isForPreview)
                {
                    cardModel.EnchantInternal(enchantmentModel, enchantmentModel.Amount);
                    enchantmentModel.ModifyCard();
                }
                else
                {
                    CardCmd.Enchant(enchantmentModel, cardModel, enchantmentModel.Amount);
                }
            }
        }
        return cardModel;
    }
}