using BaseLib.Patches.Content;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics.Ckitten;

public class FrozenRelicAttribute
{
    public static readonly SavedSpireField<RelicModel, bool> FrozenRelic = new (() => false, "frozen_relic_attribute_flag");
}

[Pool(typeof(SharedRelicPool))]
public class FrozenDagger() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    
    private bool _isActivating = false;
    private int _accumulatedDamage = 0;

    public override bool ShowCounter => !IsUsedUp;
    public override bool IsUsedUp => !HasAnyRemaining(Owner);

    public override int DisplayAmount
    {
        get
        {
            if (!IsActivating)
            {
                return AccumulatedDamage % DynamicVars["Threshold"].IntValue;
            }
            return DynamicVars["Threshold"].IntValue;
        }
    }

    private bool IsActivating
    {
        get
        {
            return _isActivating;
        }
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public int AccumulatedDamage
    {
        get
        {
            return _accumulatedDamage;
        }
        set
        {
            AssertMutable();
            _accumulatedDamage = value;
            InvokeDisplayAmountChanged();
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Relics", 5), new IntVar("Threshold", 15)];
    public static LocString IcyRelicPrefix => new LocString("relics", "FROZEN_DAGGER.icyRelicPrefix");
    
    public override async Task AfterObtained()
    {
        List<Reward> list = new List<Reward>();
        for (int i = 0; i < DynamicVars["Relics"].IntValue; i++)
        {
            RelicModel relicModel = RelicFactory.PullNextRelicForRewardFromFront(Owner).ToMutable();
            FrozenRelicAttribute.FrozenRelic.Set(relicModel, true);
            list.Add(new RelicReward(relicModel, Owner));
        }
        await RewardsCmd.OfferCustom(Owner, list);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (IsUsedUp)
        {
            MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, "Icy Dagger used up; returning", 0);
            return;
        }
        if (CombatManager.Instance.IsInProgress && target == base.Owner.Creature && result.UnblockedDamage > 0)
        {
            AccumulatedDamage += result.UnblockedDamage;
            if (AccumulatedDamage >= DynamicVars["Threshold"].IntValue)
            {
                TaskHelper.RunSafely(DoActivateVisuals());
                RelicModel relicModel = Owner.Relics.FirstOrDefault((RelicModel r) => r != null && FrozenRelicAttribute.FrozenRelic.Get(r));
                //TODO: find a way to localize - add a prefix to relics
                //TODO: find a way to affect relics tint
                if (relicModel != null)
                {
                    await RelicCmd.Remove(relicModel);
                }
                AccumulatedDamage = 0;
            }
        }
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(0.5f);
        IsActivating = false;
    }

    private bool HasAnyRemaining(Player player)
    {
        if (player == null)
        {
            return false;
        }
        int leftCount = player.Relics.Where((RelicModel r) => r != null && FrozenRelicAttribute.FrozenRelic.Get(r))
            .ToList().Count;
        return leftCount > 0;
    }
}