using System;
using System.Collections.Generic;

namespace Bloggie.web.Models.Domain
{
    public class BlogPost
    {
        public Guid Id { get; set; }
        public string Heading { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string UrlHandle { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime? ScheduledPublishDate { get; set; }  // Nullable DateTime
        public string Author { get; set; }
        public bool Visible { get; set; }

        // SEO-related properties
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }

        // Navigation properties
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<BlogPostLike> Likes { get; set; } = new List<BlogPostLike>();
        public ICollection<BlogPostComment> Comments { get; set; } = new List<BlogPostComment>();
        public ICollection<PostEditLog> EditLogs { get; set; } = new List<PostEditLog>();
    }
}
