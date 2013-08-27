using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.Handlers
{
    public class ExtendedUserPartHandler : ContentHandler
    {
        public ExtendedUserPartHandler(IRepository<ExtendedUserPartRecord> repo) {
            Filters.Add(StorageFilter.For(repo));
          //  Filters.Add(new ActivatingFilter<ExtendedUserPart>("User"));
           

        }
    }
}