using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Cards;

[Pool(typeof(ColorlessCardPool))]
public class IronWaveveveve() : SlayThemDeadlySinsCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [ new HitCountVar(3),  
                                                                new DamageVar(1, ValueProp.Move), 
                                                                new BlockVar(1, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        
        await DamageCmd.Attack(DynamicVars.Damage.IntValue).
            FromCard(this, cardPlay).
            WithHitCount(DynamicVars.HitCount.IntValue).
            Targeting(cardPlay.Target).
            AfterAttackerAnim(async delegate { await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, cardPlay);}). //hacky afffff
            WithHitFx("vfx/vfx_flying_slash").
            Execute(choiceContext);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.HitCount.UpgradeValueBy(2);
    }
}