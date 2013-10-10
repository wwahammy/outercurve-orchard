using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Security;
using Orchard.Services;
using Outercurve.Projects.Drivers;
using Outercurve.Projects.Services;
using Proligence.Orchard.Testing;
using Proligence.Orchard.Testing.Mocks;

namespace Outercurve.Projects.Tests.CLAAdminControllerTests
{
    public abstract class ClaPartDriverTestFixture : ContentDriverTestFixture {
        protected Mock<ICLAPartService> _mockClaPartService;
        protected ContentManagerMock _mockContentManager;
        
        protected CLAPartDriver claDriver;
        protected Mock<IUTCifierService> _mockUtcifierService;
        protected Mock<IMembershipService> _mockMembership;
        protected Mock<ITransactionManager> _mockTransaction;
        protected Mock<IClock> _mockClock;
        protected Mock<ICLATemplateService> _mockClaTemplate;

        protected ClaPartDriverTestFixture()  {

            _mockClaPartService = new Mock<ICLAPartService>();
            _mockContentManager = new ContentManagerMock();

            _mockUtcifierService = new Mock<IUTCifierService>();
            _mockMembership = new Mock<IMembershipService>();
            _mockTransaction = new Mock<ITransactionManager>();
            _mockClock = new Mock<IClock>();
            _mockClaTemplate = new Mock<ICLATemplateService>();
            claDriver = new CLAPartDriver(_mockClaPartService.Object, _mockContentManager.Object, _mockUtcifierService.Object, _mockMembership.Object, _mockTransaction.Object, _mockClock.Object, _mockClaTemplate.Object);
    

        }

    }
}
