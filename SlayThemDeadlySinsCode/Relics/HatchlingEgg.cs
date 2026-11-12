using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class HatchlingEgg() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    public override bool TryModifyCardRewardOptionsLate(Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions)
    {
        if (player != this.Owner || creationOptions.Flags.HasFlag((Enum)CardCreationFlags.NoHookUpgrades))
            return false;
        //upgrade valid cards
        foreach (CardCreationResult createdCard in cardRewardOptions)
        {
            var newCard = createdCard.Card;
            if (newCard.IsUpgradable)
            {
                var cardToAdd = player.RunState.CloneCard(newCard);
                CardCmd.Upgrade(cardToAdd);
                createdCard.ModifyCard(cardToAdd, (RelicModel) this);
            }
        }
        return true;
    }

    public override void ModifyMerchantCardCreationResults(Player player, List<CardCreationResult> cards)
    {
        if(player != this.Owner)
            return;
        //upgrade valid cards
        foreach (CardCreationResult createdCard in cards)
        {
            var newCard = createdCard.Card;
            if (newCard.IsUpgradable)
            {
                var cardToAdd = player.RunState.CloneCard(newCard);
                CardCmd.Upgrade(cardToAdd);
                createdCard.ModifyCard(cardToAdd, (RelicModel) this);
            }
        }
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        newCard = (CardModel) null;
        if (card.Owner != this.Owner || !card.IsUpgradable)
            return false;

        newCard = this.Owner.RunState.CloneCard(card);
        CardCmd.Upgrade(newCard, CardPreviewStyle.None);
        
        return true;
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {

        if (room.RoomType == RoomType.RestSite || room.RoomType == RoomType.Elite)
        {
            RelicCmd.Remove((RelicModel)this);
        }

    }
}