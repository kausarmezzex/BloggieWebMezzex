using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.web.Models.ViewModel
{
    public class EditBlogPostRequest
    {

        public Guid Id { get; set; }

        public string Heading { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string UrlHandle { get; set; }
        public DateTime PublishDate { get; set; }
        public string Author { get; set; }
        public bool Visible { get; set; }
        public DateTime? ScheduledPublishDate { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }

        // Display Tags Here 
        public IEnumerable<SelectListItem> Tags { get; set; }

        // Collect Tags 
        public string[] SelectedTags { get; set; } = Array.Empty<string>();

    }
}
