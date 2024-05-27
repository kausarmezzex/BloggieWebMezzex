using Bloggie.web.Models.Domain;

namespace Bloggie.web.Repositories
{
    public interface IBlogPostRepository
    {
        Task<IEnumerable<BlogPost>> GetAllPostsAsync(string? searchQuery = null, int pageSize = 100, int pageNumber = 1, string? categoryName = null);
        Task<BlogPost?> GetByIdAsync(Guid id);
        Task<BlogPost> AddAsync(BlogPost blogPost);
        Task<BlogPost?> UpdateAsync(BlogPost blogPost);
        Task<BlogPost?> DeleteAsync(Guid id);
        Task<BlogPost?> GetByUrlHangleAsync(string urlHandle);
        Task<IEnumerable<BlogPost>> SearchPostsAsync(string query);
        Task<IEnumerable<BlogPost>> GetPostsByTagAsync(string tagName);
        Task<int> CountAsync();
        Task<IEnumerable<BlogPost>> GetScheduledPostsAsync();
        Task<IEnumerable<BlogPost>> GetPostsByTagAsync(string tagName, Guid blogId);
    }
}
