using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Moq;
using Orchard.Localization;
using Orchard.Security.Permissions;
using Orchard.UI.Navigation;
using Xunit;

namespace Outercurve.Projects.Tests.Controllers.CLAAdminControllerTests
{
    public class UnauthorizedTests :CLAAdminControllerFixture
    {
        public UnauthorizedTests() {
            orchardServicesMock.AuthorizerMock.Setup(a => a.Authorize(It.IsAny<Permission>(), It.IsAny<LocalizedString>())).Returns(false);
        }

        [Fact]
        public void Unauthorized_Test()
        {
            


            //index
            Assert.IsType<HttpUnauthorizedResult>(controller.Index(null));
            Assert.IsType<HttpUnauthorizedResult>(controller.Index(It.IsAny<PagerParameters>()));

            //Create

            Assert.IsType<HttpUnauthorizedResult>(controller.Create());
            Assert.IsType<HttpUnauthorizedResult>(controller.CreatePOST());

            //Edit
            Assert.IsType<HttpUnauthorizedResult>(controller.Edit(0));
            Assert.IsType<HttpUnauthorizedResult>(controller.Edit(It.IsAny<int>()));
            Assert.IsType<HttpUnauthorizedResult>(controller.EditPOST(0));
            Assert.IsType<HttpUnauthorizedResult>(controller.EditPOST(It.IsAny<int>()));

            //get id and version
            Assert.IsType<HttpUnauthorizedResult>(controller.GetIdAndVersion(null));
            Assert.IsType<HttpUnauthorizedResult>(controller.GetIdAndVersion(It.IsAny<string>()));

            Assert.IsType<HttpUnauthorizedResult>(controller.GetExcelOfCLAs());

            Assert.IsType<HttpUnauthorizedResult>(controller.Delete(0));
            Assert.IsType<HttpUnauthorizedResult>(controller.Delete(It.IsAny<int>()));


        }
    }
}
