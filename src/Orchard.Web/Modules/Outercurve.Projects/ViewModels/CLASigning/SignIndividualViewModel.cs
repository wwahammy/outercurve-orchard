using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Outercurve.Projects.ViewModels.CLASigning
{
    [Bind(Exclude = "CLA")]
    public class SignIndividualViewModel {
        [Required]
        public int ProjectId { get; set; }

        public bool NeedCompanySignature { get; set; }

        public string ProjectName { get; set; }
        
        public string CLA { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string Employer { get; set; }

        public string CompanyContact { get; set; }
        public string CompanyContactEmail { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string Country { get; set; }
    }
}