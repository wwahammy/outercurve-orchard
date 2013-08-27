using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Security;
using Orchard.Users.Models;
using Outercurve.Projects.Models;
using Outercurve.Projects.ViewModels;

namespace Outercurve.Projects.Services
{
    public interface ICLAPartService : IDependency
    {
       void UpdateItemWithClaInfo(ContentItem item, EditCLAViewModel model);
    }
    public class CLAPartService : ICLAPartService
    {
        
        private readonly IRepository<UserPartRecord> _userRepository;

        private readonly IContentManager _contentManager;
        private readonly IUTCifierService _utcService;
        private readonly IMembershipService _membership;
        private readonly ICLATemplateService _templateService;


        public CLAPartService(IRepository<UserPartRecord> user,
            IContentManager contentManager,
            IUTCifierService utcService, IMembershipService membership, ICLATemplateService templateService)
        {
            _userRepository = user;
            
            _contentManager = contentManager;
            _utcService = utcService;
            _membership = membership;
            _templateService = templateService;
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


            var template = _templateService.GetCLATemplateFromIdVersion(model.SelectedTemplate);
            part.CLATemplate = template;

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