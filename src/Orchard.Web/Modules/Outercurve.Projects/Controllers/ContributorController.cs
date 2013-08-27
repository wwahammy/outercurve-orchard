using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Security;
using Orchard.Settings;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.Users.Services;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;

namespace Outercurve.Projects.Controllers
{
    [Themed]
    public class ContributorController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IRequestMsdnService _msdn;


        public ContributorController(IExtendedUserPartService extUserService,
                                     IGalleryService galleryService, ITransactionManager transaction,
                                     IShapeFactory shapeFactory,
                                     IOrchardServices services, ISiteService siteService, IAuthenticationService authenticationService, IRequestMsdnService msdn 
            ) 
            : base(extUserService, galleryService, transaction, shapeFactory, services, siteService) {
            _authenticationService = authenticationService;
            _msdn = msdn;
        }

        public ActionResult RegisterForMsdn() {
            var currentUser = _authenticationService.GetAuthenticatedUser();
            if (currentUser == null || !_extUserService.IsAContributor(currentUser)) {
                return new HttpUnauthorizedResult();
            }


            var lastCla = GetLastCLA(currentUser);
            var model = SetFromCLA(lastCla);


            return View("RegisterForMsdn", (object) model);


        }

        [ActionName("RegisterForMsdn"), HttpPost]
        public ActionResult RegisterForMsdnPOST() {
            var currentUser = _authenticationService.GetAuthenticatedUser();
            if (currentUser == null || !_extUserService.IsAContributor(currentUser))
            {
                return new HttpUnauthorizedResult();
            }
            var model = new RegisterForMsdnViewModel();
            if (TryUpdateModel(model)) {
                if (String.IsNullOrWhiteSpace(model.Company))
                    model.Company = "none";
                if (_msdn.SubmitMsdnRequest(model)) {
                    return View("MsdnRegistered");
                }
                _services.Notifier.Add(NotifyType.Error, 
                    T("The MSDN Submission could not be submitted, please try again later. If you see this more than once please email Eric at eschultz@outercurve.org"));
            
                

            }

            return View("RegisterForMsdn", (object) model);
        }

        private RegisterForMsdnViewModel SetFromCLA(ContentItem cla) {
            var claPart = cla.As<CLAPart>();
            var reg = new RegisterForMsdnViewModel {
                Address1 = claPart.Address1,
                Address2 = claPart.Address2,
                City = claPart.City,
                State = claPart.State,
                ZipCode = claPart.ZipCode,
                Country = claPart.Country,
                Email = claPart.SignerEmail,
                FirstName = claPart.FirstName,
                LastName = claPart.LastName,
                Project = cla.As<CommonPart>().Container.As<TitlePart>().Title

            };

            return reg;
        }

        private ContentItem GetLastCLA(IUser u) {
            return _services.ContentManager.Query("CLA").Where<CLAPartRecord>(i => i.CLASigner.Id == u.Id).OrderByDescending(c => c.SignedDate).Slice(0, 1).First();
        }
    }
}