using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Server: BaseScript
    {
        string serverId;

        public Server() 
        {
            JObject json = JObject.Parse(LoadResourceFile(GetCurrentResourceName(), "config.json"));
            serverId = json["server-id"].ToString();
        }

        [EventHandler("PlayedTime:Server:GetPlayedTime")]
        public void GetPlayedTime([FromSource] Player player)
        {
            string fivem = player.Identifiers["fivem"];

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://lambda.fivem.net/api/ticket/playtimes/" + serverId + "?identifiers[]=fivem:" + fivem);
            webRequest.ServerCertificateValidationCallback = delegate { return true; }; //Probably not the best way to handle this but it works
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";

            try
            {
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();

                responseFromServer = responseFromServer.Trim('[');
                responseFromServer = responseFromServer.Trim(']');

                JObject json = JObject.Parse(responseFromServer);

                int hours = Int32.Parse(json["seconds"].ToString()) / 3600;

                player.TriggerEvent("PlayedTime:Client:ReturnPlayedTime", hours);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }
}
