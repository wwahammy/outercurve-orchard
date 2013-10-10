using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Moq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Common.Models;
using Orchard.Messaging.Services;
using Orchard.Security;
using Orchard.Services;
using Outercurve.Projects.Helpers;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;
using Outercurve.Projects.ViewModels.CLASigning;
using Outercurve.Projects.ViewModels.Parts;
using SpecsFor.ShouldExtensions;
using Xunit;
using Moq.Protected;

namespace Outercurve.Projects.Tests.Services.ClaSigningServiceTests
{
    public class ClaSigningServiceTest {
        private readonly Mock<CLASigningService> _signingService;

        private readonly MockRepository _mockFactory;

        private readonly Mock<IClock> _mockClock;
        private readonly Mock<IEncryptionService> _mockEncryption;
        private readonly Mock<IMessageManager> _mockMessage;
        private readonly Mock<IContentManager> _mockContent;
        private readonly Mock<ContentItem> _mockProject;
        private readonly Mock<ICLAPartService> _mockCla;
        private readonly Mock<IUrlHelper> _mockUrlHelper;
        private readonly Mock<ICLATemplateService> _mockTemplateService;
        
        private readonly ContentItem _claContentItem;


        private const int CREATED_CLA_CONTENT_ITEM_ID = 99999;


        private const string URL_TO_RETURN = "fake://fake.url/fake?nonce=";

        private const string CREATE_HTML = "Create HTML Value";
        public ClaSigningServiceTest() {


            _mockFactory = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            _mockClock = _mockFactory.Create<IClock>();
            _mockClock.SetupGet(c => c.UtcNow).Returns(new DateTime(2013, 1, 1));

            _mockEncryption = _mockFactory.Create<IEncryptionService>();
            _mockEncryption.Setup(e => e.Encode(It.IsAny<byte[]>())).Returns<byte[]>(input => input.Reverse().ToArray());
            _mockEncryption.Setup(e => e.Decode(It.IsAny<byte[]>())).Returns<byte[]>(input => input.Reverse().ToArray());


            _mockMessage = _mockFactory.Create<IMessageManager>();
            _mockMessage
                .Setup(m => m.Send(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()));


            _mockProject = _mockFactory.Create<ContentItem>(MockBehavior.Default);


            _mockContent = _mockFactory.Create<IContentManager>(MockBehavior.Loose);

            _claContentItem = new ContentItemBuilder(new ContentTypeDefinitionBuilder().Named("CLA").Build()).
                Weld<CommonPart>().Weld<CLAPart>().Build();
            
            

            _mockContent
                .Setup(m => m.New("CLA")).Returns(_claContentItem);
            _mockContent.Setup(m => m.Get(It.IsAny<int>())).Returns(_mockProject.Object);




            _mockCla = _mockFactory.Create<ICLAPartService>(MockBehavior.Loose);
            _mockCla.Setup(s => s.UpdateItemWithClaInfo(It.IsAny<ContentItem>(), It.IsAny<EditCLAViewModel>()));

            _mockUrlHelper = _mockFactory.Create<IUrlHelper>();
            _mockUrlHelper.Setup(u => u.Action("SignCompany", "CLASigning", It.IsAny<RouteValueDictionary>()))
                          .Returns<string, string, RouteValueDictionary>((action, controller, rv) => "URL_TO_RETURN" + rv["nonce"]);

            _mockTemplateService = _mockFactory.Create<ICLATemplateService>();

            _signingService = _mockFactory.Create<CLASigningService>(MockBehavior.Loose, _mockClock.Object, _mockEncryption.Object, _mockMessage.Object, _mockContent.Object, _mockCla.Object, _mockTemplateService.Object);
            
        }

