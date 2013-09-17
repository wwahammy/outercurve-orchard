using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Users.ViewModels;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.ViewModels
{
    [Bind(Exclude = "UsernamesToNiceNames")]
    public class EditMultipleLeaderViewModel
    {
        public EditMultipleLeaderViewModel() {
            SelectedUsernames = new List<string>();
        }

        [MinLength(1, ErrorMessage="You need at least one leader")]
        public IList<string> SelectedUsernames { get; set; }

        public IEnumerable<KeyValuePair<string,string>> UsernamesToNiceNames { get; set; }
    }

   
  
}