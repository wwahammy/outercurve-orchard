using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.Users.Models;
using Outercurve.Projects.Models;
using Outercurve.Projects.Services;

namespace Outercurve.Projects.ViewModels
{
    [Bind(Exclude = "Template")]
    public class EditCLAViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string CLASignerUsername { get; set; }
        [Required]
        public string CLASignerFirstName { get; set; }
        [Required]
        public string CLASignerLastName { get; set; }
        [Required]
        public string CLASignerEmail { get; set; }

        public bool IsSignedByUser { get; set; }
        public string SigningDate { get; set; }


        public bool HasFoundationSigner { get; set; }
        public string FoundationSignerUsername { get; set; }
        public string FoundationSigningDate { get; set; }


        public bool NeedCompanySignature { get; set; }

        public bool HasCompanySigner { get; set; }
        public string CompanySigner { get; set; }
        public string CompanySignerEmail { get; set; }
        public string CompanySigningDate { get; set; }

        public string Employer { get; set; }

        public string LocationOfCLA { get; set; }
        public string Comments { get; set; }
     
        public bool IsCommitter { get; set; }

        public string Address1 { get; set; }
        public  string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public  string ZipCode { get; set; }
        public string Country { get; set; }

        
        

        public string SelectedTemplate { get; set; }

        public TemplateDetailViewModel Template { get; set; }
        //public IContent CLATemplate { get; set; }

        public bool StaffOverride { get; set; }

       
    }

   public class TemplateDetailViewModel {

   

       
       public string CurrentHtmlForTemplate { get; set; }
       /// <summary>
       /// Of the format {TemplateId_TemplateVersion, "*Nice Template Name*, v *TemplateVersion*"}
       /// </summary>
       public IEnumerable<KeyValuePair<string, string>> TemplateNameVersionsAndIds { get; set; }
   }

    
}