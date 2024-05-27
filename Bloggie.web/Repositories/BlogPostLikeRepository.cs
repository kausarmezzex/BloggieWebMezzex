using Bloggie.web.Data;
using Bloggie.web.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggie.web.Repositories
{
    public class BlogPostLikeRepository : IBlogPostLikeRepository
    {
        private readonly BloggieDBContext dBContext;

        public BlogPostLikeRepository(BloggieDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public async Task<int> GetTotalLikes(Guid blogPostId)
        {
            return await dBContext.Likes.CountAsync(x => x.BlogPostId == blogPostId);
        }

        public async Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike)
        {
            await dBContext.Likes.AddAsync(blogPostLike);
            await dBContext.SaveChangesAsync();
            return blogPostLike;
        }

        public async Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId)
        {
            return await dBContext.Likes.Where(x => x.BlogPostId == blogPostId).ToListAsync();
        }

        public async Task<BlogPostLike> GetLikeForBlogByUser(Guid blogPostId, Guid userId)
        {
            return await dBContext.Likes.FirstOrDefaultAsync(x => x.BlogPostId == blogPostId && x.UserId == userId);
        }
        public async Task RemoveLike(BlogPostLike like)
        {
            dBContext.Likes.Remove(like);
            await dBContext.SaveChangesAsync();
        }

    }
}
