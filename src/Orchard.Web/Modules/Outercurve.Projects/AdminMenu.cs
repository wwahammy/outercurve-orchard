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
            builder.Add(T("Galleries"), "1", menu => menu.Action("Index", "GalleryAdmin", new { area = "Outercurve.Projects" }).Permission(StandardPermissions.SiteOwner));
            builder.Add(T("Projects"), "1", menu => menu.Action("Index", "ProjectAdmin", new { area = "Outercurve.Projects" }).Permission(StandardPermissions.SiteOwner)); 
  
            builder.Add(T("CLAs"), "1", menu => menu.Action("Index", "CLAAdmin", new { area = "Outercurve.Projects" }).Permission(StandardPermissions.SiteOwner));
            
        }
    }
}