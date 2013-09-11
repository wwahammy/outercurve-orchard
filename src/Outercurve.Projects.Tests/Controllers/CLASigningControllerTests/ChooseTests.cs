using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Outercurve.Projects.Controllers;
using Outercurve.Projects.Models;
using Outercurve.Projects.ViewModels.CLASigning;
using Proligence.Orchard.Testing;
using Xunit;

namespace Outercurve.Projects.Tests.CLASigningControllerTests
{
    public class ChooseTests : CLASigningControllerFixture
    {
        public ChooseTests() {
            CreateProjects();
        }

        public static class Ids {
            public const int VALIDPROJECTID = 1;
            public const int INVALIDPROJECTID = 2;
        }

        [Fact]
        public void ChooseWithGoodProjectIdTest()
        {
            var output = Run<ViewResult>(c => c.Choose(Ids.VALIDPROJECTID));

            //Assert.Equal( "Choose", );
            Assert.Equal(Ids.VALIDPROJECTID, (output.Model as ChooseViewModel).ProjectId);

        }

        [Fact]
        public void ChooseWithBadProjectIdTest()
        {
            BadProjectNotify();

            var output = Run<RedirectToRouteResult>(c => c.Choose(Ids.INVALIDPROJECTID));



            Assert.Equal("Index", output.RouteValues["action"]);

            _mockServices.NotifierMock.Verify();
        }

        public void CreateProjects()
        {
            var notProject = ContentFactory.CreateContentItem(Ids.INVALIDPROJECTID, "SomethingElse", new CommonPart());

            _mockContent.ExpectGetItem(notProject);

          //  var claTemplate = ContentFactory.CreateContentItem(VALIDCLATEMPLATEID, "CLATemplate", new CommonPart(), new CLATemplatePart { CLA = VALIDCLA, CLATitle = VALIDCLATITLE });
           // _mockContent.ExpectGetItem(claTemplate);

            var isProject = ContentFactory.CreateContentItem(Ids.VALIDPROJECTID, "Project", new CommonPart(), new ProjectPart { Record = new ProjectPartRecord() });
            _mockContent.ExpectGetItem(isProject);

            //var notProject = new ContentItemBuilder(new ContentTypeDefinitionBuilder().Named("SomethingElse").Build()).Weld<CommonPart>().Build();


            //CLATemplate = new CLATemplatePartRecord { CLA = VALIDCLA } }, new TitlePart { Record = new TitlePartRecord(), Title = VALIDPROJECTNAME }
        }


       
    }
}
