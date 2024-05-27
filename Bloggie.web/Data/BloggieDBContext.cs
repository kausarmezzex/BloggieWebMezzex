using Bloggie.web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.web.Data
{
    public class BloggieDBContext : DbContext
    {
        public BloggieDBContext()
        {
        }

        public BloggieDBContext(DbContextOptions<BloggieDBContext> options) : base(options)
        {
        }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BlogPostLike> Likes { get; set; }
        public DbSet<BlogPostComment> BlogComments { get; set; }
        public DbSet<PostEditLog> PostEditLogs { get; set; }
    }
}
