using System.Web.Routing;
using System.Web.UI.WebControls;
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
            builder.AddImageSet("projects")
                .Add(T("Agreement Templates"), "1", menu => menu.Action("Index", "CLATemplateAdmin", new { area = "Outercurve.Projects" }).Permission(ProjectPermissions.ModifyAgreementTemplates))
                .Add(T("Galleries"), "1.1", menu => menu.Action("Index", "GalleryAdmin", new { area = "Outercurve.Projects" }).Permission(ProjectPermissions.ModifyGalleries))
                .Add(T("Projects"), "1.2", menu => menu.Action("Index", "ProjectAdmin", new {area = "Outercurve.Projects"}).Permission(ProjectPermissions.ModifyProjects))
                .Add(T("Agreements"), "1.3", menu => menu.Action("Index", "CLAAdmin", new { area = "Outercurve.Projects" }).Permission(ProjectPermissions.ModifyCLAs));

            builder.AddImageSet("users")
                .Add(T("Quick Add Users"), "11.1", 
                        menu => menu.Action("Create", "QuickUserAdmin", new {area = "Outercurve.Projects"}).Permission(ProjectPermissions.QuickCreateUser));

        }
    }
}