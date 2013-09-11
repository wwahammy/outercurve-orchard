using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Orchard.ContentManagement;
using Proligence.Orchard.Testing.Mocks;


namespace Outercurve.Projects.Tests
{
    public static class ContentManagerMockExtensions
    {
        public static void ExpectGetItem(this ContentManagerMock contentManager, ContentItem contentItem) {
            contentManager.Setup(x => x.Get(contentItem.Id)).Returns(contentItem);
        }

        public static void ExpectGetItem(this ContentManagerMock contentManager, ContentItem contentItem, int version)
        {
            contentManager.Setup(x => x.Get(contentItem.Id, It.Is<VersionOptions>(v => v.VersionNumber == version))).Returns(contentItem);
        }
    }
}
