using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics.Bollo;

[Pool(typeof(SharedRelicPool))]
public class DealersChoice() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Step", 77)];
    public override bool HasUponPickupEffect => true;
    
    //ensure that the ancient does not offer this relic if the player has no gold
    public override async Task AfterObtained()
    {
        PotionRarity potionRarity;
        CardRarity cardRarity;
        RelicRarity relicRarity;

        int goldBefore = Owner.Gold;

        await PlayerCmd.LoseGold(goldBefore, Owner, GoldLossType.Lost);

        int steps = goldBefore / DynamicVars["Step"].IntValue;

        MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, "steps to check against: " + steps, 0);        
        
        switch (steps)
        {
            case 0:
                potionRarity = PotionRarity.Common;
                cardRarity = CardRarity.Common;
                relicRarity = RelicRarity.Common;
                break;
            case 1:
                potionRarity = PotionRarity.Uncommon;
                cardRarity = CardRarity.Common;
                relicRarity = RelicRarity.Common;
                break;
            case 2:
                potionRarity = PotionRarity.Uncommon;
                cardRarity = CardRarity.Uncommon;
                relicRarity = RelicRarity.Common;
                break;
            case 3:
                potionRarity = PotionRarity.Uncommon;
                cardRarity = CardRarity.Uncommon;
                relicRarity = RelicRarity.Uncommon;
                break;
            case 4:
                potionRarity = PotionRarity.Rare;
                cardRarity = CardRarity.Common;
                relicRarity = RelicRarity.Common;
                break;
            case 5:
                potionRarity = PotionRarity.Rare;
                cardRarity = CardRarity.Uncommon;
                relicRarity = RelicRarity.Common;
                break;
            case 6:
                potionRarity = PotionRarity.Rare;
                cardRarity = CardRarity.Uncommon;
                relicRarity = RelicRarity.Uncommon;
                break;
            case 7:
                potionRarity = PotionRarity.Rare;
                cardRarity = CardRarity.Rare;
                relicRarity = RelicRarity.Common;
                break;
            case 8:
                potionRarity = PotionRarity.Rare;
                cardRarity = CardRarity.Rare;
                relicRarity = RelicRarity.Uncommon;
                break;
            default:
                potionRarity = PotionRarity.Rare;
                cardRarity = CardRarity.Rare;
                relicRarity = RelicRarity.Rare;
                break;
        }


        List<PotionModel> potionOptions = ModelDb.AllPotions.Where((PotionModel p) => p.Rarity == potionRarity).ToList();
        int potIndex = Owner.RunState.Rng.Niche.NextInt(potionOptions.Count);
        var pot = potionOptions[potIndex].ToMutable();

        List<RelicModel> relicOptions = ModelDb.AllRelics.
            Where((RelicModel r) => r.Rarity == relicRarity && (r.Pool is SharedRelicPool || r.Pool == Owner.Character.RelicPool)).ToList();
        int relIndex = Owner.RunState.Rng.Niche.NextInt(relicOptions.Count);
        var relic = relicOptions[relIndex].ToMutable();
        
        CardCreationOptions cardCreationOptions = new CardCreationOptions([Owner.Character.CardPool], 
                                                    CardCreationSource.Other, 
                                                    CardRarityOddsType.Uniform, 
                                                    (CardModel c) => c.Rarity == cardRarity);
        List<Reward> rewards = new List<Reward>();
        rewards.Add(new CardReward(cardCreationOptions, 3, Owner));
        rewards.Add(new PotionReward(pot, Owner));
        rewards.Add(new RelicReward(relic, Owner));
        
        
        await Cmd.Wait(0.2f);
        await RewardsCmd.OfferCustom(Owner, rewards);
    }
}