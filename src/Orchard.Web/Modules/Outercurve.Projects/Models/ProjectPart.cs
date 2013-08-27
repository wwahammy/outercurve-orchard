using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Outercurve.Projects.Models
{
    public class ProjectPart : ContentPart<ProjectPartRecord>
    {
        public virtual CLATemplatePartRecord CLATemplate {
            get { return Record.CLATemplate; }
            set { Record.CLATemplate = value; }
        }

    }

    public class ProjectPartRecord : ContentPartRecord {
        public virtual CLATemplatePartRecord CLATemplate { get; set; }

    }
}