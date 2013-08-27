using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Outercurve.Projects.ViewModels
{
    public class ProjectsAdminIndexViewModel
    {
        public IList<ProjectAdminIndexEntry> Projects { get; set; }
        public dynamic Pager { get; set; }
    }

    public class ProjectAdminIndexEntry {
        public ContentItem ProjectItem { get; set; }
        public bool IsChecked { get; set; }
    }
}