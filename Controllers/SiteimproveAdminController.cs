using EPiServer.ServiceLocation;
using EPiServer.Shell;
using SiteImprove.EPiserver.Plugin.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SiteImprove.EPiserver.Plugin.Controllers
{
    [Authorize(Roles = "Administrators, WebAdmins, CmsAdmins")]
    [EPiServer.PlugIn.GuiPlugIn(Area = EPiServer.PlugIn.PlugInArea.AdminMenu, Url = "/SiteimproveAdmin", DisplayName = "Siteimprove")]
    public class SiteimproveAdminController : Controller
    {
        ISettingsRepository settingsRepo;

        public SiteimproveAdminController() : this(ServiceLocator.Current.GetInstance<ISettingsRepository>()) { }
        public SiteimproveAdminController(ISettingsRepository settingsRepo)
        {
            this.settingsRepo = settingsRepo;
        }

        public ActionResult Index(bool newToken = false)
        {
            if (newToken)
            {
                // Create new token
                this.settingsRepo.saveToken(SiteimproveHelper.RequestToken());
            }

            return View(GetViewPath("Index"), this.settingsRepo.getToken() as object);
        }

        private string GetViewPath(string viewName)
        {
            return Paths.ToClientResource(typeof(SiteimproveAdminController), "Views/Admin/" + viewName + ".cshtml");
        }
    }
}
