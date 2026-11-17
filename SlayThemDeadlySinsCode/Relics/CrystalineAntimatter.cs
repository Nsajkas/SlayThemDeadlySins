using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SlayThemDeadlySins.SlayThemDeadlySinsCode;
using SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

namespace SlayThemDeadlySins.SlayThemDeadlySinsCode.Relics;

//TODO: find out how to check for dynamic var presence - and how to add if none are present
[Pool(typeof(DeprecatedRelicPool))]
public class CrystalineAntimatter() : SlayThemDeadlySinsRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar("LoseEnergy", 1)];
    private List<CardModel> _affectedCards;

    public override async Task BeforeCombatStart()
    {
        _affectedCards = [];
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player != base.Owner)
        {
            return amount;
        }

        return amount - DynamicVars["LoseEnergy"].BaseValue;
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if(card.Pile == null || card.Pile.Type != PileType.Hand)
            return;
        
        
        if (card.Pool == Owner.Character.CardPool)
        {
            if (_affectedCards.Contains(card))
                return;
                
            _affectedCards.Add(card);
            if (card.DynamicVars.ContainsKey("Energy") && card.CanonicalStarCost != -1)
            {
               
                int tempEnergyCost = card.DynamicVars.Energy.IntValue;
                int tempStarCost = card.BaseStarCost;
                
                card.EnergyCost.SetThisCombat(tempStarCost);
                card.SetStarCostThisCombat(tempEnergyCost);
                
                MainFile.Logger.LogMessage(LogLevel.Info, LogType.Actions,"CrystalineAntimatter: swap nominally triggered", 0);
            }
            
        }
    }
}