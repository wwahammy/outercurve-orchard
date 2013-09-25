using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outercurve.Projects.ViewModels.Parts
{
    public class EditCLATextViewModel
    {
        public string SelectedTemplate { get; set; }

        public TemplateDetailViewModel TemplateInfo { get; set; }

        public bool TemplateDoesntExist { get; set; }

    }

    public class TemplateDetailViewModel
    {




        public string CurrentHtmlForTemplate { get; set; }
        /// <summary>
        /// Of the format {TemplateId_TemplateVersion, "*Nice Template Name*, v *TemplateVersion*"}
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> TemplateNameVersionsAndIds { get; set; }
    }
}