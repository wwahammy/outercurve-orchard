using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outercurve.Projects.ViewModels.CLASigning
{
    public class IndexViewModel
    {
        public IList<ProjectNameAndId> Projects { get; set; }
    }

    public class ProjectNameAndId {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}