using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Core.Common.Utilities;

namespace Outercurve.Projects.Models
{
    public class CLATextPart : ContentPart<CLATextPartRecord>
    {
        private readonly LazyField<IContent> _templateId = new LazyField<IContent>();

        public LazyField<IContent> CLATemplateField { get { return _templateId; } }

        public IContent CLATemplate
        {
            get { return CLATemplateField.Value; }
            set { CLATemplateField.Value = value; }
        }

    }

    public class CLATextPartRecord : ContentPartVersionRecord {
        public virtual int TemplateId { get; set; }

        public virtual int TemplateVersion { get; set; }
    }
}