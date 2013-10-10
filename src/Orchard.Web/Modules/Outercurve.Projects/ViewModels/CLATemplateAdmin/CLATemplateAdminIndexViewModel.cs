using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Outercurve.Projects.ViewModels.CLATemplateAdmin
{
    public class CLATemplateAdminIndexViewModel
    {
        public IList<CLATemplateAdminIndexEntry> Templates { get; set; }
        public dynamic Pager { get; set; }
    }

    public class CLATemplateAdminIndexEntry {
        public ContentItem Item { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
    }
}