using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using Moq;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Security.Permissions;
using Orchard.UI.Navigation;
using Outercurve.Projects.Controllers;
using Outercurve.Projects.Services;
using Proligence.Orchard.Testing.Mocks;
using Xunit;

namespace Outercurve.Projects.Tests.Controllers.CLAAdminControllerTests
{
    public abstract class CLAAdminControllerFixture : BaseControllerFixture<CLAAdminController>{

        

        protected Mock<IProjectService> projectMock;
        protected Mock<ICLAToOfficeService> officeMock;
        protected Mock<ICLATemplateService> templateMock;
        

        protected CLAAdminControllerFixture() {
            projectMock = new Mock<IProjectService>();
            officeMock = new Mock<ICLAToOfficeService>();
            templateMock = new Mock<ICLATemplateService>();

            controller = new CLAAdminController(orchardServicesMock, extUserServiceMock.Object, galleryMock.Object, transactionMock.Object, shapeMock, siteMock.Object, projectMock.Object, officeMock.Object, templateMock.Object);

        }

        
    }
}
