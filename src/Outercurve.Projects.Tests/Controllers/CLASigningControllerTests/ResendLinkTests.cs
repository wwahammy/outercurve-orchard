using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ExpectedObjects;
using FluentAssertions;
using Moq;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Common.Utilities;
using Orchard.Core.Title.Models;
using Outercurve.Projects.Helpers;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels.CLASigning;
using Proligence.Orchard.Testing;
using SpecsFor.ShouldExtensions;
using Xunit;

namespace Outercurve.Projects.Tests.CLASigningControllerTests
{
    public class ResendLinkTests : CLASigningControllerFixture
    {
        public class S {
            public const string VALID_NONCE = "VALID_NONCE";
            public const string INVALID_NONCE = "INVALID_NONCE";
            public const string VALID_BUT_NOTCLA_NONCE = "VALID_BUT_NOTCLA";
            public const string PROJECTNAME = "projectName";
            public const string COMPANY_CONTACT = "COMPANY_CONTACT";
            public const string COMPANY_EMAIL = "COMPANY_EMAIL";
            public const string COMPANY_SIGNING_LINK = "COMPANY SIGNING LINK!!!!!";


        }

        public class I {
            public const int VALIDITEM_NOT_CLA = 1;
            public const int PROJECT_ID = 2;
            public const int VALID_CLA = 3;
        }


        public ResendLinkTests() {
            Setup();
        }

        private void Setup() {
            ContentItem invalidCla = null;
            _mockSigningService.Setup(c => c.DecryptResendNonce(S.INVALID_NONCE, out invalidCla)).Returns(false);


            
            var notCLA = ContentFactory.CreateContentItem(I.VALIDITEM_NOT_CLA, "NotCLA");
            _mockSigningService.Setup(c => c.DecryptResendNonce(S.VALID_BUT_NOTCLA_NONCE, out notCLA)).Returns(true);

            var container = ContentFactory.CreateContentItem(I.PROJECT_ID, "Project", new TitlePart {Record = new TitlePartRecord {Title = S.PROJECTNAME}});

            var isCLA = ContentFactory.CreateContentItem(I.VALID_CLA, "CLA", new CommonPart {Container = container}, new CLAPart());
            _mockSigningService.Setup(c => c.DecryptResendNonce(S.VALID_NONCE, out isCLA)).Returns(true);
            _mockContent.ExpectGetItem(isCLA);

        }

        #region GET
        [Fact]
        public void InvalidNonce() {

            var result = Run<ViewResult>(c => c.ResendLink(S.INVALID_NONCE));
            result.ViewName.Should().Be("InvalidNonce");
           
        }


        [Fact]
        public void ValidNonce_NotCLA() {
            var result = Run<ViewResult>(c => c.ResendLink(S.VALID_BUT_NOTCLA_NONCE));
            result.ViewName.Should().Be("InvalidNonce");
        }


        [Fact]
        public void ValidNonce() {
            var result = Run<ViewResult>(c => c.ResendLink(S.VALID_NONCE));
            result.ViewName.Should().Be("ResendLink");
            var model = result.Model as ResendLinkViewModel;

            model.Should().NotBeNull();

            var shouldBe = new ResendLinkViewModel {CLAId = I.VALID_CLA, ProjectName = S.PROJECTNAME}.ToExpectedObject();
            model.Should().Match(o => shouldBe.IsEqualTo(o));
        }

        #endregion

        #region POST

        [Fact]
        public void InvalidCLAPost_Test() {
            _controller.ValueProvider = CreateValidFormCollection(I.VALIDITEM_NOT_CLA).ToValueProvider();

            var result = Run<ViewResult>(c => c.ResendLinkPOST());
            result.ViewName.Should().Be("InvalidNonce");
            _mockSigningService.Verify(s => s.ResendNewNonceLink(It.IsAny<ResendLinkViewModel>(), It.IsAny<IUrlHelper>(), It.IsAny<CreateHtmlForCompanySignerEmail>()), Times.Never());
        }

        [Fact]
        public void InvalidAnswersPost_Test() {
            _controller.ValueProvider = CreateInvalidFormCollection(I.VALID_CLA).ToValueProvider();
            var result = Run<ViewResult>(c => c.ResendLinkPOST());
            result.ViewName.Should().Be("ResendLink");
            _controller.ModelState["CompanyContact"].Errors.Count.Should().Be(1);
            _controller.ModelState["CompanyContactEmail"].Errors.Count.Should().Be(1);
        }

        [Fact]
        public void ValidAnswersPost_Test() {
            _mockSigningService.Setup(c => c.CreateCompanySigningNonceLink(I.VALID_CLA, It.IsAny<IUrlHelper>())).Returns(S.COMPANY_SIGNING_LINK);

            _controller.ValueProvider = CreateValidFormCollection(I.VALID_CLA).ToValueProvider();
            var result = Run<ViewResult>(c => c.ResendLinkPOST());
            result.ViewName.Should().Be("StillNeedCompany");
            var expectedVm = new ResendLinkViewModel() {
                CLAId = I.VALID_CLA,
                CompanyContactEmail = S.COMPANY_EMAIL,
                CompanyContact = S.COMPANY_CONTACT,
            };
            
            _mockSigningService.Verify(s => s.ResendNewNonceLink(Looks.Like(expectedVm), It.IsAny<IUrlHelper>(), It.IsAny<CreateHtmlForCompanySignerEmail>()), Times.Once());
            
            
            var stillNeedCompanyResult = result.Model as StillNeedCompanyViewModel;
            stillNeedCompanyResult.Contact.Should().Be(S.COMPANY_CONTACT);
            stillNeedCompanyResult.Email.Should().Be(S.COMPANY_EMAIL);
            stillNeedCompanyResult.Project.Should().Be(S.PROJECTNAME);
            stillNeedCompanyResult.CompanySigningLink.Should().Be(S.COMPANY_SIGNING_LINK);
        }

        private FormCollection CreateInvalidFormCollection(int? claId)
        {
            var form = new FormCollection();
            form["CLAId"] = claId.ToString();

            form["CompanyContact"] = "";
            form["CompanyContactEmail"] = "";
            return form;
        }

         private FormCollection CreateValidFormCollection(int? claId)
        {
            var form = new FormCollection();
            form["CLAId"] = claId.ToString();

             form["CompanyContact"] = S.COMPANY_CONTACT;
             form["CompanyContactEmail"] = S.COMPANY_EMAIL;
            return form;
        }
        #endregion
    }
}
