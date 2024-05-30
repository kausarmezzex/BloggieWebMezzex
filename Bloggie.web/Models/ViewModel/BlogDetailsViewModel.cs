using Bloggie.web.Models.Domain;

namespace Bloggie.web.Models.ViewModel
{
    public class BlogDetailsViewModel
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

        public ICollection<Tag> Tags { get; set; }
        public int TotalLikes { get; set; }
        public bool Liked { get; set; }
        // SEO-related properties
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string CommentDescription { get; set; }
        public IEnumerable<BlogComment> Comments { get; set; }  

        public int TotalComments { get; set; }
        public string TagName { get; set; }
        // Use RelatedPostViewModel for related posts and famous posts
        public List<RelatedPostViewModel> RelatedPosts { get; set; }
        public List<RelatedPostViewModel> FamousPost { get; set; }

    }
}
