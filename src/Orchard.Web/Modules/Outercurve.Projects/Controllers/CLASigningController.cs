using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Security;
using Orchard.Services;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.Users.Events;
using Outercurve.Projects.Helpers;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;
using Outercurve.Projects.ViewModels.CLASigning;

namespace Outercurve.Projects.Controllers
{
    
    public class CLASigningController : VeryLowLevelBaseController {

        public const string BADPROJECTIDMESSAGE = "You didn't have a valid project. Are you sure you didn't copy a link wrong? If you're sure you didn't make a mistake, email Eric at eschultz@outercurve.org.";
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _services;
       
        private readonly ITransactionManager _transaction;
        private readonly ICLASigningService _claSigning;
        private readonly IExtendedUserPartService _extUserService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserEventHandler _userEventHandler;
        private readonly IClock _clock;


        public CLASigningController(IContentManager contentManager, IOrchardServices services, ITransactionManager transaction, ICLASigningService claSigning, 
            IExtendedUserPartService extUserService, IAuthenticationService authenticationService, IUserEventHandler userEventHandler, IClock clock) {
            _contentManager = contentManager;
            _services = services;
           
            _transaction = transaction;
            _claSigning = claSigning;
            _extUserService = extUserService;
            _authenticationService = authenticationService;
            _userEventHandler = userEventHandler;
            _clock = clock;
        }
        [Themed]
        public ActionResult Index() {

            var projects = _contentManager.Query("Project").List();
            var model = new IndexViewModel {
                Projects = projects.Select(i => new ProjectNameAndId {Id = i.Id, Name = i.As<TitlePart>().Title}).ToList()
            };
            return View((object) model);
        }
        [Themed]
        public ActionResult Choose(int id) {

            if (GetValidProject(id) == null) {
                _services.Notifier.Add(NotifyType.Information, T(BADPROJECTIDMESSAGE));
                return RedirectToAction("Index");
            }
            var model = new ChooseViewModel {
                ProjectId = id
            };

            return View((object)model);
        }

        [Themed]
        public ActionResult SignIndividual(int id, bool needCountersign = false) {

            var projectItem = GetValidProject(id);
            if (projectItem == null) {
                _services.Notifier.Add(NotifyType.Information, T(BADPROJECTIDMESSAGE));
                return RedirectToAction("Index");
            }

            var currentUser = _authenticationService.GetAuthenticatedUser(); ;
            var firstName = currentUser != null ? currentUser.As<ExtendedUserPart>().FirstName :"";
            var lastName = currentUser != null ? currentUser.As<ExtendedUserPart>().LastName : "";
            var email = currentUser != null ? currentUser.Email : "";
          
            var model = new SignIndividualViewModel {
                ProjectId = id,
                ProjectName = projectItem.As<TitlePart>().Title,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CLA = projectItem.As<ProjectPart>().CLATemplate.CLA,
                NeedCompanySignature = needCountersign
            };



            return View((object)model);
        }

        [Themed]
        [HttpPost, ActionName("SignIndividual")]
        public ActionResult SignIndividualPOST() {
            var model = new SignIndividualViewModel();
            if (TryUpdateModel(model, ""))
            {
                
                if (GetValidProject(model.ProjectId) == null) {
                    _services.Notifier.Add(NotifyType.Information, T(BADPROJECTIDMESSAGE));
                    return RedirectToAction("Index");
                }

               
               if (ModelState.IsValid) {

                   var currentUser = _authenticationService.GetAuthenticatedUser();
                   if (currentUser == null)
                   {
                        currentUser = _extUserService.CreateAutoRegisteredUser(model.Email, model.FirstName, model.LastName);
                       if (currentUser == null)
                       {
                           AddModelError("Email", T("That email is already registered. You should login instead!"));
                       }
                       else
                       {
                           _authenticationService.SignIn(currentUser, true);
                           _userEventHandler.LoggedIn(currentUser);
                       }

                   }

                   if (ModelState.IsValid) {
                       var item = _claSigning.SignIndividual(model, currentUser, UrlHelper, EmailToCompanySigner);
                       var projectName = _contentManager.Get<TitlePart>(model.ProjectId).Title;
                       if (model.NeedCompanySignature) {
                           var vm = new StillNeedCompanyViewModel {
                               Contact = model.CompanyContact,
                               Email = model.CompanyContactEmail,
                               Project = projectName,
                               CompanySigningLink = _claSigning.CreateCompanySigningNonceLink(item.Id, UrlHelper)
                           };

                           return View("StillNeedCompany", (object) vm);
                       }
                       else {

                           var vm = new SigningFinishedViewModel {
                               Project = projectName
                           };
                           return View("SigningFinished", (object) vm);
                       }
                   }

               }
           }
            else {
                if (GetValidProject(model.ProjectId) == null)
                {
                    _services.Notifier.Add(NotifyType.Information, T(BADPROJECTIDMESSAGE));
                    return RedirectToAction("Index");
                }
            }
          
            _transaction.Cancel();
            var projectItem = _contentManager.Get(model.ProjectId);
            model.CLA = projectItem.As<ProjectPart>().CLATemplate.CLA;
            return View("SignIndividual", (object) model);

        }

