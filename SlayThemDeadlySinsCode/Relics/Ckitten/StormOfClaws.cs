using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics.Ckitten;

[Pool(typeof(SharedRelicPool))]
public class StormOfClaws() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;

    protected override bool RelicAllowedToSpawn(Player owner)
    {
        IReadOnlyList<CardModel> cards = owner.Deck.Cards;

        return (cards.Count((CardModel c) => c is Claw) > 0);
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is Claw)
        {
            foreach (var card in CardPile.GetCards(Owner, PileType.Draw).Where((c => c is Claw)).ToList())
            {
                card.BaseReplayCount += 1;
            }
        }
    }
}