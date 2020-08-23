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
        ///Approximation for realtime and ingame difference in gold
        private int INGAME_VALUE_INCREMENT = 5;
        /// <summary>
        ///Function for reading configuration parameters and adding (calling) Daily tick function for changing workshop income. 
        /// </summary>
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
        /// <summary>
        /// Function for changing workshop income
        /// </summary>
        /// <param name="min">Minimum acceptable workshop income</param>
        /// <param name="max">Maximum acceptable workshop income</param>
        /// <param name="boundVillageIncrement">Bonus for village production</param>
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
        /// <summary>
        /// Change workshop income
        /// </summary>
        /// <param name="w">Workshop</param>
        /// <param name="v">integer for changing workshop income</param>
        private void ChangeWorkshopIncomeValue(Workshop w, int v)
        {
            w.ChangeGold(v);
        }
        /// <summary>
        /// Function for checking if any village has the same production output as chosen workshop has inputs.
        /// </summary>
        /// <param name="w">Workshop of interest</param>
        /// <returns>True if exists, any other case returns False</returns>
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
