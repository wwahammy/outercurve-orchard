using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Outercurve.Projects.Services;

namespace Outercurve.Projects.ViewModels
{
    public class MultipleLeaderRowViewModel
    {
        public MultipleLeaderRowViewModel () {
            SelectedUserName = "";
        }
        public string SelectedUserName { get; set; }
       
    }
}