using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Settings;
using Outercurve.Projects.Services;

namespace Outercurve.Projects.Controllers
{
    public abstract class BaseController : VeryLowLevelBaseController
    {
        protected readonly IExtendedUserPartService _extUserService;
        protected readonly IGalleryService _galleryService;
        protected readonly ITransactionManager _transaction;
        protected readonly IOrchardServices _services;
        protected readonly ISiteService _siteService;

       
        protected dynamic Shape { get; set; }

        protected BaseController(IExtendedUserPartService extUserService, IGalleryService galleryService, 
            ITransactionManager transaction, IShapeFactory shapeFactory, IOrchardServices services, ISiteService siteService) {
            _extUserService = extUserService;
            _galleryService = galleryService;
            _transaction = transaction;
            _services = services;
            _siteService = siteService;

            Shape = shapeFactory;
      
        }

        protected void SetGalleries(int id) {
            SetGalleries(id.ToString());
        }

        protected void SetGalleries(string selectedId) {
            var galleries = _galleryService.GetGalleriesAsSelectList();
            this.ViewBag.Galleries = new SelectList(galleries, "Id", "Name", selectedId);
        }


      

        protected void SetAllUsers() {
            var users = _extUserService.GetExtendedUserListEntries().ToList();
            this.ViewBag.AllUsers = users;
        }
        
    }
}