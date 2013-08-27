using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Outercurve.Projects.ViewModels.CLASigning
{
    public class ChooseViewModel
    {
        [Required]
        public int ProjectId { get; set; }
    }
}