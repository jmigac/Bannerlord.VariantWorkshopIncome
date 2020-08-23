using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace VariantWorkshopIncome
{
    class WorkshopIncomeChanger : CampaignBehaviorBase
    {
        private int INGAME_VALUE_INCREMENT = 5;
        public override void RegisterEvents()
        {
            int min = int.Parse(ConfigReader.GetInstance().Get("MinimumCapitalFromWorkshop"));
            int max = int.Parse(ConfigReader.GetInstance().Get("MaximumCapitalFromWorkshop"));
            int boundVillageIncrement = int.Parse(ConfigReader.GetInstance().Get("BoundVillageWorkshopIncrement"))* INGAME_VALUE_INCREMENT;
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, (Action) (()=> this.ChangeWorkshopIncome(min,max,boundVillageIncrement)));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
        private void ChangeWorkshopIncome(int min, int max, int boundVillageIncrement)
        {
            foreach (Workshop w in Hero.MainHero.OwnedWorkshops)
            {
                bool status = CheckIfBoundVillagesCreatesMaterial(w);
                int changeValue = RandomGoldGenerator.GenerateSum(min, max);
                int increaseValue = ((changeValue * INGAME_VALUE_INCREMENT) - w.Capital + w.InitialCapital);
                if (status)
                {
                    ChangeWorkshopIncomeValue(w, increaseValue + boundVillageIncrement);
                }
                else
                {
                    ChangeWorkshopIncomeValue(w, increaseValue);
                }
                    
            }
        }
        private void ChangeWorkshopIncomeValue(Workshop w, int v)
        {
            w.ChangeGold(v);
        }

        private bool CheckIfBoundVillagesCreatesMaterial(Workshop w)
        {
            TaleWorlds.CampaignSystem.WorkshopType workshopType = w.WorkshopType;
            TaleWorlds.Library.MBReadOnlyList<Village> boundVillages = w.Settlement.BoundVillages;
            foreach (Village v in boundVillages)
            {
                IReadOnlyList<TaleWorlds.CampaignSystem.WorkshopType.Production> workShopProductions = workshopType.Productions;
                IReadOnlyList<(ItemObject, float)> villageProductions = v.VillageType.Productions;
                TaleWorlds.Localization.TextObject villageOutput = v.VillageType.Productions[0].Item1.ItemCategory.GetName();
                TaleWorlds.Localization.TextObject workshopInput = workshopType.Productions[0].Inputs[0].Item1.GetName();
                if (villageOutput.ToString() == workshopInput.ToString())
                {
                    return true;
                }
                    
            }
            return false;
        }
    }
}
