using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Outercurve.Projects.ViewModels
{
    public class QuickUserViewModel
    {
       
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string ReturnUrl { get; set; }

    }
}