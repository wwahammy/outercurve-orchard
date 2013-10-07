using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Users.Models;
using Outercurve.Projects.Models;
using Outercurve.Projects.ViewModels;
using Outercurve.Projects.ViewModels.Parts;

namespace Outercurve.Projects.Services
{
    public interface ICLAPartService : IDependency
    {
       void UpdateItemWithClaInfo(ContentItem item, EditCLAViewModel model);
        bool Validate(EditCLAViewModel model, IUpdateModel updater);
    }
    public class CLAPartService : ICLAPartService
    {
        
        private readonly IRepository<UserPartRecord> _userRepository;

        private readonly IContentManager _contentManager;
        private readonly IUTCifierService _utcService;
        private readonly IMembershipService _membership;
        private readonly ICLATemplateService _templateService;
        private readonly Localizer T;


        public CLAPartService(IRepository<UserPartRecord> user,
            IContentManager contentManager,
            IUTCifierService utcService, IMembershipService membership, ICLATemplateService templateService)
        {
            _userRepository = user;
            
            _contentManager = contentManager;
            _utcService = utcService;
            _membership = membership;
            _templateService = templateService;

            T = NullLocalizer.Instance;
        }


        public bool Validate(EditCLAViewModel model, IUpdateModel updater) {
            bool hasError = false;
            //validate whether foundation and CLASigner are valid
            if (model.HasFoundationSigner) {
                if (_membership.GetUser(model.FoundationSignerUsername) == null) {
                    updater.AddModelError(model, m => m.FoundationSignerUsername, T("Foundation signer username does not belong to a valid user."));
                    hasError = true;
                }
            }

            if (_membership.GetUser(model.CLASignerUsername) == null) {
                updater.AddModelError(model, m => m.CLASignerUsername, T("The CLA signer username does not belong to a valid user"));
                hasError = true;
            }

            return !hasError;
        }

        public void UpdateItemWithClaInfo(ContentItem item, EditCLAViewModel model) {
            var part = item.As<CLAPart>();

            part.CLASigner = _membership.GetUser(model.CLASignerUsername).As<ExtendedUserPart>().Record;
            part.Comments = model.Comments;

            part.FirstName = model.CLASignerFirstName;
            part.LastName = model.CLASignerLastName;
            part.SignerEmail = model.CLASignerEmail;

            part.IsCommitter = model.IsCommitter;
            part.Employer = model.Employer;
            part.LocationOfCLA = model.LocationOfCLA;
            part.Address1 = model.Address1;
            part.Address2 = model.Address2;
            part.City = model.City;
            part.State = model.State;
            part.ZipCode = model.ZipCode;
            part.Country = model.Country;

            part.RequiresEmployerSigner = model.NeedCompanySignature;


       

            if (model.IsSignedByUser) {
                part.SignedDate = _utcService.GetUtcFromLocalDate(model.SigningDate);
            }
            else {
                part.SignedDate = null;
            }

            if (model.HasFoundationSigner) {
                part.FoundationSigner = _membership.GetUser(model.FoundationSignerUsername).As<ExtendedUserPart>().Record;
                part.FoundationSignedOn = _utcService.GetUtcFromLocalDate(model.FoundationSigningDate);
            }


            if (model.HasCompanySigner) {

                part.SignerFromCompany = model.CompanySigner;
                part.SignerFromCompanyEmail = model.CompanySignerEmail;
                part.EmployerSignedOn = _utcService.GetUtcFromLocalDate(model.CompanySigningDate);
            }
        }
    }
}