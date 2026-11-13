using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class OrigamiUmbrella() : SlayThemDeadlySinsRelic
{
    //TODO: test on the beta branch. LordsParasol code has changed, and this might need to be remodelled after that.
    public override RelicRarity Rarity =>
        RelicRarity.Ancient; //Bollo

    // protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("DrawPlus", 2), new IntVar("DrawMinus", 1)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Spend", 90), new IntVar("Count", 3)];

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is MerchantRoom merchantRoom) || Owner.Gold < DynamicVars["Spend"].IntValue)
        {
            return Task.CompletedTask;
        }

        MerchantInventory inventoryFor = RunManager.Instance.MerchantSynchronizer.GetInventoryFor(Owner);
        
        TaskHelper.RunSafely(PurchaseCount(inventoryFor));
        return Task.CompletedTask;
    }

    private async Task PurchaseCount(MerchantInventory inventory)
    {
        if (inventory.Player != base.Owner)
        {
            return;
        }

        bool uiBlocked = false;
        bool waitForCardRemoval = false;
        try
        {
            if (TestMode.IsOff)
            {
                NRun.Instance.GlobalUi.TopBar.Map.Disable();
                NRun.Instance.GlobalUi.TopBar.Deck.Disable();
                NMapScreen.Instance.SetTravelEnabled(false);
                await NRun.Instance.AwaitProcessFrame();
                uiBlocked = true;
                NMerchantRoom.Instance.Inventory.BlockInput();
                await Cmd.Wait(0.75f);
                NMerchantRoom.Instance.Inventory.Open();
                await Cmd.Wait(0.5f);
            }

            int totalNum = 0;
            
            int cardCharTopIndex = inventory.CharacterCardEntries.Count;
            int cardGreyTopIndex = cardCharTopIndex + inventory.ColorlessCardEntries.Count;
            int relicTopIndex = cardGreyTopIndex + inventory.RelicEntries.Count;
            int potionTopIndex = relicTopIndex + inventory.PotionEntries.Count;
            int cardRemovalTopIndex = inventory.CardRemovalEntry == null ? potionTopIndex : potionTopIndex + 1;

            int potionsNum = Owner.PotionSlots.Count - Owner.Potions.Count();

            totalNum = cardRemovalTopIndex;
            
            string ranges_msg = "OrigamiUmbrella: Ranges: character card < " + cardCharTopIndex;
            ranges_msg += "\n grey card < " + cardGreyTopIndex;
            ranges_msg += "\n relic < " + relicTopIndex;
            ranges_msg += "\n potion < " + potionTopIndex;
            ranges_msg += "\n card removal == " + cardRemovalTopIndex;
            
            
            MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, ranges_msg, 0);
            MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, "number of available potion slots is " +potionsNum, 0);
            
            //doing this jank-ass random generation to force usage of the game's RNG. 
            //look into writing a custom RNG that returns a new MerchantEntry instead

            #region RandomGeneration
            
            int[] itemIndexes = new int[3];
            itemIndexes[0] = Owner.RunState.Rng.Niche.NextInt(0, totalNum + 1);
            while ((itemIndexes[0] > relicTopIndex && itemIndexes[0] <= potionTopIndex &&
                     relicTopIndex + potionsNum > itemIndexes[0]))
            {
                itemIndexes[0] = Owner.RunState.Rng.Niche.NextInt(0, totalNum + 1);
                string msg = "Rolled " + itemIndexes[0] + " as index 0";
                MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, msg, 0);
            }
            
            waitForCardRemoval = itemIndexes[0] == cardRemovalTopIndex && inventory.CardRemovalEntry != null;
            if (itemIndexes[0] > relicTopIndex && itemIndexes[0] <= potionTopIndex)
            {
                potionsNum--;
            }

            bool validSecond = false;
            bool validThird = false;

            while (!validSecond)
            {
                itemIndexes[1] = Owner.RunState.Rng.Niche.NextInt(0, totalNum + 1);
                string msg = "Rolled " + itemIndexes[1] + " as index 1";
                MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, msg, 0);
                bool doubleRemoval = itemIndexes[1] == cardRemovalTopIndex && waitForCardRemoval;
                bool equalToFirst = itemIndexes[0] == itemIndexes[1];
                bool invalidPotion = (itemIndexes[1] > relicTopIndex && itemIndexes[1] <= potionTopIndex &&
                                      relicTopIndex + potionsNum > itemIndexes[1]);
                validSecond = !(doubleRemoval || equalToFirst || invalidPotion);
            }
            
            waitForCardRemoval = (itemIndexes[1] == cardRemovalTopIndex && inventory.CardRemovalEntry != null) || waitForCardRemoval;
            if (itemIndexes[1] > relicTopIndex && itemIndexes[1] <= potionTopIndex)
            {
                potionsNum--;
            }
            
            while (!validThird)
            {
                itemIndexes[2] = Owner.RunState.Rng.Niche.NextInt(0, totalNum + 1);
                string msg = "Rolled " + itemIndexes[2] + " as index 2";
                MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, msg, 0);
                bool doubleRemoval = itemIndexes[2] == cardRemovalTopIndex && waitForCardRemoval;
                bool equalToFirst = itemIndexes[0] == itemIndexes[2];
                bool equalToSecond = itemIndexes[1] == itemIndexes[2];
                bool invalidPotion = (itemIndexes[2] > relicTopIndex && itemIndexes[2] <= potionTopIndex &&
                                      relicTopIndex + potionsNum > itemIndexes[2]);
                validThird = !(doubleRemoval || equalToFirst || equalToSecond || invalidPotion);
            }
            
            waitForCardRemoval = (itemIndexes[2] == cardRemovalTopIndex && inventory.CardRemovalEntry != null) || waitForCardRemoval;
            
            
            #endregion
            
            await PlayerCmd.LoseGold(DynamicVars["Spend"].IntValue, Owner, GoldLossType.Spent);

            foreach (int i in itemIndexes)
            {
                if (i < cardCharTopIndex)
                {
                    MerchantCardEntry charCard = inventory.CharacterCardEntries[i];
                    if (!charCard.IsStocked)
                    {
                        string text = charCard.CreationResult?.Card.Id.Entry ?? "NULL";
                        string msg = ("OrigamiUmbrella tried to buy an out-of-stock character card: " + text);
                        MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, msg, 0);
                    }
                    else
                    {
                        await charCard.OnTryPurchaseUnsynchronized(inventory, true);
                        await Cmd.Wait(0.25f);
                    }
                }
                else if (i < cardGreyTopIndex)
                {
                    MerchantCardEntry greyCard = inventory.ColorlessCardEntries[i-cardCharTopIndex];
                    if (!greyCard.IsStocked)
                    {
                        string text = greyCard.CreationResult?.Card.Id.Entry ?? "NULL";
                        string msg = ("OrigamiUmbrella tried to buy an out-of-stock character card: " + text);
                        MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, msg, 0);
                    }
                    else
                    {
                        await greyCard.OnTryPurchaseUnsynchronized(inventory, true);
                        await Cmd.Wait(0.25f);
                    }
                }
                else if (i < relicTopIndex)
                {
                    MerchantRelicEntry relic = inventory.RelicEntries[i - cardGreyTopIndex];
                    if (!relic.IsStocked)
                    {
                        string msg = ("OrigamiUmbrella tried to buy an out-of-stock character relic: " + relic);
                        MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions, msg, 0);
                    }
                    else
                    {
                        NRun.Instance.GlobalUi.TopBar.Map.Enable();
                        NRun.Instance.GlobalUi.TopBar.Deck.Enable();
                        await relic.OnTryPurchaseUnsynchronized(inventory, ignoreCost: true);
                        NRun.Instance.GlobalUi.TopBar.Deck.Disable();
                        NRun.Instance.GlobalUi.TopBar.Map.Disable();
                        await Cmd.Wait(0.25f);
                    }
                }
                else if (i < potionTopIndex)
                {
                    MerchantPotionEntry potion = inventory.PotionEntries[i - relicTopIndex];
                    await potion.OnTryPurchaseUnsynchronized(inventory, true);
                    await Cmd.Wait(0.25f);
                }
            }
        }
        finally
        {
            if (uiBlocked)
            {
                NRun.Instance.GlobalUi.TopBar.Map.Enable();
                NRun.Instance.GlobalUi.TopBar.Deck.Enable();
                NMapScreen.Instance.SetTravelEnabled(true);
                NMerchantRoom.Instance.Inventory.UnblockInput();
                await Cmd.Wait(0.75f);
                NMerchantRoom.Instance.Inventory.Open();
                await Cmd.Wait(0.5f);
            }

            if (waitForCardRemoval)
            {
                if (inventory.CardRemovalEntry != null)
                {
                    NMapScreen.Instance.SetTravelEnabled(false);
                    await inventory.CardRemovalEntry.OnTryPurchaseUnsynchronized(inventory, true);
                    NMapScreen.Instance.SetTravelEnabled(true);
                }
            }
        }
    }
}