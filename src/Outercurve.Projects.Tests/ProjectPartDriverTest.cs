using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Moq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Outercurve.Projects.Drivers;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;
using Proligence.Orchard.Testing.Mocks;
using Xunit;

namespace Outercurve.Projects.Tests
{
    public class ProjectPartDriverTest {

        private readonly IContentPartDriver _projectPartDriver;
        private readonly Mock<IContentManager> _contentManagerMock;
        private readonly Mock<IProjectService> _projectServiceMock;
        private readonly Mock<ITransactionManager> _transactionMock;

        public ProjectPartDriverTest() {

            _contentManagerMock = new Mock<IContentManager>();
            _projectServiceMock = new Mock<IProjectService>();

            _transactionMock = new Mock<ITransactionManager>();

            _projectPartDriver = new ProjectPartDriver(_contentManagerMock.Object, _projectServiceMock.Object, _transactionMock.Object);
        }

        [Fact]
        public void ImportSetsAll_Test() {

            const int TEMPLATE_ID = 999;

            var doc = XElement.Parse(String.Format(@"
                <data>
                <ProjectPart
                    CLATemplateId=""{0}""/>
                </data>
                ", TEMPLATE_ID));

            var localContentManagerMock = new ContentManagerMock();

            var templatePart = new CLATemplatePart();

            Helpers.PreparePart<CLATemplatePart, CLATemplatePartRecord>(templatePart, "CLATemplate", TEMPLATE_ID);
            localContentManagerMock.ExpectGetItem(TEMPLATE_ID, templatePart.ContentItem);

            var part = new ProjectPart();
            Helpers.PreparePart<ProjectPart, ProjectPartRecord>(part, "Project");
            var context = new ImportContentContext(part.ContentItem, doc, new ImportContentSession(localContentManagerMock.Object));
            _projectPartDriver.Importing(context);

            Assert.Equal(1, part.CLATemplate.Id);
        }
    }
}
