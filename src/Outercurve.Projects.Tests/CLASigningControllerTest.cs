using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using ExpectedObjects;
using Moq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Events;
using Orchard.Users.Models;
using Outercurve.Projects.Controllers;
using Outercurve.Projects.Helpers;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels.CLASigning;
using Proligence.Orchard.Testing;
using Proligence.Orchard.Testing.Mocks;
using SpecsFor.ShouldExtensions;
using Xunit;

namespace Outercurve.Projects.Tests
{
    public class CLASigningControllerTest {
        private readonly ContentManagerMock _mockContent;
        private readonly OrchardServicesMock _mockServices;
        private readonly Mock<ITransactionManager> _mockTransaction;
        private readonly Mock<ICLASigningService> _mockSigningService;
        private readonly Mock<IExtendedUserPartService> _mockExtService;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Mock<IUserEventHandler> _mockUserEventHandler;
        private readonly Mock<IClock> _mockClock;

        private readonly MockRepository _repo;
        private readonly CLASigningController _controller;

        private const string EMAIL = "EMAIL";
        private const string FIRSTNAME = "FIRSTNAME";
        public const string LASTNAME = "LASTNAME";
        public const int USERID = 999;
        public const string USERNAME = "USERNAME";
        public const int VALIDPROJECTID = 2;
        public const string VALIDPROJECTNAME = "PROJECTNAME";

        public const string VALIDCLA = "CLA";
        public const int VALIDCLAID = 3;





        public const string VALIDNONCE = "VALID NONCE";

        public const string INVALIDNONCE = "INVALID NONCE";

        public const string VALIDCLATITLE = "VALIDCLATitle";

        public const string SIGNERFROMCOMPANY = "SIGNERFROMCOMPANY";

        public const int VALIDCLATEMPLATEID = 66;


        public CLASigningControllerTest() {
            _repo = new MockRepository(MockBehavior.Default);

            _mockContent = new ContentManagerMock();
            _mockServices = new OrchardServicesMock();
            _mockTransaction = _repo.Create<ITransactionManager>();
            _mockSigningService = _repo.Create<ICLASigningService>();
            _mockExtService = _repo.Create<IExtendedUserPartService>();
            _mockAuthService = _repo.Create<IAuthenticationService>();
            _mockUserEventHandler = _repo.Create<IUserEventHandler>();
            _mockClock = _repo.Create<IClock>();

            _controller = new CLASigningController(_mockContent.Object,
                _mockServices, _mockTransaction.Object, _mockSigningService.Object, _mockExtService.Object, _mockAuthService.Object, _mockUserEventHandler.Object, _mockClock.Object);
            //CreateProjects();
        }


        

  

       




       

       

 

    
    }
}
