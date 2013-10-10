using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.Handlers
{
    public class CLAPartHandler : ContentHandler
    {
        private readonly IContentManager _contentManager;

        public CLAPartHandler(IRepository<CLAPartRecord> repository, IContentManager contentManager) {
            _contentManager = contentManager;
            Filters.Add(StorageFilter.For(repository));
           

        }
    }
}