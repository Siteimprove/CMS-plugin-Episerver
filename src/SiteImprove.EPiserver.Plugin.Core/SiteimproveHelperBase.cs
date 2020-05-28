using System;
using System.Net.Http;
using System.Security;
using System.Text;
using EPiServer.Logging;
using Newtonsoft.Json;

namespace SiteImprove.EPiserver.Plugin.Core
{
    public abstract class SiteimproveHelperBase
    {
        private static readonly ILogger _log = LogManager.GetLogger(typeof(SiteimproveHelperBase));
        public abstract string GetVersion();
        public string RequestToken()
        {
            var response = string.Empty;
            try
            { 
                using (var client = new HttpClient())
                {
                    // Request a token from Siteimprove
                    var version = GetVersion();
                    string data = client.GetStringAsync(string.Format("{0}?cms=Episerver-{1}", Constants.SiteImproveTokenUrl, version)).Result;
                    response = JsonConvert.DeserializeObject<dynamic>(data)["token"];
                }
            }
            catch(Exception ex)
            {
                _log.Error("An error occured requesting token. perhaps its a wrong version - a version is required for generating a token ", ex);
            }
            return response;
        }

        public void PassEvent(string type, string url, string token)
        {
            var data = new { url, type, token };
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    var response = client.PostAsync(Constants.SiteImproveRecheckUrl, content).Result;
                }
            }
            catch(Exception ex)
            {
                _log.Error(string.Format("An error occured requesting {0}. perhaps token is missing? - A token are required for this operation ",  type), ex);
            }
        }
    }
}
