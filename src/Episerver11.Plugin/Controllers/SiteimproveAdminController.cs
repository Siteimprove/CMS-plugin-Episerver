using System.Web.Mvc;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using SiteImprove.EPiserver.Plugin.Core;
using SiteImprove.EPiserver.Plugin.Core.Models;
using SiteImprove.EPiserver.Plugin.Core.Repositories;

namespace SiteImprove.EPiserver11.Plugin.Controllers
{
    [Authorize(Roles = "Administrators, WebAdmins, CmsAdmins, SiteimproveAdmins")]
    [GuiPlugIn(Area = PlugInArea.AdminMenu, Url = "/SiteimproveAdmin", DisplayName = "Siteimprove")]
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

            var settings = this._settingsRepo.GetSetting();
            var vm = new SettingsViewModel()
            {
                Token = settings.Token,
                NoRecheck = settings.NoRecheck
            };

            return View(_siteimproveHelper.GetAdminViewPath("Index"), vm);
        }

       [HttpPost]
        public ActionResult Save(string token, bool noRecheck)
        {
            if (string.IsNullOrEmpty(token))
            {
                token = _siteimproveHelper.RequestToken();
            }

            this._settingsRepo.SaveToken(token, noRecheck);
            var vm = new SettingsViewModel()
            {
                Token = token,
                NoRecheck = noRecheck
            };
            return View(_siteimproveHelper.GetAdminViewPath("Index"), vm);
        }
    }
}
