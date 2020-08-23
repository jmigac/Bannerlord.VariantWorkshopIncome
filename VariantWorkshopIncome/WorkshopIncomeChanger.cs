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
            int levelBonus = int.Parse(ConfigReader.GetInstance().Get("BonusCreditsPerWorkshopLevel")) * INGAME_VALUE_INCREMENT;
            bool workshopLevelingStatus = ConfigReader.GetInstance().Get("MinimumCapitalFromWorkshop") == "YES" ? true : false;
            int[] workshopBorderLevelCapital = new int[5];
            workshopBorderLevelCapital[0] = int.Parse(ConfigReader.GetInstance().Get("WorkshopLevel1"));
            workshopBorderLevelCapital[1] = int.Parse(ConfigReader.GetInstance().Get("WorkshopLevel2"));
            workshopBorderLevelCapital[2] = int.Parse(ConfigReader.GetInstance().Get("WorkshopLevel3"));
            workshopBorderLevelCapital[3] = int.Parse(ConfigReader.GetInstance().Get("WorkshopLevel4"));
            workshopBorderLevelCapital[4] = int.Parse(ConfigReader.GetInstance().Get("WorkshopLevel5"));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, (Action) (()=> this.ChangeWorkshopIncome(min,max,boundVillageIncrement,levelBonus,workshopBorderLevelCapital,workshopLevelingStatus)));
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
        /// <param name="levelBonus">Level bonus in gold per workshop level</param>
        private void ChangeWorkshopIncome(int min, int max, int boundVillageIncrement,int levelBonus,int[] workshopLevels,bool useWorkshopLeveling)
        {
            foreach (Workshop w in Hero.MainHero.OwnedWorkshops)
            {
                bool status = CheckIfBoundVillagesCreatesMaterial(w);
                int changeValue = RandomGoldGenerator.GenerateSum(min, max);
                int increaseValue = ((changeValue * INGAME_VALUE_INCREMENT) - w.Capital + w.InitialCapital);
                if (status)
                {
                    ChangeWorkshopIncomeValue(w, increaseValue + boundVillageIncrement+(levelBonus*w.Level));
                }
                else
                {
                    ChangeWorkshopIncomeValue(w, increaseValue + (levelBonus * w.Level));
                }
                if(useWorkshopLeveling)
                    UpgradeWorkshop(w,workshopLevels);    
            }
        }
        /// <summary>
        /// Function for upgrading workshop from level 1 till level 4 based on workshop capital.
        /// </summary>
        /// <param name="w">Chosen workshop</param>
        private void UpgradeWorkshop(Workshop w,int[] workshopLevels)
        {
            if (w.Level == 1 && w.Capital >= workshopLevels[0] && w.CanBeUpgraded)
            {
                DisplayMessage($"Workshop in {w.Settlement.Name.ToString()} has been promoted at level 1");
                w.Upgrade();
            }
            else if (w.Level == 2 && w.Capital >= workshopLevels[1] && w.CanBeUpgraded)
            {
                DisplayMessage($"Workshop in {w.Settlement.Name.ToString()} has been promoted at level 2");
                w.Upgrade();
            }
            else if (w.Level == 3 && w.Capital >= workshopLevels[2] && w.CanBeUpgraded)
            {
                DisplayMessage($"Workshop in {w.Settlement.Name.ToString()} has been promoted at level 3");
                w.Upgrade();
            }
            else if (w.Level == 4 && w.Capital >= workshopLevels[3] && w.CanBeUpgraded)
            {
                DisplayMessage($"Workshop in {w.Settlement.Name.ToString()} has been promoted at level 4");
                w.Upgrade();
            }
            else if (w.Level == 5 && w.Capital >= workshopLevels[4] && w.CanBeUpgraded)
            {
                DisplayMessage($"Workshop in {w.Settlement.Name.ToString()} has been promoted at level 5");
                w.Upgrade();
            }
        }
        private void DisplayMessage(string message)
        {
            InformationManager.DisplayMessage(new InformationMessage(message));
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
