using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class Backvolver() : SlayThemDeadlySinsRelic
{
    //implement saved value, add it to counter

    private int _relicCounter = 0;
    private bool _isActivating;
    
    public override bool ShowCounter => true;

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int AttacksThrown
    {
        get
        {
            return _relicCounter;
        }
        set
        {
            AssertMutable();
            _relicCounter = value;
            Status = ((decimal)_relicCounter == DynamicVars["AttackAmount"].BaseValue - 1M)
                ? RelicStatus.Active
                : RelicStatus.Normal;
            InvokeDisplayAmountChanged();
        }
    }

    public override int DisplayAmount
    {
        get
        {
            if (!IsActivating)
            {
                return AttacksThrown;
            }

            return DynamicVars["AttackAmount"].IntValue;
        }
    }

    public bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("AttackAmount", 6), new DamageVar(0, ValueProp.Move)];

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (!dealer.IsMonster || target != Owner.Creature)
            return;
        
        if (++AttacksThrown >= DynamicVars["AttackAmount"].BaseValue)
        {
            TaskHelper.RunSafely(DoActivateVisuals());
            DynamicVars.Damage.BaseValue = result.TotalDamage - (decimal)(dealer.HasPower<StrengthPower>() ? dealer.GetPowerAmount<StrengthPower>() : 0); //done to avoid double strength propagation

            await CreatureCmd.Damage(choiceContext, dealer, DynamicVars.Damage, dealer);

            AttacksThrown = 0;
        }
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }
}