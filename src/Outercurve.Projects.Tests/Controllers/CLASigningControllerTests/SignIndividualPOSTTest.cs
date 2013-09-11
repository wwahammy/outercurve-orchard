using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Security;
using Outercurve.Projects.Helpers;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels.CLASigning;
using Proligence.Orchard.Testing;
using SpecsFor.ShouldExtensions;
using Xunit;

namespace Outercurve.Projects.Tests.CLASigningControllerTests
{
    public class SignIndividualPOSTTest : CLASigningControllerFixture
    {
        public SignIndividualPOSTTest() {
            CreateProjects();
        }


        public class Ids
        {
            public const int VALIDPROJECTID = 1;
            public const int VALIDUSER = 2;
            public const int INVALIDPROJECTID = VALIDUSER;
            public const int VALIDCLATEMPLATEID = 3;
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
        }


       

        [Fact]
        public void SignIndividualPOST_LoggedInUser_IndividualOnly()
        {
            SetupValidUser();
       
            CreateGoodTryUpdateModel();
        



            var view = Run<ViewResult>(c => c.SignIndividualPOST());


            Assert.Equal("SigningFinished", view.ViewName);
            Assert.Equal(Strings.VALIDPROJECTNAME, (view.Model as SigningFinishedViewModel).Project);

            Assert.True(_controller.ModelState.IsValid);

            _mockSigningService.Verify(i => i.SignIndividual(Looks.Like(GetGoodModel()), _mockAuthService.Object.GetAuthenticatedUser(),
                It.IsAny<IUrlHelper>() //this should work using _controller.UrlHelper but doesn't. I wonder why?,
                ,It.IsAny<CreateHtmlForCompanySignerEmail>()
                ), Times.Once());
        }

        [Fact]
        public void SignIndividualPOST_NewUser_IndividualOnly()
        {
            CreateGoodTryUpdateModel();
          
            var mockUser = new Mock<IUser>();
            mockUser.SetupGet(m => m.Email).Returns(Strings.EMAIL);

            _mockExtService.Setup(m => m.CreateAutoRegisteredUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockUser.Object);


            var view = Run<ViewResult>(c => c.SignIndividualPOST());


            _mockExtService.Verify(m => m.CreateAutoRegisteredUser(Strings.EMAIL, Strings.FIRSTNAME, Strings.LASTNAME), Times.Once());
            _mockExtService.Verify(m => m.CreateAutoRegisteredUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());

            _mockAuthService.Verify(i => i.SignIn(mockUser.Object, true), Times.Once());
            _mockAuthService.Verify(i => i.SignIn(It.IsAny<IUser>(), It.IsAny<bool>()), Times.Once());

            _mockUserEventHandler.Verify(i => i.LoggedIn(mockUser.Object), Times.Once());

            _mockSigningService.Verify(i => i.SignIndividual(Looks.Like(GetGoodModel()), mockUser.Object,
                 It.IsAny<IUrlHelper>() //this should work using _controller.UrlHelper but doesn't. I wonder why?
                 , It.IsAny<CreateHtmlForCompanySignerEmail>()
                 ), Times.Once());

            Assert.Equal("SigningFinished", view.ViewName);
            Assert.Equal(Strings.VALIDPROJECTNAME, ((SigningFinishedViewModel)view.Model).Project);
        }


