using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Outercurve.Projects.ViewModels
{
    public class ViewCLAViewModel
    {
        public ContentItem Project { get; set; }
        public string FoundationSigner { get; set; }
        public string CLASigner { get; set; }
        public string ValidDate { get; set; }
        public bool IsValid { get; set; }
    }
}