        [Fact]
        public void CreateSignIndividualWithoutCompanyTest() {
            
            var userMock = new Mock<IUser>();
            const string USERNAME = "TEST_USERNAME";

            var inputModel = new SignIndividualViewModel {
                ProjectId = 0,
                Address1 = "ADDRESS1",
                Address2 = "ADDRESS2",
                City = "CITY",
                State = "STATE",
                Country =  "COUNTRY",
                Employer = "EMPLOYER",
                FirstName = "FIRSTNAME",
                LastName = "LASTNAME",
   
                Email = "EMAIL",
                
                NeedCompanySignature =  false
                
            };




            var outputModel = new EditCLAViewModel {
                Address1 = inputModel.Address1,
                Address2 = inputModel.Address2,
                City = inputModel.City,
                State = inputModel.State,
                ZipCode = inputModel.ZipCode,
                Country = inputModel.Country,
                Employer = inputModel.Employer,
                CLASignerFirstName = inputModel.FirstName,
                CLASignerLastName = inputModel.LastName,
                CLASignerEmail = inputModel.Email,
                IsSignedByUser = true,
              

                SigningDate = _mockClock.Object.UtcNow.ToString("d"),
                CLASignerUsername = USERNAME,
                NeedCompanySignature = inputModel.NeedCompanySignature,
                
            };


            _signingService.Protected().Setup<bool>("IsCLAPartValid", _claContentItem).Returns(true);

            _signingService.Protected().Setup<EditCLAViewModel>("CreateIndividualCLA", inputModel, userMock.Object).Returns(outputModel);





            userMock.SetupGet(u => u.UserName).Returns(USERNAME);

            var ret = _signingService.Object.SignIndividual(inputModel, userMock.Object, _mockUrlHelper.Object, (name, firstName, lastName, signer, link) =>  "");

           
            Assert.Same(_claContentItem, ret);

            Assert.Same(_claContentItem.As<CommonPart>().Container, _mockProject.Object);
            Assert.DoesNotThrow(() => _mockCla.Verify(c => c.UpdateItemWithClaInfo(_claContentItem, outputModel)));


            Assert.DoesNotThrow(() => _mockMessage.Verify(m => m.Send(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never()));
            _mockFactory.Verify();
        }


        [Fact]
        public void CreateSignIndividualWithCompanyTest()
        {

            var userMock = _mockFactory .Create<IUser>();
            const string USERNAME = "TEST_USERNAME";

            var inputModel = new SignIndividualViewModel
            {
                ProjectId = 0,
                Address1 = "ADDRESS1",
                Address2 = "ADDRESS2",
                City = "CITY",
                State = "STATE",
                Country = "COUNTRY",
                Employer = "EMPLOYER",


                NeedCompanySignature = true,
                CompanyContact = "COMPANYCONTACT",
                CompanyContactEmail = "COMPANYCONTACTEMAIL"
                

            };




            var outputModel = new EditCLAViewModel
            {
                Address1 = inputModel.Address1,
                Address2 = inputModel.Address2,
                City = inputModel.City,
                State = inputModel.State,
                ZipCode = inputModel.ZipCode,
                Country = inputModel.Country,
                Employer = inputModel.Employer,

                IsSignedByUser = true,
              

                SigningDate = _mockClock.Object.UtcNow.ToString("d"),
                CLASignerUsername = USERNAME,
                HasCompanySigner = inputModel.NeedCompanySignature,
                CompanySigner = inputModel.CompanyContact,
               
            };



            _signingService.Protected().Setup<bool>("IsCLAPartValid", _claContentItem).Returns(false);

            _signingService.Protected().Setup<EditCLAViewModel>("CreateIndividualCLA", inputModel, userMock.Object).Returns(outputModel);

            _signingService.Protected().Setup<bool>("DoesCLAPartRequireEmployerSignature", _claContentItem).Returns(true);
            
            var link = _signingService.Object.CreateCompanySigningNonceLink(_claContentItem.Id, _mockUrlHelper.Object);
            SetupMessageSendingStuff(CREATE_HTML);

            userMock.SetupGet(u => u.UserName).Returns(USERNAME);

            var ret = _signingService.Object.SignIndividual(inputModel, userMock.Object, _mockUrlHelper.Object, EmailToCompanySigner);

            Assert.Same(_claContentItem, ret);

            Assert.Same(_claContentItem.As<CommonPart>().Container, _mockProject.Object);
            Assert.DoesNotThrow(() => _mockCla.Verify(c => c.UpdateItemWithClaInfo(_claContentItem, outputModel)));




            VerifyMessageSendingStuff(CREATE_HTML);

         


            /*    m.Send(new[] { inputModel.CompanyContactEmail }, "CLAMessage", "email", 
                new Dictionary<string, string> {{"body", link}), Times.Once()t));
            */
            
        }

        private string EmailToCompanySigner(string projectName, string firstName, string lastName, string companySigner, string link) {
            return CREATE_HTML;
        }

        


        void SetupMessageSendingStuff(string body) {

     

            _mockMessage
                .Setup(m => m.Send(Looks.Like(new[] { "COMPANYCONTACTEMAIL" }), "CLAMessage", "email", Looks.Like(new Dictionary<string, string> { { "body",body }, {"Subject", "Please sign the CLA"}, {"Sender", "eschultz@outercurve.org"}})));
        }

        void VerifyMessageSendingStuff(string body)
        {
          
            _mockMessage
                .Verify(m => m.Send(Looks.Like(new[] { "COMPANYCONTACTEMAIL" }), "CLAMessage", "email", Looks.Like(new Dictionary<string, string> { { "body", body }, {"Subject", "Please sign the CLA"}, {"Sender", "eschultz@outercurve.org"} })), Times.Once());

            _mockMessage
               .Verify(m => m.Send(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Once());
        }

        



     
        
    }
}
