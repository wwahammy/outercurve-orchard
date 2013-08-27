using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Outercurve.Projects.Helpers
{
    public class UrlHelperWrapper : UrlHelper, IUrlHelper {
        public UrlHelperWrapper(RequestContext requestContext) : base(requestContext) {
            
        }

        public UrlHelperWrapper(RequestContext requestContext, RouteCollection routeCollection)
            : base(requestContext, routeCollection) {
            
        }

        public UrlHelperWrapper(UrlHelper inner) : base(inner.RequestContext, inner.RouteCollection) {
            
        }
    }

    public interface IUrlHelper {
        string Action(string actionName, string controllerName, Object routeValues, string protocol);
        string Action(string actionName, string controllerName, RouteValueDictionary routeValues);
    }
}