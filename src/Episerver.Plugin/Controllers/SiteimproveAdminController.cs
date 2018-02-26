using EPiServer.ServiceLocation;
using EPiServer.Shell;
using System.Web.Mvc;
using EPiServer.PlugIn;
using SiteImprove.EPiserver.Plugin.Core;
using SiteImprove.EPiserver.Plugin.Core.Repositories;

namespace SiteImprove.EPiserver.Plugin.Controllers
{
    [Authorize(Roles = "Administrators, WebAdmins, CmsAdmins, SiteimproveAdmins")]
    [GuiPlugIn(Area = EPiServer.PlugIn.PlugInArea.AdminMenu, Url = "/SiteimproveAdmin", DisplayName = "Siteimprove")]
    public class SiteimproveAdminController : Controller
    {
        private readonly ISettingsRepository _settingsRepo;
        private readonly ISiteimproveHelper _siteimproveHelper;

        public SiteimproveAdminController() : this(ServiceLocator.Current.GetInstance<ISettingsRepository>(), ServiceLocator.Current.GetInstance<ISiteimproveHelper>()) { }
        public SiteimproveAdminController(ISettingsRepository settingsRepo, ISiteimproveHelper siteimproveHelper)
        {
            _settingsRepo = settingsRepo;
            _siteimproveHelper = siteimproveHelper;
        }

        public ActionResult Index(bool newToken = false)
        {
            if (newToken)
            {
                // Create new token
                this._settingsRepo.SaveToken(_siteimproveHelper.RequestToken());
            }

            return View(_siteimproveHelper.GetAdminViewPath("Index"), this._settingsRepo.GetToken() as object);
        }

       
    }
}
