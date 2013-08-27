using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Outercurve.Projects.ViewModels.CLASigning
{
    [Bind(Include = "CompanyContact,CompanyContactEmail,CLAId")]
    public class SignCompanyViewModel
    {
        public string CLA { get; set; }

        public int CLAId { get; set; }
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }
        

        public string FirstName { get; set; }
        public string LastName { get; set; }
        

        public string Email { get; set; }


        public string Employer { get; set; }


        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }

        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }


        [Required]
        public string CompanyContact { get; set; }
        [Required]
        public string CompanyContactEmail { get; set; }

    }
}