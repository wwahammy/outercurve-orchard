using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels.CLATemplateAdmin;

namespace Outercurve.Projects.Controllers {
    [Admin]
    public class CLATemplateAdminController : BaseController {
        public CLATemplateAdminController(IExtendedUserPartService extUserService, IGalleryService galleryService, ITransactionManager transaction, IShapeFactory shapeFactory, IOrchardServices services, ISiteService siteService)
            : base(extUserService, galleryService, transaction, shapeFactory, services, siteService) {}


        public ActionResult Index(PagerParameters pagerParameters) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list custom forms")))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);


            var query = _services.ContentManager.Query().ForType("CLATemplate");
            var pagerShape = Shape.Pager(pager).TotalItemCount(query.Count());

            var results = query.Slice(pager.GetStartIndex(), pager.PageSize);


            var model = new CLATemplateAdminIndexViewModel {
                Templates = results.Select(i => new CLATemplateAdminIndexEntry {
                    Item = i,
                    Name = i.As<CLATemplatePart>().CLATitle,
                    Text = i.As<CLATemplatePart>().CLA.SafeSubstring(0, 100)
                }).ToList(),

                Pager = pagerShape
            };

            return View((object) model);

        }

        public ActionResult Create() {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized create agreement templates."))) {
                return new HttpUnauthorizedResult();
            }

            var claTemplate = _services.ContentManager.New("CLATemplate");

            var shape = _services.ContentManager.BuildEditor(claTemplate);

            return View((object) shape);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST() {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to create agreement templates."))) {
                return new HttpUnauthorizedResult();
            }

            var claTemplate = _services.ContentManager.New("CLATemplate");

            _services.ContentManager.Create(claTemplate);
            var model = _services.ContentManager.UpdateEditor(claTemplate, this);

            if (!ModelState.IsValid) {
                _transaction.Cancel();
                return View((object) model);
            }

            _services.ContentManager.Publish(claTemplate);

            _services.Notifier.Add(NotifyType.Information, T("The Agreement Template was properly created."));

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to edit agreement templates."))) {
                return new HttpUnauthorizedResult();
            }

            var claTemplate = _services.ContentManager.Get(id);
            var shape = _services.ContentManager.BuildEditor(claTemplate);
            return View((object) shape);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to edit agreement templates."))) {
                return new HttpUnauthorizedResult();
            }
            var cla = _services.ContentManager.Get(id);

            var model = _services.ContentManager.UpdateEditor(cla, this);

            if (!ModelState.IsValid) {
                _transaction.Cancel();
                return View((object) model);
            }

            _services.ContentManager.Publish(cla);

            _services.Notifier.Add(NotifyType.Information, T("The agreement template was properly edited,"));
            return RedirectToAction("Index");
        }

        /*
        public ActionResult Delete(int id) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to remove projects")))
            {
                return new HttpUnauthorizedResult();
            }
            var item = _services.ContentManager.Get(id);
            _services.ContentManager.Remove(item);
        }*/
    }

}