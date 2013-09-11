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
           

            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                var hadError = false;
                _claService.UpdateItemWithClaInfo(part.ContentItem, model);
                /*
                DateTime utc = new DateTime();
                //check the data
                
                if (DateTime.TryParse(model.SigningDate, out utc)) {
                    
                }
                else {
                    updater.AddModelError("SigningDate", T("{0} is not a valid date", model.SigningDate));
                    hadError = true;
                }

                //TODO: verify users
                if (!hadError) {
                    
                }
                else {*/
                    
//              }
                
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
            var selectedTemplate = part.CLATemplate;

            if (part.CLATemplate == null) {
                selectedTemplate = _contentManager.Query("CLATemplate").List().First();
            }

          

            
                

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
               SelectedTemplate = _templateService.CreateCLATemplateIdVersion(selectedTemplate)
                 

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


            MustSetEveryTime(vm);

            return ContentShape("Parts_CLA_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: TemplateName,
                                    Model: vm,
                                    Prefix: Prefix));
        }

        private void MustSetEveryTime(EditCLAViewModel vm) {

            var selectedTemplate = _templateService.GetCLATemplateFromIdVersion(vm.SelectedTemplate);

            var allTemplatesIdVersionAndNiceName =
                _contentManager.Query(VersionOptions.AllVersions, "CLATemplate").
                                List().Select(i => new KeyValuePair<string, string>(_templateService.CreateCLATemplateIdVersion(i), i.As<CLATemplatePart>().CLATitle + ", v" + i.Version));

            vm.Template = new TemplateDetailViewModel {
                CurrentHtmlForTemplate = new Markdown().Transform(selectedTemplate.As<CLATemplatePart>().CLA),
                TemplateNameVersionsAndIds = allTemplatesIdVersionAndNiceName
            };
        }

        #region Import/Export

        protected override void Exporting(CLAPart part, Orchard.ContentManagement.Handlers.ExportContentContext context) {

           // var partIdentity = _contentManager.GetItemMetadata(_contentManager.Get(part.TemplateId)).Identity;

            var claSignerIdentity = part.CLASigner == null ? null : _contentManager.GetItemMetadata(_contentManager.Get(part.CLASigner.Id)).Identity;
            var foundationSignerId = part.FoundationSigner == null ? null : _contentManager.GetItemMetadata(_contentManager.Get(part.FoundationSigner.Id)).Identity;

            var last = context.Element(part.PartDefinition.Name)
                              .Attr("Address1", part.Record.Address1)
                              .Attr("Address2", part.Record.Address2)
                              .Attr("City", part.Record.City)
                              .Attr("State", part.Record.State)
                              .Attr("Comments", part.Record.Comments)
                              .Attr("Country", part.Record.Country)
                              .Attr("Employer", part.Record.Employer)

                              .Attr("FirstName", part.Record.FirstName)
                              .Attr("LastName", part.Record.LastName)
                              .Attr("LocationOfCLA", part.Record.LocationOfCLA)
                              .Attr("IsCommitter", part.Record.IsCommitter)

                // .Attr("FoundationSignerName", part.Record.FoundationSignerName)
                              .Attr("OfficeValidOverride", part.Record.OfficeValidOverride)
                              .Attr("RequiresEmployerSigner", part.Record.RequiresEmployerSigner)
                              .Attr("SignerEmail", part.Record.SignerEmail)
                              .Attr("SignerFromCompany", part.Record.SignerFromCompany)
                              .Attr("ZipCode", part.Record.ZipCode)
                              .Attr("TemplateVersion", part.Record.TemplateVersion);
                 //  .Attr("TemplateId", partIdentity.ToString());

            if (claSignerIdentity != null) {
                last = last.Attr("CLASignerId", claSignerIdentity.ToString());
            }


            if (foundationSignerId != null) {
                last = last.Attr("FoundationSignerId", foundationSignerId.ToString());
            }
                   
                   
            

            if (part.FoundationSignedOn != null) {
                last = last.Attr("FoundationSignedOn", XmlConvert.ToString(part.FoundationSignedOn.Value, XmlDateTimeSerializationMode.Utc));
            }

            if (part.EmployerSignedOn != null) {
                last = last.Attr("FoundationSignedOn", XmlConvert.ToString(part.EmployerSignedOn.Value, XmlDateTimeSerializationMode.Utc));
            }

            if (part.SignedDate != null) {
                last.Attr("SignedDate", XmlConvert.ToString(part.SignedDate.Value, XmlDateTimeSerializationMode.Utc));
            }


        }


        protected override void Importing(CLAPart part, Orchard.ContentManagement.Handlers.ImportContentContext context) {

            var elem = context.Data.Element(part.PartDefinition.Name);

            part.Address1 = elem.Attr("Address1");
            part.Address2 = elem.Attr("Address2");
          
           // part.CLAText = context.Attribute(part.PartDefinition.Name, "CLAText");
            part.City = elem.Attr("City");
            part.Comments = elem.Attr("Comments");
            part.Country = elem.Attr("Country");
            part.Employer = elem.Attr("Employer");
            part.FirstName = elem.Attr("FirstName");
            part.LastName = elem.Attr("LastName");
            part.LocationOfCLA = elem.Attr("LocationOfCLA");
           // part.FoundationSignerName = elem.Attr("FoundationSignerName");
            part.SignerEmail = elem.Attr("SignerEmail");
            part.SignerFromCompany = elem.Attr("SignerFromCompany");
            part.State = elem.Attr("State");
            part.ZipCode = elem.Attr("ZipCode");


            //bool parse
            part.IsCommitter = bool.Parse(elem.Attr("IsCommitter"));
            part.OfficeValidOverride = bool.Parse(elem.Attr("OfficeValidOverride"));
            part.RequiresEmployerSigner = bool.Parse(elem.Attr("RequiresEmployerSigner"));


            //date parse
            var employerSignedOnAttr = context.Attribute(part.PartDefinition.Name, "EmployerSignedOn");
            var foundationSignedOnAttr = context.Attribute(part.PartDefinition.Name, "FoundationSignedOn");
            var signedDateAttr = context.Attribute(part.PartDefinition.Name, "SignedDate");
            
            if (employerSignedOnAttr != null) {
                part.EmployerSignedOn = XmlConvert.ToDateTime(employerSignedOnAttr, XmlDateTimeSerializationMode.Utc);
            }

            if (foundationSignedOnAttr != null) {
                part.FoundationSignedOn = XmlConvert.ToDateTime(foundationSignedOnAttr, XmlDateTimeSerializationMode.Utc);
            }

            if (signedDateAttr != null) {
                part.SignedDate = XmlConvert.ToDateTime(foundationSignedOnAttr, XmlDateTimeSerializationMode.Utc);
            }

            //user parse
            var founderIdAttr = context.Attribute(part.PartDefinition.Name, "FoundationSignerId");
            if (founderIdAttr != null) {
               
                part.FoundationSigner = context.GetItemFromSession(context.Attribute(part.PartDefinition.Name, "FoundationSignerId")).As<ExtendedUserPart>().Record;
            }

            var claSignerIdAttr = context.Attribute(part.PartDefinition.Name, "CLASignerId");
            if (claSignerIdAttr != null) {
               
                part.CLASigner = context.GetItemFromSession(context.Attribute(part.PartDefinition.Name, "FoundationSignerId")).As<ExtendedUserPart>().Record;
                
            }
            

            //template parse

          //  part.TemplateId = context.GetItemFromSession(context.Attribute(part.PartDefinition.Name, "TemplateId")).Id;
           // part.TemplateVersion = int.Parse(context.Attribute(part.PartDefinition.Name, "TemplateVersion"));

        }



        #endregion
    }
}