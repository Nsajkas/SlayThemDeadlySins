using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Cards;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Cards;

[Pool(typeof(TokenCardPool))]
public class Hairball() : SlayThemDeadlySinsCard(0,
    CardType.Attack, CardRarity.Token,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override int MaxUpgradeLevel => 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_bloody_impact");
        int hpLoss = Owner.Creature.CurrentHp / 3;
        int enemyHpLoss = Owner.Creature.MaxHp / 3;
        DamageVar selfDmg = new DamageVar("self_damage", hpLoss, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, selfDmg, this, play);
        
        DamageVar enemyDmg = new DamageVar("enemy_damage", enemyHpLoss, ValueProp.Move);
        await DamageCmd.Attack(enemyDmg.BaseValue).FromCard(this, play).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {

    }
}