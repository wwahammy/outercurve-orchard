using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Outercurve.Projects.Services;

namespace Outercurve.Projects.ViewModels
{
    public class CreateGalleryViewModel
    {
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string OwnerUserName { get; set; }

       
        
    }
}