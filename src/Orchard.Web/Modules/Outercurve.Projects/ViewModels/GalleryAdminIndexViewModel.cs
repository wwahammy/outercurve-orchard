using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Outercurve.Projects.ViewModels
{
    public class GalleryAdminIndexViewModel
    {
        public IList<GalleryAdminIndexEntry> Galleries { get; set; }
        public dynamic Pager { get; set; }
    }

    public class GalleryAdminIndexEntry {
        public ContentItem GalleryItem { get; set; }
        public bool IsChecked { get; set; }
    }
}