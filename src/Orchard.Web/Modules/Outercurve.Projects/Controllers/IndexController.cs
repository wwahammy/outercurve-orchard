using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Security;
using Orchard.Settings;
using Orchard.Themes;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels.Index;

namespace Outercurve.Projects.Controllers
{
    [Themed]
    public class IndexController : BaseController 
    {
        private readonly IAuthenticationService _authService;


        public IndexController(IExtendedUserPartService extUserService,
            IGalleryService galleryService,
            ITransactionManager transaction,
            IShapeFactory shapeFactory,
            IOrchardServices services,
            ISiteService siteService, IAuthenticationService authService) : base(extUserService, galleryService, transaction, shapeFactory, services, siteService) {
            _authService = authService;
        }

        public ActionResult Index() {

            var user = _authService.GetAuthenticatedUser();
            var model = new IndexViewModel();
            if (user != null) {
                model.IsContributor = _extUserService.IsAContributor(user);
                model.IsProjectLeader = _extUserService.IsAProjectLeader(user);
            }
                
            

            return View("Index", (object)model);
        }
    }
}