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
        public override void RegisterEvents()
        {
            int min = int.Parse(ConfigReader.GetInstance().Get("MinimumCapitalFromWorkshop"));
            int max = int.Parse(ConfigReader.GetInstance().Get("MaximumCapitalFromWorkshop"));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, (Action) (()=> this.ChangeWorkshopIncome(min,max)));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
        private void ChangeWorkshopIncome(int min, int max)
        {
            foreach (Workshop w in Hero.MainHero.OwnedWorkshops)
            {
                int changeValue = RandomGoldGenerator.GenerateSum(min, max);
                int increaseValue = ((changeValue * 5) - w.Capital + w.InitialCapital);
                w.ChangeGold(increaseValue);
                //w.ChangeGold(changeValue*5);
            }
        }
    }
}
