using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.ViewModels.Parts
{
    public class EditMoreSiteSettingsViewModel
    {
        [Required]
        public  MoreSiteSettingsPart Part { get; set; }
    }
}