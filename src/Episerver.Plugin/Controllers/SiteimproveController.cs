using System;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using SiteImprove.EPiserver.Plugin.Repositories;
using System.Net;
using System.Security.Principal;
using System.Web.Mvc;
using EPiServer.Web;

namespace SiteImprove.EPiserver.Plugin.Controllers
{
    [Authorize(Roles = "Administrators, WebAdmins, CmsAdmins, SiteimproveAdmins")]
    public class SiteimproveController : Controller
    {
        private readonly ISettingsRepository _settingsRepo;

        public SiteimproveController() : this(ServiceLocator.Current.GetInstance<ISettingsRepository>()) { }
        public SiteimproveController(ISettingsRepository settingsRepo)
        {
            _settingsRepo = settingsRepo;
        }

        [HttpGet]
        [AllowAnonymous]
        public bool IsAuthorized()
        {
            IPrincipal user = HttpContext.User;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }


            var rolesSplit = new string[] { "Administrators", "WebAdmins", "CmsAdmins", "SiteimproveAdmins" };
            if (rolesSplit.Length > 0 && !rolesSplit.Any(user.IsInRole))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }

            return true;
        }

        public JsonResult Token()
        {
            return Json(_settingsRepo.getToken(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PageUrl(string contentId, string locale)
        {
            var contentRep = ServiceLocator.Current.GetInstance<IContentRepository>();
            var page = contentRep.Get<PageData>(
                new ContentReference(contentId),
                new LanguageSelector(locale));

            if (page != null)
            {
                if (page.CheckPublishedStatus(PagePublishedStatus.Published))
                {
                    var externalUrl = SiteimproveHelper.GetExternalUrl(page);
                    return Json(new {url = externalUrl, isDomain = false}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var currentSiteUrl = SiteDefinition.Current.SiteUrl.ToString();
                    return Json(new {url = currentSiteUrl, isDomain = true}, JsonRequestBehavior.AllowGet);
                }
            }

            return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
        }
    }
}
