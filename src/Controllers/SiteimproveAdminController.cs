using EPiServer.ServiceLocation;
using EPiServer.Shell;
using SiteImprove.EPiserver.Plugin.Repositories;
using System.Web.Mvc;

namespace SiteImprove.EPiserver.Plugin.Controllers
{
    [Authorize(Roles = "Administrators, WebAdmins, CmsAdmins")]
    [EPiServer.PlugIn.GuiPlugIn(Area = EPiServer.PlugIn.PlugInArea.AdminMenu,
        Url = "/SiteimproveAdmin",
        DisplayName = "Siteimprove"
        )]
    public class SiteimproveAdminController : Controller
    {
        private readonly ISettingsRepository _settingsRepo;

        public SiteimproveAdminController() : this(ServiceLocator.Current.GetInstance<ISettingsRepository>()) { }
        public SiteimproveAdminController(ISettingsRepository settingsRepo)
        {
            _settingsRepo = settingsRepo;
        }

        public ActionResult Index(bool newToken = false)
        {
            if (newToken)
            {
                // Create new token
                this._settingsRepo.saveToken(SiteimproveHelper.RequestToken());
            }

            return View(SiteimproveHelper.GetAdminViewPath("Index"), this._settingsRepo.getToken() as object);
        }

       
    }
}
