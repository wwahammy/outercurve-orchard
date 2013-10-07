using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Outercurve.Projects.Models {
    public class MoreSiteSettingsPart : ContentPart<MoreSiteSettingsPartRecord>, IMoreSiteSettings {
        public string SiteOwner {
            get { return Record.SiteOwner; }
            set { Record.SiteOwner = value; }
        }
    }

    public interface IMoreSiteSettings : IContent {
        
        string SiteOwner { get; set; }
    }

    public class MoreSiteSettingsPartRecord : ContentPartVersionRecord {
        public virtual string SiteOwner { get; set; }
    }

}