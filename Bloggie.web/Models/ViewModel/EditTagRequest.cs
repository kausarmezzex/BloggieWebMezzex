using System.ComponentModel.DataAnnotations;

namespace Bloggie.web.Models.ViewModel
{
    public class EditTagRequest
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Tag Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tag Name must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Tag Name must contain only letters and spaces")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Display Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Display Name must be between 3 and 100 characters")]
        public string DisplayName { get; set; }
        public string ImageUrl { get; set; }
    }
}
