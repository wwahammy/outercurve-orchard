using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpectedObjects;
using Moq;
using Moq.Protected;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Services;
using Orchard.Users.Models;
using Outercurve.Projects.Drivers;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels;
using Outercurve.Projects.ViewModels.Parts;
using Proligence.Orchard.Testing;
using SpecsFor.ShouldExtensions;
using Xunit;

namespace Outercurve.Projects.Tests.CLAAdminControllerTests
{
    public class EditorTests : ClaPartDriverTestFixture {

        private ExtendedUserPartRecord _signer, _foundationSigner = null;
        private CLAPart _claPart = null;
        public class Strings
        {
            public const string EMAIL = "Strings.EMAIL";
            public const string FIRSTNAME = "Strings.FIRSTNAME";
            public const string LASTNAME = "Strings.LASTNAME";
            public const string USERNAME = "USERNAME";
            public const string NORMALIZEDUSERNAME = "username";
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
            public const string COMMENTS = "COMMENTS";
        }

        public static class Ints {
            public const int CLAId = 1;
            public const int SignerId = 2;
            public const int FoundationId = 3;

            public const int ProjectId = 4;

        }

        

        public EditorTests() {
            RunSetup();
        }

        private void RunSetup() {
            _signer = new ExtendedUserPartRecord {AutoRegistered = false, FirstName = Strings.FIRSTNAME, LastName = Strings.LASTNAME, Id = Ints.SignerId};
            var userpart = new UserPartRecord { NormalizedUserName = Strings.NORMALIZEDUSERNAME, Id = Ints.SignerId };
            _mockContentManager.ExpectGetItem(ContentFactory.CreateContentItem(Ints.SignerId, new ExtendedUserPart { Record = _signer}, new UserPart { Record = userpart}));
            _mockClock.Setup(c => c.UtcNow).Returns(new DateTime(100));

        }
        [Fact]
        public void EditorBasic() {
            CreateCLAPart();
            _mockDriver.Protected().Setup<DriverResult>("CreateShape", ItExpr.IsAny<EditCLAViewModel>(), ItExpr.IsAny<object>());


           Assert.DoesNotThrow(() => claDriver.InvokeEditor(_claPart, ShapeFactory));

         //  _mockDriver.Protected().Verify("CreateShape", Times.Once(), ItExpr.IsAny<EditCLAViewModel>(), ItExpr.IsAny<object>());

          _mockDriver.Protected().Verify("CreateShape", Times.Once(), ItExpr.Is<EditCLAViewModel>(i => ValidFromBuild().ToExpectedObject().IsEqualTo(i)), ItExpr.IsAny<object>());
            
           
            
        }




        private EditCLAViewModel ValidFromBuild() {
            var ret = new EditCLAViewModel {
                Id = Ints.CLAId,
                CLASignerUsername = Strings.NORMALIZEDUSERNAME,
                Address1 = Strings.ADDRESS1,
                Address2 = Strings.ADDRESS2,
                City = Strings.CITY,
                State = Strings.STATE,
                Country = Strings.COUNTRY,
                CLASignerFirstName = Strings.FIRSTNAME,
                CLASignerLastName = Strings.LASTNAME,
                CLASignerEmail = Strings.EMAIL,
                Comments = Strings.COMMENTS,
                Employer = Strings.EMPLOYER,
                HasFoundationSigner = false,
                HasCompanySigner = false,
                NeedCompanySignature = false,
                StaffOverride = false,
                LocationOfCLA = null,
                CompanySigningDate = _mockClock.Object.UtcNow.ToString("d"),
                FoundationSigningDate = _mockClock.Object.UtcNow.ToString("d"),
                SigningDate = _mockClock.Object.UtcNow.ToString("d")
            };

            return ret;
        }

        public void CreateCLAPart() {
            var claPartRecord = new CLAPartRecord {
                Address1 = Strings.ADDRESS1,
                Address2 = Strings.ADDRESS2,
                City = Strings.CITY,
                State = Strings.STATE,
                Country = Strings.COUNTRY,
                FirstName = Strings.FIRSTNAME,
                LastName = Strings.LASTNAME,
                CLASigner = _signer,
                Comments = Strings.COMMENTS,
                IsCommitter = false,
                SignedDate = null,
                FoundationSignedOn = null,
                Employer = Strings.EMPLOYER,

                RequiresEmployerSigner = false,
                LocationOfCLA =  null,
                EmployerMustSignBy = null,
                OfficeValidOverride = false,
                SignerEmail = Strings.EMAIL
            };




            var item = ContentFactory.CreateContentItem(Ints.CLAId, new CLAPart {Record = claPartRecord});
            _claPart = item.As<CLAPart>();

            _mockContentManager.ExpectGetItem(item);

            
        }
    }
}
