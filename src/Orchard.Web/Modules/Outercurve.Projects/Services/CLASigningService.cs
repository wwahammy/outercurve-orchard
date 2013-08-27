using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Messaging.Services;
using Orchard.Security;
using Orchard.Services;
using Orchard.Settings;
using Outercurve.Projects.Models;
using Outercurve.Projects.ViewModels;
using Outercurve.Projects.ViewModels.CLASigning;
using Outercurve.Projects.Helpers;

namespace Outercurve.Projects.Services
{
    public interface ICLASigningService : IDependency {
        string CreateCompanySigningNonce(int claId);

        /// <summary>
        /// Takes in a resend nonce, decrypts it and gives the cla back
        /// </summary>
        /// <param name="nonce">resend nonce</param>
        /// <param name="cla"></param>
        /// <returns>true if the decryption succeeded, false otherwise</returns>
        bool DecryptResendNonce(string nonce, out ContentItem cla);

        bool DecryptCompanySigningNonce(string nonce, out int claId);
        /// <summary>
        /// Decrypts the nonce for the company co-signing
        /// </summary>
        /// <param name="nonce">nonce</param>
        /// <param name="cla">The ContentItem of the CLA referred to in the nonce.</param>
        /// <param name="validateByUtc">The date by which the nonce must be used.</param>
        /// <returns>true if the nonce can be decrypted AND it refers to a valid CLA</returns>
        bool DecryptCompanySigningNonce(string nonce, out ContentItem cla);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="claId"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        string CreateCompanySigningNonceLink(int claId, IUrlHelper helper);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <param name="helper"></param>
        /// <param name="emailToCompanySigner">An embarrassing, awful hack</param>
        /// <returns></returns>
        ContentItem SignIndividual(SignIndividualViewModel model, IUser user, IUrlHelper helper, Func<string,string,string,string,string, string> emailToCompanySigner);
        
        /// <summary>
        /// Emails the user of the invalid employer signing nonce and gives them a link to resend the link to the employer
        /// </summary>
        /// <param name="cla">The cla of the invalid nonce</param>
        /// <param name="helper">IURlhelper for creating the nonce link</param>
        void NotifyUserOfLateNonce(CLAPart cla, IUrlHelper helper);


        ContentItem SignCompany(SignCompanyViewModel model, IUrlHelper helper);


        ContentItem ResendNewNonceLink(ResendLinkViewModel model, IUrlHelper helper, Func<string, string, string, string, string, string> emailToCompanySigner);
    }


    public class CLASigningService : ICLASigningService {

        public static readonly TimeSpan DELAY = TimeSpan.FromDays(14);

        private readonly IClock _clock;
        private readonly IEncryptionService _encryptionService;
        private readonly IMessageManager _messageManager;
        private readonly IContentManager _contentManager;
        private readonly ICLAPartService _claService;
        private readonly ICLATemplateService _templateService;
        

        public CLASigningService(IClock clock, IEncryptionService encryptionService, IMessageManager messageManager, IContentManager contentManager,
                                 ICLAPartService claService, ICLATemplateService templateService ) {
            _clock = clock;
            _encryptionService = encryptionService;
            _messageManager = messageManager;
            _contentManager = contentManager;
            _claService = claService;
            _templateService = templateService;
           
        }



        public string CreateCompanySigningNonce(int claId) {
            var claPart = _contentManager.Get<CLAPart>(claId);
            var offset = _clock.UtcNow.ToUniversalTime().Add(DELAY);
            claPart.EmployerMustSignBy = offset;

            var challengeToken = new XElement("n", new XAttribute("cla", claId), new XAttribute("utc", offset.ToString(CultureInfo.InvariantCulture))).ToString();
            var data = Encoding.UTF8.GetBytes(challengeToken);
            return Convert.ToBase64String(_encryptionService.Encode(data));
        }

        public bool DecryptResendNonce(string nonce, out ContentItem cla) {
            int claId;
            cla = null;

            var decrypted = DecryptResendNonce(nonce, out claId);
            if (decrypted) {
                cla = _contentManager.Get(claId);
                return true;
            }
            return false;
        }

        private bool DecryptResendNonce(string nonce, out int claId) {
           claId = 0;

            try {
                var data = _encryptionService.Decode(Convert.FromBase64String(nonce));
                var xml = Encoding.UTF8.GetString(data);
                var element = XElement.Parse(xml);
                claId = int.Parse(element.Attribute("cla").Value);
                var companyResendAttr = element.Attr("companyResend");
                return companyResendAttr == "companyResend";
            }
            catch {
                return false;
            }
        }

        public bool DecryptCompanySigningNonce(string nonce, out int claId) {
            claId = 0;

            try {
                var data = _encryptionService.Decode(Convert.FromBase64String(nonce));
                var xml = Encoding.UTF8.GetString(data);
                var element = XElement.Parse(xml);
                claId = int.Parse(element.Attribute("cla").Value);
                return true;
            }
            catch {
                return false;
            }

        }

        public bool DecryptCompanySigningNonce(string nonce, out ContentItem cla) {
            int claId;
            cla = null;
            
            var decryptResult = DecryptCompanySigningNonce(nonce, out claId);
            if (!decryptResult)
                return false;

            cla = _contentManager.Get(claId);

            return cla != null && cla.ContentType == "CLA";
        }

