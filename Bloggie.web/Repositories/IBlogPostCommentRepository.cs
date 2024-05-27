using Bloggie.web.Models.Domain;

namespace Bloggie.web.Repositories
{
    public interface IBlogPostCommentRepository
    {
        Task<BlogPostComment > AddAsync (BlogPostComment comment);
        Task<IEnumerable<BlogPostComment>> GetCommentByBlogIdAsync(Guid blogPostId);
        Task<int> GetCommentCount(Guid blogPostId);
    }
}
