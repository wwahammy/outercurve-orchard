using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Settings;
using Orchard.Themes;
using Orchard.UI.Navigation;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;

namespace Outercurve.Projects.Controllers
{

    [Themed]
    public class CLAController : BaseController
    {
      

        
        private readonly IContentManager _contentManager;

        public CLAController(IOrchardServices services, 
            IExtendedUserPartService extUserService, ISiteService siteService, IShapeFactory shapeHelper,
            IContentManager contentManager, IGalleryService galleryService, ITransactionManager transaction): 
            base(extUserService, galleryService, transaction, shapeHelper, services, siteService) {
       
            _contentManager = contentManager;
        }


        public ActionResult Projects(PagerParameters pagerParameters) {
            var query = _services.ContentManager.Query().ForType("Project").OrderBy<TitlePartRecord>( t => t.Title);

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            pager.PageSize = query.Count();

            var pagerShape = Shape.Pager(pager).TotalItemCount(query.Count());

            
            var pageOfItems = query.Slice(0, pager.PageSize).ToList();

            var listShape = Shape.List();
            listShape.AddRange(pageOfItems.Select(item => _contentManager.BuildDisplay(item, "ControllerSummary")));
            listShape.Classes.Add("content-items");
            listShape.Classes.Add("list-items");

            return View((object) listShape);
        }

        public ActionResult Project(int Id) {

            var project = _contentManager.Get(Id);
           
            var query = _services.ContentManager.Query().ForType("CLA").
                Where<CommonPartRecord>(c => c.Container == project.Record).
                OrderBy<CLAPartRecord>(c =>
                   c.LastName).OrderBy<CLAPartRecord>(c => c.FirstName);

            var list = query.List().Select(i => new CLAItemViewModel {FirstName = i.As<CLAPart>().FirstName, LastName = i.As<CLAPart>().LastName, IsValid = i.As<CLAPart>().IsValid}).ToList();


            var projTitle = project.As<TitlePart>().Title;
           
            return View("Project", (object) new CLAProjectViewModel {Items = list, Name = projTitle});
        }

        public ActionResult View(int claId) {
            var cla = _services.ContentManager.Get(claId);
            if (cla.ContentType != "CLA") {
                //bad stuff!!!
            }
            var claPart = cla.As<CLAPart>();

            var model = new ViewCLAViewModel {
                Project = cla.As<CommonPart>().Container.ContentItem,
                FoundationSigner = _extUserService.GetFullName(claPart.FoundationSigner),
                CLASigner = _extUserService.GetFullName(claPart.CLASigner),
               
               // ValidDate = claPart.SignedDate == null ? "Not signed" : ((DateTime)claPart.SignedDate).ToLocalTime().ToString("d")
            };

            return View((object) model);
        }
    }
}