        public virtual string CreateCompanySigningNonceLink(int claId, IUrlHelper helper) {

            var item = new {area = "Outercurve.Projects", nonce = CreateCompanySigningNonce(claId)};
            return helper.Action("SignCompany", "CLASigning", item, "http");
        }


       public ContentItem SignCompany(SignCompanyViewModel model, IUrlHelper helper) {
           var cla = _contentManager.Get(model.CLAId);
           var claPart = cla.As<CLAPart>();
           claPart.SignerFromCompany = model.CompanyContact;
           claPart.SignerFromCompanyEmail = model.CompanyContactEmail;
           claPart.EmployerSignedOn = _clock.UtcNow;


           _contentManager.Publish(cla);

           _messageManager.Send(new[] { claPart.SignerEmail}, "CLAMessage", "email", new Dictionary<string, string> { { "Body", String.Format("Your CLA for {0} is valid!", cla.As<CommonPart>().Container.As<TitlePart>().Title)} });
           return cla;
       }


       public ContentItem SignIndividual(SignIndividualViewModel model, IUser user, IUrlHelper helper, Func<string, string, string, string, string, string> emailToCompanySigner)
       {

            var project = _contentManager.Get(model.ProjectId);

            var claItem = _contentManager.New("CLA");
            _contentManager.Create(claItem);
            claItem.As<CommonPart>().Container = project;

            _claService.UpdateItemWithClaInfo(claItem, CreateIndividualCLA(model, user));


            _contentManager.Publish(claItem);


            SendCompanyLink(claItem, helper, model.CompanyContactEmail, model.CompanyContact, emailToCompanySigner);

         

            return claItem;


        }

       public ContentItem ResendNewNonceLink(ResendLinkViewModel model, IUrlHelper helper, Func<string, string, string, string, string, string> emailToCompanySigner)
       {
            var cla = _contentManager.Get(model.CLAId);

            var claPart = cla.As<CLAPart>();
            claPart.EmployerMustSignBy = _clock.UtcNow.ToUniversalTime().Add(DELAY);
            _contentManager.Publish(cla);
            SendCompanyLink(cla, helper, model.CompanyContactEmail,  model.CompanyContact, emailToCompanySigner);

            return cla;
        }

        public void NotifyUserOfLateNonce(CLAPart cla, IUrlHelper helper) {
            var item = new {area = "Outercurve.Projects", nonce = CreateResendLinkNonce(cla.Id)};
            var link = helper.Action("ResendLink", "CLASigning", item, "http");
            _messageManager.Send(new[] {cla.SignerEmail}, "CLAMessage", "email", new Dictionary<string, string> {{"Body", link}});
        }

        private void SendCompanyLink(ContentItem claItem, IUrlHelper helper, string companyContactEmail, string companySigner, Func<string, string, string, string, string, string> emailToCompanySigner)
        {
            if (!IsCLAPartValid(claItem) && DoesCLAPartRequireEmployerSignature(claItem))
            {
                var link = CreateCompanySigningNonceLink(claItem.Id, helper);
                var claPart = claItem.As<CLAPart>();
                dynamic createHtml = emailToCompanySigner.Invoke(claItem.As<CommonPart>().Container.As<TitlePart>().Title, claPart.FirstName, claPart.LastName, companySigner, link);
                _messageManager.Send(new[] { companyContactEmail }, "CLAMessage", "email", new Dictionary<string, string> { {"Subject", "Please sign the CLA"}, {"Sender", "eschultz@outercurve.org"},{ "Body", createHtml } });
            }
        }


       
        //protected virtual for testing
        protected virtual EditCLAViewModel CreateIndividualCLA(SignIndividualViewModel model, IUser user) {

            var claTemplate = _contentManager.Get(_contentManager.Get<ProjectPart>(model.ProjectId).CLATemplate.Id);

            var claViewModel = new EditCLAViewModel {
                CLASignerFirstName = model.FirstName,
                CLASignerLastName = model.LastName,
                CLASignerEmail = model.Email,
                Address1 = model.Address1, 
                Address2 = model.Address2, 
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                Country = model.Country,
                Employer = model.Employer,
                
                IsSignedByUser = true,
               
                SigningDate = _clock.UtcNow.ToString("d"),
                
                CLASignerUsername = user.UserName,
                NeedCompanySignature =  model.NeedCompanySignature,
                SelectedTemplate = _templateService.CreateCLATemplateIdVersion(claTemplate)
                
            };


            return claViewModel;
        }

        protected virtual string CreateResendLinkNonce(int claId) {
            var challengeToken = new XElement("n", new XAttribute("cla", claId), new XAttribute("companyResend", "companyResend")).ToString();
            var data = Encoding.UTF8.GetBytes(challengeToken);
            return Convert.ToBase64String(_encryptionService.Encode(data));
        }




        //protected virtual for Testing
        protected virtual bool IsCLAPartValid(ContentItem item) {
            return item.As<CLAPart>().IsValid;
        }

        //protected virtual for Testing
        protected virtual bool DoesCLAPartRequireEmployerSignature(ContentItem item) {
            return item.As<CLAPart>().RequiresEmployerSigner;
        }

    }
}