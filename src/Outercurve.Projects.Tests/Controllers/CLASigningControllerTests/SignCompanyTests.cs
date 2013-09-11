using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ExpectedObjects;
using Moq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Outercurve.Projects.Helpers;
using Outercurve.Projects.Models;
using Outercurve.Projects.ViewModels.CLASigning;
using Proligence.Orchard.Testing;

using Xunit;

namespace Outercurve.Projects.Tests.CLASigningControllerTests
{
    public class SignCompanyTests : CLASigningControllerFixture
    {
        public SignCompanyTests() {
            SetupCLA();
        }


        public class Ids
        {
            public const int VALIDPROJECTID = 1;
            public const int VALIDUSER = 2;
            public const int INVALIDPROJECTID = VALIDUSER;
            public const int VALIDCLATEMPLATEID = 3;
            public const int CLAID = 4;

            public const int CLAVERSION = 1;

            public const int CURRENTUTCCLICKS = 400;
        }

        public class Strings
        {
            public const string EMAIL = "Strings.EMAIL";
            public const string FIRSTNAME = "Strings.FIRSTNAME";
            public const string LASTNAME = "Strings.LASTNAME";
            public const string USERNAME = "USERNAME";
            public const string VALIDPROJECTNAME = "PROJECTNAME";
            public const string EMPLOYER = "EMPLOYER";
            public const string ADDRESS1 = "ADDRESS1";
            public const string ADDRESS2 = "ADDRESS2";
            public const string CITY = "CITY";
            public const string STATE = "STATE";
            public const string ZIPCODE = "ZIPCODE";
            public const string COUNTRY = "COUNTRY";
            public const string CLATITLE = "CLATITLE";
            public const string CLATEXT = "CLATEXT";

            public const string VALIDNONCE = "VALIDNONCE";
            public const string INVALIDNONCE = "INVALIDNONCE";

            public const string ORIGINALSIGNERFROMCOMPANY = "SIGNERFROMCOMPANY";
            public const string ORIGINALCOMPANYEMAIL = "ORIGINALSIGNEREMAIL";
        }

        private ContentItem _claContentItem;

        [Fact]
        public void SignCompany_InvalidNonce()
        {

            ContentItem ci = It.IsAny<ContentItem>();
          
            _mockSigningService.Setup(i => i.DecryptCompanySigningNonce(Strings.INVALIDNONCE, out ci)).Returns(false);

            var view = Run<ViewResult>(c => c.SignCompany(Strings.INVALIDNONCE));

            _mockSigningService.Verify(i => i.DecryptCompanySigningNonce(Strings.INVALIDNONCE, out ci), Times.Once());

            Assert.Equal("InvalidNonce", view.ViewName);
        }
        [Fact]
        public void SignCompany_ValidNonce() {

            var ci = _claContentItem;
            var dt = new DateTime(600);
            ci.As<CLAPart>().EmployerMustSignBy = dt;
            _mockSigningService.Setup(i => i.DecryptCompanySigningNonce(Strings.VALIDNONCE, out ci)).Returns(true);

            var val = Run<ViewResult>(c => c.SignCompany(Strings.VALIDNONCE));

           _mockSigningService.Verify(i => i.DecryptCompanySigningNonce(Strings.VALIDNONCE, out ci), Times.Once());
            Assert.Equal("SignCompany", val.ViewName);

            var validVM = CreateValidVM();
            var model = val.Model as SignCompanyViewModel;
            validVM.ToExpectedObject().ShouldEqual(model);
            
           

        }

        [Fact]
        public void SignCompany_TooLate() {
            var ci = _claContentItem;
            var dt = new DateTime(0);
            ci.As<CLAPart>().EmployerMustSignBy = dt;
            _mockSigningService.Setup(i => i.DecryptCompanySigningNonce(Strings.VALIDNONCE, out ci)).Returns(true);

            var val = Run<ViewResult>(c => c.SignCompany(Strings.VALIDNONCE));

            _mockSigningService.Verify(c => c.NotifyUserOfLateNonce(ci.As<CLAPart>(), It.IsAny<IUrlHelper>()), Times.Once());

            Assert.Equal("TooLateForNonce", val.ViewName);
        }

        
        public void SetupCLA() {


            var claTemplate = ContentFactory.CreateContentItem(Ids.VALIDCLATEMPLATEID, "CLATemplate", new CommonPart(), new CLATemplatePart { Record = new CLATemplatePartRecord(), CLA = Strings.CLATEXT, CLATitle = Strings.CLATITLE });
            _mockContent.ExpectGetItem(claTemplate, Ids.CLAVERSION);
            
            var isProject = ContentFactory.CreateContentItem(Ids.VALIDPROJECTID, "Project", new CommonPart(),
                new ProjectPart { Record = new ProjectPartRecord(), CLATemplate = claTemplate.As<CLATemplatePart>().Record },
            new TitlePart { Record = new TitlePartRecord(), Title = Strings.VALIDPROJECTNAME });
            _mockContent.ExpectGetItem(isProject);
           
            var claPart =  new CLAPart {
                                                          Record = new CLAPartRecord 
                                                          { 
                                                              Address1 = Strings.ADDRESS1, Address2 = Strings.ADDRESS2, City = Strings.CITY, State = Strings.STATE, 
                                                              Country = Strings.COUNTRY, Employer = Strings.EMPLOYER, ZipCode = Strings.ZIPCODE,
                                                              RequiresEmployerSigner = true, 
                                                              SignerEmail = Strings.EMAIL,
                                                              TemplateId =  Ids.VALIDCLATEMPLATEID,
                                                              TemplateVersion = Ids.CLAVERSION,
                                                              FirstName = Strings.FIRSTNAME,
                                                              LastName = Strings.LASTNAME,
                                                              
                                                              
                                                          }};
            claPart.CLATemplateField.Loader(() => claTemplate);


            _claContentItem = ContentFactory.CreateContentItem(Ids.CLAID, "CLA",
                                                      new CommonPart {Record = new CommonPartRecord {Container = isProject.Record}, Container = isProject}, claPart);

            _mockContent.ExpectGetItem(_claContentItem);

            _mockClock.SetupGet(c => c.UtcNow).Returns(new DateTime(Ids.CURRENTUTCCLICKS));

        }


        private SignCompanyViewModel CreateValidVM() {

           

            var model = new SignCompanyViewModel
            {
                CLAId = Ids.CLAID,
                CLA = Strings.CLATEXT,
                FirstName = Strings.FIRSTNAME,
                LastName = Strings.LASTNAME,
                Address1 = Strings.ADDRESS1,
                Address2 = Strings.ADDRESS2,
                City = Strings.CITY,
                ZipCode = Strings.ZIPCODE,
                State = Strings.STATE,
                Country = Strings.COUNTRY,
                Email = Strings.EMAIL,
                ProjectId = Ids.VALIDPROJECTID,
                ProjectName = Strings.VALIDPROJECTNAME
            };

            return model;
        }

    }
}
