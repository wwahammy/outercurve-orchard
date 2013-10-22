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
    public class ProjectAdminController : BaseController
    {
        

        public ProjectAdminController(IOrchardServices services, IExtendedUserPartService extUserService, ITransactionManager transaction, IGalleryService galleryService, 
            IShapeFactory shapeFactory, ISiteService siteService) : base(extUserService, galleryService, transaction, shapeFactory, services, siteService) {
     
        }


        public ActionResult Index(PagerParameters pagerParameters)
        {
            if (!_services.Authorizer.Authorize(ProjectPermissions.ModifyProjects, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);


            var query = _services.ContentManager.Query().ForType("Project");
            var pagerShape = Shape.Pager(pager).TotalItemCount(query.Count());

            var results = query.Slice(pager.GetStartIndex(), pager.PageSize);

            var model = new ProjectsAdminIndexViewModel {
                Pager = pagerShape,
                Projects = results.Select(x => new ProjectAdminIndexEntry {ProjectItem = x}).ToList()
            };
            

            return View((object)model);
        }

        public ActionResult Create() {
            if (!_services.Authorizer.Authorize(ProjectPermissions.ModifyProjects, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }


            var project =  _services.ContentManager.New("Project");
            SetDefaults(project);
            var model = _services.ContentManager.BuildEditor(project);
            SetAllUsers();
            
            SetGalleries(null);

            return View((object) model);
        }

        

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST() {
            if (!_services.Authorizer.Authorize(ProjectPermissions.ModifyProjects, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

            var project = _services.ContentManager.New("Project");
            SetDefaults(project);
            _services.ContentManager.Create(project);
            var model = _services.ContentManager.UpdateEditor(project, this);
            if (!ModelState.IsValid) {
                _transaction.Cancel();
                SetAllUsers();
                SetGalleries(null);
                return View((object) model);
            }

            _services.ContentManager.Publish(project);

            _services.Notifier.Add(NotifyType.Information, T("Project named {0} created successfully", project.As<TitlePart>().Title));

            return RedirectToAction("Index");
        }

        
       

        
        public ActionResult Edit(int id)
        {
            if (!_services.Authorizer.Authorize(ProjectPermissions.ModifyProjects, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

            var gallery = _services.ContentManager.Get(id);
            SetAllUsers();
            SetGalleries(id);

            var model = _services.ContentManager.BuildEditor(gallery);



            return View((object)model);
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

           var project = _services.ContentManager.Get(id);
           SetDefaults(project);
            
           var model = _services.ContentManager.UpdateEditor(project, this);
            if (!ModelState.IsValid) {
                _transaction.Cancel();
                SetAllUsers();
                SetGalleries(id);
                return View((object) model);
            }
            _services.ContentManager.Publish(project);
            _services.Notifier.Add(NotifyType.Information, T("{0} gallery was edited successfully", project.As<TitlePart>().Title));
            return RedirectToAction("Index");

        }

        public ActionResult Delete(int id) {
            if (!_services.Authorizer.Authorize(ProjectPermissions.ModifyProjects, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }
           
            var project = _services.ContentManager.Get(id);
            var projectTitle = project.As<TitlePart>().Title;
            _services.ContentManager.Remove(project);

            _services.Notifier.Add(NotifyType.Information, T("{0} gallery was removed", projectTitle));

            return RedirectToAction("Index");
        }

        private void SetDefaults(ContentItem project)
        {
            project.As<ContainerPart>().ItemContentType = "CLA";
        }

       
    }
}