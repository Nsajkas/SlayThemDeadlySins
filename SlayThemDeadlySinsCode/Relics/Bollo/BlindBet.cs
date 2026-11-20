using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class BlindBet() : SlayThemDeadlySinsRelic
{
    private List<CardModel> _affectedCards;
    
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar("GainEnergy", 2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Ethereal)];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player != Owner)
        {
            return amount;
        }

        PlayerCombatState? playerCombatState = Owner.PlayerCombatState;
        if(playerCombatState == null || playerCombatState.TurnNumber != 1)
        {
            return amount;
        }

        return amount + DynamicVars["GainEnergy"].BaseValue;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner)
        {
            return Task.CompletedTask;
        }

        if (Owner.PlayerCombatState.TurnNumber != 1)
        {
            return Task.CompletedTask;
        }

        Flash();
        _affectedCards = PileType.Hand.GetPile(Owner).Cards.
            Where((CardModel c) => c != null && !c.Keywords.Contains(CardKeyword.Ethereal)).ToList();

        foreach (CardModel c in _affectedCards)
        {
            c.AddKeyword(CardKeyword.Ethereal);
        }
        
        return Task.CompletedTask;
    }
    
    public override Task AfterCombatEnd(CombatRoom room)
    {
        foreach (CardModel card in _affectedCards)
        {
            CardCmd.RemoveKeyword(card, CardKeyword.Ethereal);
        }
        return Task.CompletedTask;
    }
}