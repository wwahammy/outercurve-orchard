using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.Services
{
    public interface ICLATextPartService : IDependency {
        void UpdatePart(ContentItem item, CLATextPartModel model);
        void UpdatePart(ContentItem item, ContentItem templateItem);
    }

    public class CLATextPartService : ICLATextPartService
    {
        private readonly IContentManager _contentManager;

        public CLATextPartService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public void UpdatePart(ContentItem item, CLATextPartModel model) {
            
            UpdatePart(item, _contentManager.Get(model.ContentId, VersionOptions.Number(model.ContentVersion)));

            
        }

        public void UpdatePart(ContentItem item, ContentItem templateItem) {
            var part = item.As<CLATextPart>();

            part.CLATemplate = templateItem;

        }
    }

    public class CLATextPartModel {
        public int ContentId { get; set; }
        public int ContentVersion { get; set; }

    }
}