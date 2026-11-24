using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class SharedSuffering() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon; //potentially move to one of the ancients at some point

    private bool _usedThisCombat;
    
    public bool UsedThisCombat
    {
        get => this._usedThisCombat;
        private set
        {
            if (this._usedThisCombat == value)
                return;
            this.AssertMutable();
            this._usedThisCombat = value;
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (!CombatManager.Instance.IsInProgress || target != this.Owner.Creature || result.UnblockedDamage <= 0 || this.UsedThisCombat)
            return;
        
        this.Flash();
        this._usedThisCombat = true;
        IEnumerable<Creature> everyone = (IEnumerable<Creature>)target.CombatState.GetOpponentsOf(target).Concat(target.CombatState.GetTeammatesOf(target));
        await CreatureCmd.Damage(choiceContext, everyone, (Decimal) result.UnblockedDamage, ValueProp.Unpowered, target);
    }
    
    public override Task AfterCombatEnd(CombatRoom _)
    {
        this.UsedThisCombat = false;
        return Task.CompletedTask;
    }
}