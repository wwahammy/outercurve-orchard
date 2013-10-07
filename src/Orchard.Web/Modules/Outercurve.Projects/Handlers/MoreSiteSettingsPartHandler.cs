using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.Handlers
{
    public class MoreSiteSettingsPartHandler : ContentHandler
    {
        public MoreSiteSettingsPartHandler(IRepository<MoreSiteSettingsPartRecord> repository) {
            Filters.Add(new ActivatingFilter<MoreSiteSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}