using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Localization;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;

namespace VariantWorkshopIncome
{
    public class Main : MBSubModuleBase
    {
       
        protected override void OnGameStart(Game game,IGameStarter gameStarter)
        {
            if (isGameCampaign(game))
                AddGameWorkerToBehavior(gameStarter);
            StartTheGame(game);
        }
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            InformationManager.DisplayMessage(new InformationMessage("Variant Workshop Income v1.0.1 by m1g4c"));
        }

        private void StartTheGame(Game game)
        {
            this.BeginGameStart(game);
        }

        private void AddGameWorkerToBehavior(IGameStarter gameStarter)
        {
            ((CampaignGameStarter)gameStarter).AddBehavior((CampaignBehaviorBase)new WorkshopIncomeChanger());
        }

        private bool isGameCampaign(Game game)
        {
            return game.GameType is Campaign ? true : false;
        }
    }
}
