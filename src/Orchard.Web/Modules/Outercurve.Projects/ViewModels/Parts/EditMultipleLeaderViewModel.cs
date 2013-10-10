using System.Collections.Generic;
using System.Web.Mvc;

namespace Outercurve.Projects.ViewModels.Parts
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