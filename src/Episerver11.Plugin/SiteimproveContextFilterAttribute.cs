using System.Web.Mvc;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace SiteImprove.EPiserver11.Plugin
{
    public class SiteimproveContextFilterAttribute : ActionFilterAttribute
    {
        private readonly IQueryParameterResolver _queryParameterResolver;

        public SiteimproveContextFilterAttribute() : this(ServiceLocator.Current.GetInstance<IQueryParameterResolver>()) { }
        public SiteimproveContextFilterAttribute(IQueryParameterResolver queryParameterResolver)
        {
            _queryParameterResolver = queryParameterResolver;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string siteimproveContext = _queryParameterResolver.GetParameterValue("siteimprovecontext");
            if (!string.IsNullOrEmpty(siteimproveContext))
            {
                filterContext.RequestContext.SetContextMode(ContextMode.Default);
            }
        }
    }
}