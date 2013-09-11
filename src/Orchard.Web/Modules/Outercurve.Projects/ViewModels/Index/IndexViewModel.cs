using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outercurve.Projects.ViewModels.Index
{
    public class IndexViewModel
    {
        public bool IsCommitter { get; set; }
        public bool IsContributor { get; set; }
        public bool IsProjectLeader { get; set; }
    }
}