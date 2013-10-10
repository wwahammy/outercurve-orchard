using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MarkdownSharp;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;

namespace Outercurve.Projects.Controllers
{
    [Admin]
    public class CLAAdminController : BaseController
    {
        private readonly IProjectService _projectService;
        private readonly ICLAToOfficeService _officeService;
        private readonly ICLATemplateService _templateService;
        public ILogger Logger;

        public CLAAdminController(IOrchardServices services, IExtendedUserPartService extUserService, IGalleryService galleryService, ITransactionManager transaction, IShapeFactory shapeFactory, ISiteService siteService, IProjectService projectService, 
            ICLAToOfficeService officeService, ICLATemplateService templateService ) :
            base(extUserService, galleryService, transaction, shapeFactory, services, siteService) {
            _projectService = projectService;
            _officeService = officeService;
            _templateService = templateService;
        }


        public ActionResult Index(CLAIndexOptions options, PagerParameters pagerParameters) {
             if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to view agreements")))
                return new HttpUnauthorizedResult();

            try {

                var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

                if (options == null) 
                    options = new CLAIndexOptions();
                

                var query = _services.ContentManager.Query().ForType("CLA");

                switch (options.Order) {
                    case CLAOrder.Created:
                        //this should happen anyways but I don't think this breaks anything by doing this
                        query = query.OrderBy<CLAPartRecord>(r => r.Id);
                        break;
                    case CLAOrder.Name:
                        query = query.OrderBy<CLAPartRecord>(r => r.FirstName).OrderBy(r => r.LastName);
                        break;
                    case CLAOrder.SignedByUser:
                        query = query.OrderBy<CLAPartRecord>(r => r.SignedDate);
                        break;

                }

                var pagerShape = Shape.Pager(pager).TotalItemCount(query.Count());

                var results = query.Slice(pager.GetStartIndex(), pager.PageSize);

                 
                //this needs to be fixed:
                var model = new CLAAdminIndexViewModel {
                    CLAs = results.Select(i => new CLAAdminIndexEntry {
                        CLAItem = i,
                       CLASignerName =  String.Format("{0} {1} - (User: {2})",i.As<CLAPart>().FirstName, i.As<CLAPart>().LastName, _services.ContentManager.Get<IUser>(i.As<CLAPart>().CLASigner.Id).UserName),
                        FoundationSignerName = i.As<CLAPart>().FoundationSigner == null ? "-" : _extUserService.GetFullName(i.As<CLAPart>().FoundationSigner),
                        ProjectTitle = i.As<CommonPart>().Container == null ? "-" :i.As<CommonPart>().Container.As<TitlePart>().Title,
                        SignedDate = i.As<CLAPart>().SignedDate == null ? "Not Signed" : i.As<CLAPart>().SignedDate.ToLocalDateString(),
                        Employer = i.As<CLAPart>().Employer,
                        IsActive = i.As<CLAPart>().IsValid
                    }).ToList(),
                    Pager = pagerShape,
                    Options = options
                };
                var routeData = new RouteData();
                routeData.Values.Add("Options.Order", options.Order);
                pagerShape.RouteData(routeData);

                return View(model);
            }
            catch (Exception e) {
                Logger.Error(e, "holy cow!!");
                throw;
            }
        }

        public ActionResult Create() {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

            var cla = _services.ContentManager.New("CLA");

            var shape = _services.ContentManager.BuildEditor(cla);
            SetAllProjects();
            SetAllUsers();
            return View((object) shape);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST() {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

            var cla = _services.ContentManager.New("CLA");

            _services.ContentManager.Create(cla);
            var model = _services.ContentManager.UpdateEditor(cla, this);
            
            if (!ModelState.IsValid) {
                _transaction.Cancel();
                SetAllUsers();
                SetAllProjects();
                return View((object) model);
            }

            _services.ContentManager.Publish(cla);
            _services.Notifier.Add(NotifyType.Information, T("The Agreement was properly created."));
            return RedirectToAction("Index");
        }


        public ActionResult Edit(int id) {

            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

            var cla = _services.ContentManager.Get(id);
            var shape = _services.ContentManager.BuildEditor(cla);

            SetAllUsers();
            SetAllProjects();
            return View((object)shape);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }
            var cla = _services.ContentManager.Get(id);

            var model = _services.ContentManager.UpdateEditor(cla, this);

            if (!ModelState.IsValid)
            {
                _transaction.Cancel();
                SetAllUsers();
                SetAllProjects();
                return View((object)model);
            }


            _services.ContentManager.Publish(cla);
            _services.Notifier.Add(NotifyType.Information, T("The CLA was properly edited."));
            return RedirectToAction("Index");
        }

        public ActionResult GetIdAndVersion(string idVersion) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

            var template = _templateService.GetCLATemplateFromIdVersion(idVersion).As<CLATemplatePart>();
            
            

            var markdown = new Markdown();
            var cla = markdown.Transform(template.CLA);
            return new JsonResult {Data = cla, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }
       

        public ActionResult GetExcelOfCLAs() {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

            var xlsx = _officeService.CreateCLASpreadsheet();

            var cd = new System.Net.Mime.ContentDisposition
            {
                // for example foo.bak
                FileName = "CLAs.xlsx",

                // always prompt the user for downloading, set to true if you want 
                // the browser to try to show the file inline
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public ActionResult Delete(int id) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to remove projects")))
            {
                return new HttpUnauthorizedResult();
            }
            var item = _services.ContentManager.Get(id);
            _services.ContentManager.Remove(item);



            _services.Notifier.Add(NotifyType.Information, T("The CLA was removed"));
            return RedirectToAction("Index");
        }

        private void SetAllProjects() {
            var projects = _projectService.GetAllProjectsEntries().ToList();
            this.ViewBag.AllProjects = projects;
        }
    }
}