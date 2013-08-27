using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Security;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.ViewModels
{
    public class EditExtendedUserViewModel
    {
        public IUser User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }

        public bool HideLegend { get; set; }
        
    }
}