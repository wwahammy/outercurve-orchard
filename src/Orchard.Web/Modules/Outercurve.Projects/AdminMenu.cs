using System.Web.Routing;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;
using Outercurve.Projects.Controllers;

namespace Outercurve.Projects
{
    public class AdminProjectsMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder.Add(T("Agreement Templates"), "1", menu => menu.Action("Index", "CLATemplateAdmin", new { area = "Outercurve.Projects" }).Permission(StandardPermissions.SiteOwner));
            builder.Add(T("Galleries"), "1.1", menu => menu.Action("Index", "GalleryAdmin", new { area = "Outercurve.Projects" }).Permission(StandardPermissions.SiteOwner));
            builder.Add(T("Projects"), "1.2", menu => menu.Action("Index", "ProjectAdmin", new {area = "Outercurve.Projects"}).Permission(StandardPermissions.SiteOwner));
            builder.Add(T("Agreements"), "1.3", menu => menu.Action("Index", "CLAAdmin", new { area = "Outercurve.Projects" }).Permission(StandardPermissions.SiteOwner));
            
        }
    }
}