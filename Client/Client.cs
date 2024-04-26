using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Client: BaseScript
    {
        public Client() 
        {
            RegisterCommand("played", new Action(() => { TriggerServerEvent("PlayedTime:Server:GetPlayedTime"); }), false);
        }

        [EventHandler("PlayedTime:Client:ReturnPlayedTime")]
        private void HandleRecievePlayedTime(int hours)
        {
            ShowNotification($"You have ~g~{hours}~s~ hours played on this server.");
        }

        private void ShowNotification(string message)
        {
            BeginTextCommandThefeedPost("STRING");
            AddTextComponentString(message);
            EndTextCommandThefeedPostMessagetext("CHAR_CALL911", "CHAR_CALL911", false, 4, "~y~Player Information", "~b~Hours Played");
            EndTextCommandThefeedPostTicker(false, false);
        }
    }
}
