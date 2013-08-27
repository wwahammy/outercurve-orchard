using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Users.ViewModels;
using Outercurve.Projects.Models;

namespace Outercurve.Projects.ViewModels
{
    public class EditMultipleLeaderViewModel
    {
        public EditMultipleLeaderViewModel() {
            UsernamesForLeadersSelected = new List<MultipleLeaderRowViewModel>();
        }

        [MinLength(1, ErrorMessage="You need at least one leader")]
        public IList<MultipleLeaderRowViewModel> UsernamesForLeadersSelected { get; set; }
    }

    public class MultipleLeaderRowItem {
        public string Title { get; set; }
        public string Id { get; set; }
    }
  
}