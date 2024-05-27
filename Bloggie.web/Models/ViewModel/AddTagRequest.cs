using System.ComponentModel.DataAnnotations;

namespace Bloggie.web.Models.ViewModel
{
    public class AddTagRequest
    {
        [Required(ErrorMessage = "Tag Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tag Name must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s-]*$", ErrorMessage = "Tag Name must contain only letters, spaces, and hyphens")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Display Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Display Name must be between 3 and 50 characters")]
        public string DisplayName { get; set; }
        public string ImageUrl { get; set; }
    }
}
