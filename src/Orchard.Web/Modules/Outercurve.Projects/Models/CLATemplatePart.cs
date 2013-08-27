using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace Outercurve.Projects.Models
{
    public class CLATemplatePart : ContentPart<CLATemplatePartRecord> {

        public string CLATitle { get { return Record.CLATitle; } set { Record.CLATitle = value; } }

        public string CLA { get { return Record.CLA; } set { Record.CLA = value; } }
    }

    public class CLATemplatePartRecord : ContentPartRecord {
        public virtual string CLATitle { get; set; }
        [StringLengthMax]
        public virtual string CLA { get; set; }
        
    }
}