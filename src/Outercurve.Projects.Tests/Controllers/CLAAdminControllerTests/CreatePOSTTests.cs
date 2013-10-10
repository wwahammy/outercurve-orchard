using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Mvc;
using Moq;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Notify;
using Proligence.Orchard.Testing;
using StructureMap.Query;
using Xunit;

namespace Outercurve.Projects.Tests.Controllers.CLAAdminControllerTests
{
    public class CreatePOSTTests : CLAAdminControllerFixture {

        private ContentItem newCLA;

        public CreatePOSTTests() {
         
            orchardServicesMock.AuthorizerMock.Allow(StandardPermissions.SiteOwner);
            
           
        
        }

        [Fact]
        public void CreatePOST_ValidModelState()
        {
            CreateCLA();

            orchardServicesMock.ContentManagerMock.ExpectNewItem("CLA", newCLA);
            orchardServicesMock.ContentManagerMock.ExpectCreateItem(newCLA);


            

            orchardServicesMock.NotifierMock.ExpectInformation(It.IsAny<string>());

            var result = Run<RedirectToRouteResult>(c => c.CreatePOST());

            orchardServicesMock.ContentManagerMock.Verify(c => c.New("CLA"), Times.Once);
            orchardServicesMock.ContentManagerMock.Verify(c => c.Create(newCLA), Times.Once);

            orchardServicesMock.ContentManagerMock.Verify(c => c.Publish(newCLA), Times.Once);
            orchardServicesMock.NotifierMock.Verify(i => i.Add(NotifyType.Information, It.IsAny<LocalizedString>()));

            Assert.Contains("Index", result.RouteValues.Values);

        }

        public void CreatePOST_InvalidModelState() {
            CreateCLA();
        }

        public void CreateCLA()
        {
            newCLA = ContentFactory.CreateContentItem(0, "CLA");

        }
    }
}
