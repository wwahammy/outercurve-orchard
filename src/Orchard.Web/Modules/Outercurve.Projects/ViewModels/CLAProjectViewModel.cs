using System;
using System.Collections.Generic;

namespace Outercurve.Projects.ViewModels {
    public class CLAProjectViewModel {
        public string Name { get; set; }
        public IList<CLAItemViewModel> Items { get; set; }
        //public dynamic List { get; set; }
        //public dynamic Pager { get; set; }
    }

    public class CLAItemViewModel {
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public bool IsValid { get; set; }

        public string ValidFrom { get; set; }
    }
}