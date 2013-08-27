using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outercurve.Projects.ViewModels.CLASigning
{
    public class EmailToCompanySignerViewModel
    {
        public string ProjectName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanySigner { get; set; }
        public string Link { get; set; }
    }
}