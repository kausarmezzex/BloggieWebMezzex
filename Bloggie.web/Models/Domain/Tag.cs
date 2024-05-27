

namespace Bloggie.web.Models.Domain
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ImageUrl { get; set; }  // Add this line to include ImageUrl
        public ICollection<BlogPost> BlogPosts { get; set; }
    }
}
