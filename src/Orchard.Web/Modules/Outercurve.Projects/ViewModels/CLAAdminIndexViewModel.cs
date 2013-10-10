using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Outercurve.Projects.ViewModels
{
    public class CLAAdminIndexViewModel
    {
        public IList<CLAAdminIndexEntry> CLAs { get; set; }
        public dynamic Pager { get; set; }
        public CLAIndexOptions Options {get; set; }
    }

    public class CLAAdminIndexEntry
    {
        public ContentItem CLAItem { get; set; }
        public string ProjectTitle { get; set; }
        public string FoundationSignerName { get; set; }
        public string CLASignerName { get; set; }
        public string Employer { get; set; }
        public string SignedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CLAIndexOptions {
        public CLAOrder Order { get; set; }
    }

    public enum CLAOrder {
       Created,
       SignedByUser,
       Name
    }
}