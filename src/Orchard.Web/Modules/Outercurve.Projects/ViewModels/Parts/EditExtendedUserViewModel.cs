using Orchard.Security;

namespace Outercurve.Projects.ViewModels.Parts
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