        [Themed]
        public ActionResult SignCompany(string nonce) {
            ContentItem cla;
            
            if (_claSigning.DecryptCompanySigningNonce(nonce, out cla)) {
                var claPart = cla.As<CLAPart>();
                if (claPart.EmployerMustSignBy != null && claPart.EmployerMustSignBy.Value < _clock.UtcNow)
                {
                    //notify the user to resend
                    _claSigning.NotifyUserOfLateNonce(claPart, UrlHelper);
                    
                    return View("TooLateForNonce");
                }


                var model = CreateSignCompanyViewModel(cla);

                return View("SignCompany", (object) model);
            }
            else {
                //nonce decrypt failed
                return View("InvalidNonce");
            }
        }


        [Themed]
        [HttpPost, ActionName("SignCompany")]
        public ActionResult SignCompanyPOST() {
            var vm = new SignCompanyViewModel();
            ContentItem cla;
            if (TryUpdateModel(vm)) {
                cla = GetValidCLA(vm.CLAId);
                if (cla == null) 
                {
                    //nonce decrypt failed
                    return View("InvalidNonce");
                }
                var claPart = cla.As<CLAPart>();
                if (claPart.EmployerMustSignBy < _clock.UtcNow) {
                    //notify the user to resend
                    _claSigning.NotifyUserOfLateNonce(claPart, UrlHelper);

                    return View("TooLateForNonce");
                }

              

                _claSigning.SignCompany(vm, UrlHelper);

                return View("CompanySigningFinished");


            }


            cla = GetValidCLA(vm.CLAId);
            if (cla == null)
            {
                //nonce decrypt failed
                return View("InvalidNonce");
            }
            var model = CreateSignCompanyViewModel(cla);

            model.CompanyContact = vm.CompanyContact;
            model.CompanyContactEmail = vm.CompanyContactEmail;

            return View("SignCompany", (object) model);
            
            
        }

        [Themed]
        public ActionResult ResendLink(string nonce) {
            ContentItem item;
            if (!_claSigning.DecryptResendNonce(nonce, out item)) {
                return View("InvalidNonce");
            }
            var cla = GetValidCLA(item.Id);
            if (cla == null) {
                //nonce decrypt failed
                return View("InvalidNonce");
            }
            //TODO validate that we need a link resent
            var model = new ResendLinkViewModel {CLAId = item.Id, ProjectName = item.As<CommonPart>().Container.As<TitlePart>().Title};


            return View("ResendLink", (object)model);
        }
        [Themed]
        [ActionName("ResendLink"), HttpPost]
        public ActionResult ResendLinkPOST() {
            var model = new ResendLinkViewModel();
            
            ContentItem cla;
            if (TryUpdateModel(model)) {
                //TODO validate that we need a link resent
                cla = GetValidCLA(model.CLAId);
                if (cla == null)
                {
                    //nonce decrypt failed
                    return View("InvalidNonce");
                }

                var projectName = cla.As<CommonPart>().Container.As<TitlePart>().Title;
                _claSigning.ResendNewNonceLink(model, UrlHelper, EmailToCompanySigner);
                

                var vm = new StillNeedCompanyViewModel {
                    Contact = model.CompanyContact,
                    Email = model.CompanyContactEmail,
                    Project = projectName,
                    CompanySigningLink = _claSigning.CreateCompanySigningNonceLink(cla.Id, UrlHelper)
                };
                
                return View("StillNeedCompany", (object)vm);
            }

            cla = GetValidCLA(model.CLAId);
            if (cla == null)
            {
                //nonce decrypt failed
                return View("InvalidNonce");
            }


            var projName = cla.As<CommonPart>().Container.As<TitlePart>().Title;
            model.ProjectName = projName;

            return View("ResendLink", (object) model);

        }

        
        private string EmailToCompanySigner(string projectName, string firstName, string lastName, string companySigner, string link) {
            var output = new EmailToCompanySignerViewModel {ProjectName = projectName, FirstName = firstName, LastName = lastName, CompanySigner = companySigner, Link = link};
            return RenderRazorViewToString("EmailToCompanySigner", output);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <from>http://stackoverflow.com/questions/483091/render-a-view-as-a-string</from>
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

       
     
        public IUrlHelper UrlHelper {
            get { return new UrlHelperWrapper(Url); }
        }


        private SignCompanyViewModel CreateSignCompanyViewModel(ContentItem cla) {

            var claPart = cla.As<CLAPart>();
            var project = cla.As<CommonPart>().Container.As<TitlePart>();
            var actualCLAText = cla.As<CLATextPart>().CLATemplate;
            //var actualCLAText = claPart.CLATemplate;

            var model = new SignCompanyViewModel {
                CLAId =  cla.Id,
                CLA = actualCLAText.As<CLATemplatePart>().CLA,
                FirstName = claPart.FirstName,
                LastName = claPart.LastName,
                Address1 = claPart.Address1,
                Address2 = claPart.Address2,
                City = claPart.City,
                ZipCode = claPart.ZipCode,
                State = claPart.State,
                Country = claPart.Country,
                Email = claPart.SignerEmail,
                ProjectId = project.Id,
                ProjectName = project.Title
            };


            

           
            return model;
        }


        private ContentItem GetValidProject(int id) {

            var projectItem = _contentManager.Get(id);
            if (projectItem == null || !projectItem.Has<ProjectPart>() || projectItem.ContentType != "Project") {
                return null;
            }

            return projectItem;
        }

        private ContentItem GetValidCLA(int id) {
            var claItem = _contentManager.Get(id);
            if (claItem == null || !claItem.Has<CLAPart>() || claItem.ContentType != "CLA") {
                return null;
            }

            return claItem;
        }
    }
}