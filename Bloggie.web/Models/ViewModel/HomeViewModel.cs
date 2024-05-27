using Bloggie.web.Models.Domain;

namespace Bloggie.web.Models.ViewModel
{
    public class HomeViewModel
    {
        public IEnumerable<BlogPost> BlogPosts { get; set; }

        public IEnumerable<Tag> Tags { get; set; }
    }
}
