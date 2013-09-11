using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Moq;
using Orchard;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Security;
using Orchard.Settings;
using Outercurve.Projects.Controllers;
using Outercurve.Projects.Services;
using Outercurve.Projects.ViewModels.Index;
using Proligence.Orchard.Testing;
using Xunit;

namespace Outercurve.Projects.Tests.Controllers
{
    public class IndexControllerTests : ControllerTestFixture {
        public Mock<IAuthenticationService> _authService;
        private Mock<IExtendedUserPartService> _extUserMock;
        private IndexController _controller;


        public IndexControllerTests() {
              _authService = new Mock<IAuthenticationService>();
              _extUserMock = new Mock<IExtendedUserPartService>();
            _controller = new IndexController(_extUserMock.Object, It.IsAny<IGalleryService>(), It.IsAny<ITransactionManager>(), It.IsAny<IShapeFactory>(), It.IsAny<IOrchardServices>(), It.IsAny<ISiteService>(), _authService.Object);

        }

       


        [Fact]
        public void NotSignedIn_Test() {
            _authService.Setup(a => a.GetAuthenticatedUser()).Returns((IUser)null);

            var result = Run<ViewResult>(c => c.Index());
            Assert.Equal("Index", result.ViewName);
            var model = result.Model as IndexViewModel;
            Assert.False(model.IsCommitter);
            Assert.False(model.IsContributor);
            Assert.False(model.IsProjectLeader);


        }


        protected TResult Run<TResult>(Func<IndexController, ActionResult> action) where TResult : ActionResult
        {
            ActionResult actionResult = null;
            TResult result;
            Assert.DoesNotThrow(() => actionResult = action(_controller));
            Assert.NotNull(result = actionResult as TResult);
            return result;

        }
    }
}
