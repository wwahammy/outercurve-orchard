using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Outercurve.Projects.ViewModels.CLASigning
{
    [Bind(Exclude = "ProjectName")]
    public class ResendLinkViewModel
    {
        [Required]
        public int CLAId { get; set; }

        public string ProjectName { get; set; }

        [Required]
        public string CompanyContact { get; set; }

        [Required]
        public string CompanyContactEmail { get; set; }
    }
}