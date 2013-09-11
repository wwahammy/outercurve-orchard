using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Orchard.Data;
using Orchard.Settings;
using Outercurve.Projects.Controllers;
using Outercurve.Projects.Services;
using Proligence.Orchard.Testing.Mocks;

namespace Outercurve.Projects.Tests.Controllers
{
    public abstract class BaseControllerFixture<T> where T : BaseController {
        protected OrchardServicesMock orchardServicesMock;
        protected Mock<IExtendedUserPartService> extUserServiceMock;
        protected Mock<IGalleryService> galleryMock;
        protected Mock<ITransactionManager> transactionMock;
        protected ShapeFactoryMock shapeMock;
        protected Mock<ISiteService> siteMock;
        protected T controller;

        protected BaseControllerFixture() {
            extUserServiceMock = new Mock<IExtendedUserPartService>();
            galleryMock = new Mock<IGalleryService>();
            transactionMock = new Mock<ITransactionManager>();
            shapeMock = new ShapeFactoryMock();
            siteMock = new Mock<ISiteService>();
            orchardServicesMock = new OrchardServicesMock();
        }

        protected TResult Run<TResult>(Func<T, ActionResult> action ) where TResult : ActionResult {
            ActionResult actionResult = null;
            TResult result;
            Assert.DoesNotThrow(() => actionResult = action(controller));
            Assert.NotNull(result = actionResult as TResult);
            return result;
            
        }

    }
}
