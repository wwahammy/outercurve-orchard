using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Xml;
using MarkdownSharp;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Models;
using Outercurve.Projects.Helpers;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;
using Outercurve.Projects.ViewModels.Parts;

namespace Outercurve.Projects.Drivers
{
    public class CLAPartDriver : ContentPartDriver<CLAPart> {
        private readonly ICLAPartService _claService;
        private readonly IContentManager _contentManager;

        private readonly IUTCifierService _utcService;
        private readonly IMembershipService _membershipService;
        private readonly ITransactionManager _transaction;
        private readonly IClock _clock;
        private readonly ICLATemplateService _templateService;
        public ILogger Logger;
        private const string TemplateName = "Parts/CLA";
        public Localizer T;

        
        protected override string Prefix {
            get { return "CLA"; }
        }

        public CLAPartDriver(ICLAPartService claService, IContentManager contentManager, IUTCifierService utcService,
            IMembershipService membershipService, ITransactionManager transaction, IClock clock, ICLATemplateService templateService)
        {
            _claService = claService;
            _contentManager = contentManager;

            _utcService = utcService;
            _membershipService = membershipService;
            _transaction = transaction;
            _clock = clock;
            _templateService = templateService;
            Logger = NullLogger.Instance;

            T = NullLocalizer.Instance;
        }

        protected override DriverResult Display(CLAPart part, string displayType, dynamic shapeHelper) {
            try {
                


                return ContentShape("Parts_CLA", () => shapeHelper.Parts_CLA(
                    ContentItem: part.ContentItem,
                    ContentPart: part,
                    IsValid: part.IsValid,
                    ProjectPart: part.As<CommonPart>().Container,
                    Project: part.As<CommonPart>().Container.As<TitlePart>().Title,
                    CLASigner: part.CLASigner.FullName(),
                    ValidDate: part.SignedDate,
                    FoundationSigner: part.FoundationSigner == null ? "-" : part.FoundationSigner.FullName()
                                                           ));
            }
            catch (Exception e) {
                Logger.Error(e, "Failure in CLAPart");
                throw e;
            }

        }

        protected override DriverResult Editor(CLAPart part, dynamic shapeHelper) {

            return CreateShape(BuildEditorViewModel(part), shapeHelper);
        }

        protected override DriverResult Editor(CLAPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var model = new EditCLAViewModel();
           

            if (updater.TryUpdateModel(model, Prefix, null, null) && _claService.Validate(model, updater)) {
                
                _claService.UpdateItemWithClaInfo(part.ContentItem, model);
            }
            else {
                _transaction.Cancel();
            }
            //do anything we need to do
            //any of the loading stuff should happen here
            return CreateShape(model, shapeHelper);
         
        }


        private EditCLAViewModel BuildEditorViewModel(CLAPart part) {

           //var templateIdVersion = _templateService.CreateCLATemplateIdVersion(part.CLATemplate);
          /*  var selectedTemplate = part.CLATemplate;

            if (part.CLATemplate == null) {
                selectedTemplate = _contentManager.Query("CLATemplate").List().First();
            }*/

          

            
                

            var vm = new EditCLAViewModel {
                
                Id = part.Id,
                CLASignerUsername =  part.CLASigner == null ? null : _contentManager.Get(part.CLASigner.Id).As<UserPart>().NormalizedUserName,
                CLASignerFirstName = part.FirstName,
                CLASignerLastName = part.LastName,
                CLASignerEmail = part.SignerEmail,
                Comments = part.Comments,
                IsCommitter = part.IsCommitter,
                Employer = part.Employer,
                Address1 = part.Address1,
                Address2 = part.Address2,
                City = part.City,
                State = part.State,
                ZipCode = part.ZipCode,
                Country = part.Country,
                NeedCompanySignature = part.RequiresEmployerSigner,
                StaffOverride = part.OfficeValidOverride,
                LocationOfCLA = part.LocationOfCLA,
               // SelectedTemplate = _templateService.CreateCLATemplateIdVersion(selectedTemplate)
                 

            };

            var currentTime = _clock.UtcNow.ToLocalTime();

            if (part.IsSignedByUser) {
                vm.SigningDate = part.SignedDate.Value.ToLocalTime().ToString("d");
                vm.IsSignedByUser = true;
            }
            else {
                
                vm.SigningDate = currentTime.ToString("d");
            }



            if (part.HasFoundationSigner) {
                vm.FoundationSignerUsername = part.FoundationSigner == null ? null : _contentManager.Get(part.FoundationSigner.Id).As<UserPart>().NormalizedUserName;
                vm.FoundationSigningDate =  part.FoundationSignedOn.Value.ToLocalTime().ToString("d");
                vm.HasFoundationSigner = true;
            }
            else {
                vm.FoundationSigningDate = currentTime.ToString("d");
            }

            if (part.RequiresEmployerSigner)
            {
                vm.CompanySigner = part.SignerFromCompany;
                vm.CompanySignerEmail = part.SignerFromCompanyEmail;
                vm.CompanySigningDate = part.EmployerSignedOn == null ? "": part.EmployerSignedOn.Value.ToLocalTime().ToString("d");
                vm.HasCompanySigner = part.EmployerSignedOn != null;
            }
            else {
                vm.CompanySigningDate = currentTime.ToString("d");
            }

            

          
            
            return vm;
        }

        protected virtual DriverResult CreateShape(EditCLAViewModel vm, dynamic shapeHelper) {


            

            return ContentShape("Parts_CLA_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: TemplateName,
                                    Model: vm,
                                    Prefix: Prefix));
        }
    }
}