        [Fact]
        public void SignIndividualPOST_CantCreateNewUser_IndividualOnly()
        {
            
            CreateGoodTryUpdateModel();
          
            var mockUser = new Mock<IUser>();
            mockUser.SetupGet(m => m.Email).Returns(Strings.EMAIL);

            _mockExtService.Setup(m => m.CreateAutoRegisteredUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(() => null);


            var view = Run<ViewResult>(c => c.SignIndividualPOST());



            _mockExtService.Verify(m => m.CreateAutoRegisteredUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());


            _mockAuthService.Verify(i => i.SignIn(It.IsAny<IUser>(), It.IsAny<bool>()), Times.Never());

            _mockUserEventHandler.Verify(i => i.LoggedIn(It.IsAny<IUser>()), Times.Never());

            _mockSigningService.Verify(i => i.SignIndividual(It.IsAny<SignIndividualViewModel>(), It.IsAny<IUser>(),
                 It.IsAny<IUrlHelper>(), It.IsAny <CreateHtmlForCompanySignerEmail >() //this should work using _controller.UrlHelper but doesn't. I wonder why?
                 ), Times.Never());

            _mockTransaction.Verify(t => t.Cancel(), Times.Once());



            Assert.Equal("SignIndividual", view.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(1, _controller.ModelState["Email"].Errors.Count);
        }



        [Fact]
        public void SignIndividualPOST_NullProjectId_EverythingElseFine()
        {
          
            CreateNullProject_TryUpdateModel();
        
            BadProjectNotify();

            var view = Run<RedirectToRouteResult>(c => c.SignIndividualPOST());
            VerifyBadProjectRedirect(view);


        }

        [Fact]
        public void SignIndividualPOST_BadProjectId_EverythingElseFine()
        {
           
            CreateBadProject_TryUpdateModel();
         
            BadProjectNotify();

            var view = Run<RedirectToRouteResult>(c => c.SignIndividualPOST());
            VerifyBadProjectRedirect(view);
        }


       

        private void CreateGoodTryUpdateModel()
        {


            var form = CreateValidFormCollection(Ids.VALIDPROJECTID);


            _controller.ValueProvider = form.ToValueProvider();

        }

        private void CreateBadProject_TryUpdateModel()
        {
            var form = CreateValidFormCollection(Ids.INVALIDPROJECTID);


            _controller.ValueProvider = form.ToValueProvider();
        }

        private void CreateNullProject_TryUpdateModel()
        {
            var form = CreateValidFormCollection(null);


            _controller.ValueProvider = form.ToValueProvider();
        }

        private FormCollection CreateValidFormCollection(int? projectId)
        {
            var form = new FormCollection();
            form["ProjectId"] = projectId.ToString();

            form["FirstName"] = Strings.FIRSTNAME;
            form["LastName"] = Strings.LASTNAME;
            form["Email"] = Strings.EMAIL;
            form["Employer"] = Strings.EMPLOYER;
            form["Address1"] = Strings.ADDRESS1;
            form["Address2"] = Strings.ADDRESS2;
            form["City"] = Strings.CITY;
            form["State"] = Strings.STATE;
            form["ZipCode"] = Strings.ZIPCODE;
            form["Country"] = Strings.COUNTRY;
            return form;
        }


        private SignIndividualViewModel GetGoodModel()
        {
            return GetModel(Ids.VALIDPROJECTID);

        }

        private SignIndividualViewModel GetBadModel()
        {
            return GetModel(Ids.INVALIDPROJECTID);
        }

        private SignIndividualViewModel GetModel(int projectId)
        {
            return new SignIndividualViewModel
            {
                ProjectId = projectId,
                Address1 = Strings.ADDRESS1,
                Address2 = Strings.ADDRESS2,
                FirstName = Strings.FIRSTNAME,
                LastName = Strings.LASTNAME,
                Email = Strings.EMAIL,
                Employer = Strings.EMPLOYER,
                City = Strings.CITY,
                State = Strings.STATE,
                ZipCode = Strings.ZIPCODE,
                Country = Strings.COUNTRY

            };

        }



        public void CreateProjects()
        {


            var claTemplate = ContentFactory.CreateContentItem(Ids.VALIDCLATEMPLATEID, "CLATemplate", new CommonPart(), 
                new CLATemplatePart {Record = new CLATemplatePartRecord(), CLA = Strings.CLATEXT, CLATitle = Strings.CLATITLE });
            _mockContent.ExpectGetItem(claTemplate);

            var isProject = ContentFactory.CreateContentItem(Ids.VALIDPROJECTID, "Project", new CommonPart(), 
                new ProjectPart { Record = new ProjectPartRecord(), CLATemplate = claTemplate.As<CLATemplatePart>().Record}, 
            new TitlePart { Record = new TitlePartRecord(), Title = Strings.VALIDPROJECTNAME });
            _mockContent.ExpectGetItem(isProject);

            //var notProject = new ContentItemBuilder(new ContentTypeDefinitionBuilder().Named("SomethingElse").Build()).Weld<CommonPart>().Build();


            //CLATemplate = new CLATemplatePartRecord { CLA = VALIDCLA } }, new TitlePart { Record = new TitlePartRecord(), Title = VALIDPROJECTNAME }
        }



        private void SetupValidUser()
        {


            var user = UserFactory.CreateUser(new UserCreationArgs { Id = Ids.VALIDUSER, Email = Strings.EMAIL, LastName = Strings.LASTNAME, FirstName = Strings.FIRSTNAME, Username = Strings.USERNAME });

            _mockAuthService.Setup(auth => auth.GetAuthenticatedUser()).Returns(user);



            //  var i = _mockServices.WorkContext.CurrentUser;
        }

    }
}
