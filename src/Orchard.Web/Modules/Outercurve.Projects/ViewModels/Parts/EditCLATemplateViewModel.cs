using System.ComponentModel.DataAnnotations;

namespace Outercurve.Projects.ViewModels.Parts
{
    public class EditCLATemplateViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string CLA { get; set; }
    }
}