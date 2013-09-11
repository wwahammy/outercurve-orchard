using System;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Orchard.Data;
using Orchard.Security;
using Orchard.Services;
using Orchard.Users.Events;
using Outercurve.Projects.Controllers;
using Outercurve.Projects.Services;
using Proligence.Orchard.Testing.Mocks;
using Xunit;

namespace Outercurve.Projects.Tests.CLASigningControllerTests
{
    public abstract class CLASigningControllerFixture
    {
        protected readonly ContentManagerMock _mockContent;
        protected readonly OrchardServicesMock _mockServices;
        protected readonly Mock<ITransactionManager> _mockTransaction;
        protected readonly Mock<ICLASigningService> _mockSigningService;
        protected readonly Mock<IExtendedUserPartService> _mockExtService;
        protected readonly Mock<IAuthenticationService> _mockAuthService;
        protected readonly Mock<IUserEventHandler> _mockUserEventHandler;
        protected readonly Mock<IClock> _mockClock;
        protected readonly MockRepository _repo;

        protected readonly CLASigningController _controller;

       
   

        protected CLASigningControllerFixture() {
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
            
            SetupBasicControllerStuff();
        }


        
        protected TResult Run<TResult>(Func<CLASigningController, ActionResult> action ) where TResult : ActionResult {
            ActionResult actionResult = null;
            TResult result;
            Assert.DoesNotThrow(() => actionResult = action(_controller));
            Assert.NotNull(result = actionResult as TResult);
            return result;
            
        }


        protected  void BadProjectNotify()
        {
            _mockServices.NotifierMock.ExpectInformation(CLASigningController.BADPROJECTIDMESSAGE);
        }

        protected void VerifyBadProjectRedirect(RedirectToRouteResult getRoute)
        {
            Assert.Equal("Index", getRoute.RouteValues["action"]);

            _mockServices.NotifierMock.Verify();
        }

        public void SetupBasicControllerStuff()
        {
            _controller.ControllerContext = new ControllerContext();
            _controller.Url = new UrlHelper(new RequestContext(), new RouteCollection());
        }
    }
}
