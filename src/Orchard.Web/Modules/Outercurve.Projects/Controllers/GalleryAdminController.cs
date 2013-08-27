using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;

namespace Outercurve.Projects.Controllers
{
    [Admin]
    public class GalleryAdminController : BaseController, IUpdateModel
    {
      


        public GalleryAdminController(IOrchardServices services, 
            IExtendedUserPartService extUserService, ITransactionManager transaction, IGalleryService galleryService, IShapeFactory shapeFactory, 
            ISiteService siteService) : base(extUserService, galleryService, transaction, shapeFactory, services, siteService)
        {

          

        }

        public ActionResult Index(PagerParameters pagerParameters) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list custom forms")))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);


            var query = _services.ContentManager.Query().ForType("Gallery");
            var pagerShape = Shape.Pager(pager).TotalItemCount(query.Count());
            
            var results = query.Slice(pager.GetStartIndex(), pager.PageSize);
            var model = new GalleryAdminIndexViewModel {
                Galleries = results.Select(x => new GalleryAdminIndexEntry {GalleryItem = x}).ToList(),
                Pager = pagerShape
            };

            return View((object)model);
        }


        public ActionResult Create() {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }
            var gallery = _services.ContentManager.New("Gallery");
            SetDefaults(gallery);
            var model = _services.ContentManager.BuildEditor(gallery);




            SetAllUsers();
            
          
            return View((object) model);

        }

        


        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST() {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }


            var gallery = _services.ContentManager.New("Gallery");
            SetDefaults(gallery);
            _services.ContentManager.Create(gallery);
            var model = _services.ContentManager.UpdateEditor(gallery, this);


            if (!ModelState.IsValid) {
                _transaction.Cancel();
                
                SetAllUsers();
                return View((object) model);
            }

            _services.ContentManager.Publish(gallery);
            _services.Notifier.Add(NotifyType.Information, T("The {0} gallery was created.", (model.ContentItem as ContentItem).As<TitlePart>().Title));
            return RedirectToAction("Index");
        }


        public ActionResult Edit(int id) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }
            SetAllUsers();
            var gallery = _services.ContentManager.Get(id);
            var model = _services.ContentManager.BuildEditor(gallery);

            return View((object) model);
        }

        
        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

            var gallery = _services.ContentManager.Get(id);
            SetDefaults(gallery);
            var model = _services.ContentManager.UpdateEditor(gallery, this);

            if (!ModelState.IsValid) {
                _transaction.Cancel();
                SetAllUsers();
                return View((object) model);
            }

            _services.ContentManager.Publish(gallery);
            _services.Notifier.Add(NotifyType.Information, T("{0} gallery was properly saved", gallery.As<TitlePart>().Title));
            
            return RedirectToAction("Index");

        }

        public ActionResult Delete(int id)
        {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

            var gallery = _services.ContentManager.Get(id);
            var galleryTitle = gallery.As<TitlePart>().Title;
            _services.ContentManager.Remove(gallery);

            _services.Notifier.Add(NotifyType.Information, T("{0} gallery was removed", galleryTitle));

            return RedirectToAction("Index");
        }

        private void SetDefaults(ContentItem gallery)
        {
            gallery.As<ContainerPart>().ItemContentType = "Project";
        }
    }
}