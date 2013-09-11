using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Outercurve.Projects
{
    public class Routes : IRouteProvider
    {
        

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {

                new RouteDescriptor {
                   
                    Priority = 100000,
                    Route = new Route("", new RouteValueDictionary {
                        {"area", "Outercurve.Projects"},
                        {"controller", "Index"},
                        {"action", "Index"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary {
                        {"area", "Outercurve.Projects"}
                    },
                        
                    new MvcRouteHandler()
                )

                },
                new RouteDescriptor {
                    Route = new Route(
                        "CLA/{action}",
                        new RouteValueDictionary {
                            {"area", "Outercurve.Projects"},
                            {"controller", "CLA"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Outercurve.Projects"}
                        },
                        new MvcRouteHandler()
                        )
                        },
                new RouteDescriptor {
                    Route = new Route(
                        "CLA/{action}/{id}",
                        new RouteValueDictionary {
                            {"area", "Outercurve.Projects"},
                            {"controller", "CLA"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Outercurve.Projects"}
                        },
                        new MvcRouteHandler()
                        )
                        },

               new RouteDescriptor {
                    Route = new Route(
                        "Signing",
                        new RouteValueDictionary {
                            {"area", "Outercurve.Projects"},
                            {"controller", "CLASigning"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Outercurve.Projects"}
                        },
                        new MvcRouteHandler()
                        )
                        },
                new RouteDescriptor {
                    Route = new Route(
                        "Signing/{action}",
                        new RouteValueDictionary {
                            {"area", "Outercurve.Projects"},
                            {"controller", "CLASigning"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Outercurve.Projects"}
                        },
                        new MvcRouteHandler()
                        )
                        },
                new RouteDescriptor
                { Route= new Route (
                    "Contributor",
                   new RouteValueDictionary {
                            {"area", "Outercurve.Projects"},
                            {"controller", "Contributor"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Outercurve.Projects"}
                        },
                        new MvcRouteHandler()
                        )
                   },

                    new RouteDescriptor
                { Route= new Route (
                    "Contributor/{action}",
                   new RouteValueDictionary {
                            {"area", "Outercurve.Projects"},
                            {"controller", "Contributor"},
                            
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Outercurve.Projects"}
                        },
                        new MvcRouteHandler()
                        )
                   },
            };

        }
    }
}