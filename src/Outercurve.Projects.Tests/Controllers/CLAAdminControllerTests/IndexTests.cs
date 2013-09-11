using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Mvc;
using Moq;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Notify;
using Proligence.Orchard.Testing;
using Xunit;

namespace Outercurve.Projects.Tests.Controllers.CLAAdminControllerTests
{
    public class IndexTests : CLAAdminControllerFixture {
       
        public IndexTests() {
            orchardServicesMock.AuthorizerMock.Allow(StandardPermissions.SiteOwner);
            
           
        }


        


        

       



        
    }
}
