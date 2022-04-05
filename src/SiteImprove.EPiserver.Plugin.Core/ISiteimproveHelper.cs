﻿using EPiServer.Core;

namespace SiteImprove.EPiserver.Plugin.Core
{
    public interface ISiteimproveHelper
    {
        string GetVersion();
        string RequestToken();
        void PassEvent(string type, string url, string token);
        string GetAdminViewPath(string viewName);
        string GetExternalUrl(PageData page);
        bool GetPrepublishCheckEnabled(string apiUser, string apiKey);
        bool EnablePrepublishCheck(string apiUser, string apiKey);
    }
}