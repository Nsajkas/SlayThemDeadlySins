using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class CyberneticImplant() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override bool IsAllowedInternal(IRunState runState)
    {
        return !(runState.GetPlayer(LocalContext.NetId.Value).Character is Defect);
    }

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains<Creature>(this.Owner.Creature) || this.Owner.PlayerCombatState.TurnNumber > 1)
            return;
        var defectCardPool = ModelDb.CardPool<DefectCardPool>();
        IEnumerable<CardModel> AvailableCards = defectCardPool.GetUnlockedCards(this.Owner.UnlockState, this.Owner.RunState.CardMultiplayerConstraint);
        
        if (AvailableCards.Count() == 0)
            return;
        
        this.Flash(); // test - remove untill propper graphics are introduced
        
        List<CardModel> GeneratedCards = CardFactory.GetDistinctForCombat(this.Owner, (IEnumerable<CardModel>)AvailableCards, 1, this.Owner.RunState.Rng.CombatCardGeneration).ToList<CardModel>();
        foreach (CardModel cardModel in GeneratedCards)
        {
            cardModel.SetToFreeThisCombat();
        }

        IReadOnlyList<CardPileAddResult> res =
            await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>)GeneratedCards, PileType.Hand,
                this.Owner);
    }
}