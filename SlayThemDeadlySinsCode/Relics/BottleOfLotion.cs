using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SilentRelicPool))]
public class BottleOfLotion() : SlayThemDeadlySinsRelic
{
    private List<CardModel> _affectedCards;

    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Sly)];
    
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner)
        {
            return Task.CompletedTask;
        }
        if (base.Owner.PlayerCombatState.TurnNumber > 1)
        {
            return Task.CompletedTask;
        }
        
        Flash();
        _affectedCards = PileType.Hand.GetPile(Owner).Cards.
            Where((CardModel c) => c != null && c.Type == CardType.Skill && !c.Keywords.Contains(CardKeyword.Sly)).ToList();

        foreach (CardModel card in _affectedCards)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Sly);
        }
        
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        foreach (CardModel card in _affectedCards)
        {
            CardCmd.RemoveKeyword(card, CardKeyword.Sly);
        }

        return Task.CompletedTask;
    }
}