using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Admin;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;

namespace Outercurve.Projects.Controllers
{
    [Admin]
    public class QuickUserAdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _services;
        private readonly IExtendedUserPartService _extUserService;
        private readonly ITransactionManager _transaction;
        private readonly Localizer T;

        public QuickUserAdminController(IOrchardServices services, IExtendedUserPartService extUserService, ITransactionManager transaction) {
            _services = services;
            _extUserService = extUserService;
            _transaction = transaction;
            T = NullLocalizer.Instance;

        }

        public ActionResult Create(string returnUrl) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

            var model = new QuickUserViewModel {ReturnUrl = returnUrl};

            return View((object) model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST([Bind(Exclude= "Id")]QuickUserViewModel model) {
            if (!_services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list projects")))
            {
                return new HttpUnauthorizedResult();
            }

            IUser user = null;
            if (TryUpdateModel(model)) {
                user = _extUserService.CreateAutoRegisteredUser(model.Email, model.FirstName, model.LastName);
                if (user == null) {
                    AddModelError("Email", T("The Email is not unique"));
                }
            }

            if (!ModelState.IsValid) {
               _transaction.Cancel();

                return View((object) model);
            }

            else {
                return Redirect(model.ReturnUrl);
            }
            
        }



         bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        public void AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    
    }
}