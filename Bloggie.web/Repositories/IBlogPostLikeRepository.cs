using Bloggie.web.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bloggie.web.Repositories
{
    public interface IBlogPostLikeRepository
    {
        Task<int> GetTotalLikes(Guid blogPostId);
        Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike);
        Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId);
        Task<BlogPostLike> GetLikeForBlogByUser(Guid blogPostId, Guid userId);
        Task RemoveLike(BlogPostLike like);
    }
}
