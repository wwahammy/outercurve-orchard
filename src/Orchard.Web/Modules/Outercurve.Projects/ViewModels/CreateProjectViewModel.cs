using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Outercurve.Projects.ViewModels
{
    public class CreateProjectViewModel
    {

        public dynamic Shape { get; set; }

        [Required]
        public string OwnerUserName { get; set; }

        [Required]
        public int GalleryId { get; set; }
    }
}