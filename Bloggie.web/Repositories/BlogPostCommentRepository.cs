using Bloggie.web.Data;
using Bloggie.web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.web.Repositories
{
    public class BlogPostCommentRepository : IBlogPostCommentRepository
    {
        private readonly BloggieDBContext bloggieDBContext;

        public BlogPostCommentRepository(BloggieDBContext bloggieDBContext)
        {
            this.bloggieDBContext = bloggieDBContext;
        }
        public async Task<BlogPostComment> AddAsync(BlogPostComment comment)
        {
            await bloggieDBContext.BlogComments.AddAsync(comment);
            await bloggieDBContext.SaveChangesAsync();
            return comment;
        }

        public async Task<IEnumerable<BlogPostComment>> GetCommentByBlogIdAsync(Guid blogPostId)
        {
          return  await bloggieDBContext.BlogComments.Where(x=> x.BlogPostId == blogPostId).ToListAsync();
        }

        public async Task<int> GetCommentCount(Guid blogPostId)
        {
            return await bloggieDBContext.BlogComments
                                         .Where(comment => comment.BlogPostId == blogPostId)
                                         .CountAsync();
        }

    }
}
