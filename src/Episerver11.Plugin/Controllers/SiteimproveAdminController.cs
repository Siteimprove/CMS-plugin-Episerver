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
            var settings = this._settingsRepo.GetSetting();
            if (newToken)
            {
                settings.Token = _siteimproveHelper.RequestToken();
                this._settingsRepo.SaveToken(settings.Token, settings.NoRecheck, settings.ApiUser, settings.ApiKey);
            }

            var vm = new SettingsViewModel()
            {
                Token = settings.Token,
                NoRecheck = settings.NoRecheck,
                ApiUser = settings.ApiUser,
                ApiKey = settings.ApiKey,
                PrepublishCheckEnabled = _siteimproveHelper.GetPrepublishCheckEnabled(settings.ApiUser, settings.ApiKey)
            };
            return View(_siteimproveHelper.GetAdminViewPath("Index"), vm);
        }

        [HttpPost]
        public ActionResult Save(bool noRecheck, string apiUser, string apiKey)
        {
            var settings = this._settingsRepo.GetSetting();
            settings.NoRecheck = noRecheck;
            settings.ApiUser = apiUser;
            settings.ApiKey = apiKey;

            this._settingsRepo.SaveToken(settings.Token, settings.NoRecheck, settings.ApiUser, settings.ApiKey);

            var vm = new SettingsViewModel()
            {
                Token = settings.Token,
                NoRecheck = settings.NoRecheck,
                ApiUser = settings.ApiUser,
                ApiKey = settings.ApiKey,
                PrepublishCheckEnabled = _siteimproveHelper.GetPrepublishCheckEnabled(settings.ApiUser, settings.ApiKey)
            };
            return View(_siteimproveHelper.GetAdminViewPath("Index"), vm);
        }

        [HttpPost]
        public ActionResult EnablePrepublishCheck(bool enablePrepublishCheck = false)
        {
            var settings = this._settingsRepo.GetSetting();

            _siteimproveHelper.EnablePrepublishCheck(settings.ApiUser, settings.ApiKey);

            var vm = new SettingsViewModel()
            {
                Token = settings.Token,
                NoRecheck = settings.NoRecheck,
                ApiUser = settings.ApiUser,
                ApiKey = settings.ApiKey,
                PrepublishCheckEnabled = _siteimproveHelper.GetPrepublishCheckEnabled(settings.ApiUser, settings.ApiKey)
            };
            return View(_siteimproveHelper.GetAdminViewPath("Index"), vm);
        }
    }
}
