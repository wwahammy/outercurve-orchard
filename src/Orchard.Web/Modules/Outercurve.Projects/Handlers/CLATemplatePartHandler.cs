using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.Handlers
{
    public class CLATemplatePartHandler : ContentHandler
    {
        public CLATemplatePartHandler(IRepository<CLATemplatePartRecord> repo) {
            Filters.Add(StorageFilter.For(repo));
        }
    }
}