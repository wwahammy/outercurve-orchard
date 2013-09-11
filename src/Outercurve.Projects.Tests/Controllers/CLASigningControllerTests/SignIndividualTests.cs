using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Outercurve.Projects.Models;
using Outercurve.Projects.ViewModels.CLASigning;
using Proligence.Orchard.Testing;
using Xunit;

namespace Outercurve.Projects.Tests.CLASigningControllerTests
{
    public class SignIndividualTests : CLASigningControllerFixture
    {
        public SignIndividualTests() {
            CreateProjects();
        }

        public class Ids {
            public const int VALIDPROJECTID = 1;
            public const int VALIDUSER = 2;
            public const int INVALIDPROJECTID = VALIDUSER;
            public const int VALIDCLATEMPLATEID = 3;
        }

        public class Strings {
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
        }

        #region SignIndividual

        [Fact]
        public void SignIndividualWithCurrentUserNoCountersignTest()
        {

            SetupValidUser();
            var result = Run<ViewResult>(c => c.SignIndividual(Ids.VALIDPROJECTID));

            var vm = result.Model as SignIndividualViewModel;

            Assert.Equal(Strings.FIRSTNAME, vm.FirstName);
            Assert.Equal(Strings.LASTNAME, vm.LastName);
            Assert.Equal(Ids.VALIDPROJECTID, vm.ProjectId);
            Assert.Equal(Strings.VALIDPROJECTNAME, vm.ProjectName);
            Assert.False(vm.NeedCompanySignature);

        }

        [Fact]
        public void SignIndividualWithNoCurrentUserNoCountersignTest()
        {

            
            var result = Run<ViewResult>(c => c.SignIndividual(Ids.VALIDPROJECTID));

            var vm = result.Model as SignIndividualViewModel;
            Assert.Equal(Ids.VALIDPROJECTID, vm.ProjectId);
            Assert.Equal(Strings.VALIDPROJECTNAME, vm.ProjectName);
            Assert.False(vm.NeedCompanySignature);
        }



        [Fact]
        public void SignIndividualWithCurrentUserWithCountersignTest()
        {

            SetupValidUser();
            var result = Run<ViewResult>(c => c.SignIndividual(Ids.VALIDPROJECTID, true));

            var vm = result.Model as SignIndividualViewModel;

            Assert.Equal(Strings.FIRSTNAME, vm.FirstName);
            Assert.Equal(Strings.LASTNAME, vm.LastName);
            Assert.Equal(Ids.VALIDPROJECTID, vm.ProjectId);
            Assert.Equal(Strings.VALIDPROJECTNAME, vm.ProjectName);
            Assert.True(vm.NeedCompanySignature);

        }


        [Fact]
        public void SignIndividualWithNoCurrentUserCountersignTest()
        {

           
            var result = Run<ViewResult>(c => c.SignIndividual(Ids.VALIDPROJECTID, true));


            var vm = result.Model as SignIndividualViewModel;
            Assert.Equal(Ids.VALIDPROJECTID, vm.ProjectId);
            Assert.Equal(Strings.VALIDPROJECTNAME, vm.ProjectName);
            Assert.True(vm.NeedCompanySignature);
        }



        [Fact]
        public void SignIndividualWithBadProjectId()
        {
            BadProjectNotify();
            var getRoute = Run<RedirectToRouteResult>(c => c.SignIndividual(Ids.INVALIDPROJECTID));
            VerifyBadProjectRedirect(getRoute);

        }


        private void SetupValidUser()
        {


            var user = UserFactory.CreateUser(new UserCreationArgs { Id = Ids.VALIDUSER, Email = Strings.EMAIL, LastName = Strings.LASTNAME, FirstName = Strings.FIRSTNAME, Username = Strings.USERNAME });

            _mockAuthService.Setup(auth => auth.GetAuthenticatedUser()).Returns(user);



            //  var i = _mockServices.WorkContext.CurrentUser;
        }


        public void CreateProjects()
        {



            var claTemplate = ContentFactory.CreateContentItem(Ids.VALIDCLATEMPLATEID, "CLATemplate", new CommonPart(), new CLATemplatePart { Record = new CLATemplatePartRecord(), CLA = Strings.CLATEXT, CLATitle = Strings.CLATITLE });
            _mockContent.ExpectGetItem(claTemplate);

            var isProject = ContentFactory.CreateContentItem(Ids.VALIDPROJECTID, "Project", new CommonPart(),
                new ProjectPart { Record = new ProjectPartRecord(), CLATemplate = claTemplate.As<CLATemplatePart>().Record },
                new TitlePart { Record = new TitlePartRecord(), Title = Strings.VALIDPROJECTNAME });
            _mockContent.ExpectGetItem(isProject);


            //var notProject = new ContentItemBuilder(new ContentTypeDefinitionBuilder().Named("SomethingElse").Build()).Weld<CommonPart>().Build();


            //CLATemplate = new CLATemplatePartRecord { CLA = VALIDCLA } }, new TitlePart { Record = new TitlePartRecord(), Title = VALIDPROJECTNAME }
        }

        #endregion

    }
}
