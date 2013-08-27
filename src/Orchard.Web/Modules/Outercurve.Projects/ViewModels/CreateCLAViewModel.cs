using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Outercurve.Projects.ViewModels
{
    public class CreateCLAViewModel
    {

        [Required]
        public string SignerUserName { get; set; }

        [Required]
        public string FoundationUserName { get; set; }

        public string CompanySigner { get; set; }

        public DateTime? SigningDate { get; set; }
    }
}