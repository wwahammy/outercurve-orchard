using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.ViewModels.Parts
{
    public class EditProjectViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int SelectedTemplate_Id { get; set; }

        public IList<CLATemplatePart> AllTemplates { get; set; }

        
    }
}