using Bloggie.web.Data;
using Bloggie.web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.web.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BloggieDBContext _dbContext;
        public BlogPostRepository(BloggieDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await _dbContext.AddAsync(blogPost);
            await _dbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlog = await _dbContext.BlogPosts.FindAsync(id);
            if (existingBlog != null)
            {
                _dbContext.BlogPosts.Remove(existingBlog);
                await _dbContext.SaveChangesAsync();
                return existingBlog;
            }
            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllPostsAsync(string searchQuery, int pageSize = 100, int pageNumber = 1, string categoryName = null)
        {
            var query = _dbContext.BlogPosts.AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(x => x.PageTitle.Contains(searchQuery) ||
                                         x.Heading.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                query = query.Where(x => x.Tags.Any(tag => tag.DisplayName.Equals(categoryName, StringComparison.OrdinalIgnoreCase)));
            }

            // Pagination
            query = query.OrderByDescending(bp => bp.PublishDate);
            var skipResult = (pageNumber - 1) * pageSize;
            query = query.Skip(skipResult).Take(pageSize);

            return await query.Include(x => x.Tags).ToListAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(Guid id)
        {
            return await _dbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> GetByUrlHangleAsync(string urlHandle)
        {
            return await _dbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<IEnumerable<BlogPost>> SearchPostsAsync(string query)
        {
            return await _dbContext.BlogPosts
                .Where(post => post.PageTitle.Contains(query) || post.Content.Contains(query))
                .ToListAsync();
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existingPost = await _dbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == blogPost.Id);
            if (existingPost != null)
            {
                existingPost.Id = blogPost.Id;
                existingPost.Author = blogPost.Author;
                existingPost.ShortDescription = blogPost.ShortDescription;
                existingPost.Content = blogPost.Content;
                existingPost.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                existingPost.Author = blogPost.Author;
                existingPost.PublishDate = blogPost.PublishDate;
                existingPost.Visible = blogPost.Visible;
                existingPost.Heading = blogPost.Heading;
                existingPost.PageTitle = blogPost.PageTitle;
                existingPost.UrlHandle = blogPost.UrlHandle;
                existingPost.Tags = blogPost.Tags;
                existingPost.MetaDescription = blogPost.MetaDescription;
                existingPost.MetaKeywords = blogPost.MetaKeywords;
                existingPost.MetaTitle = blogPost.MetaTitle;
                await _dbContext.SaveChangesAsync();
                return existingPost;
            }
            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetPostsByTagAsync(string tagName)
        {
            // Query the database to retrieve posts by tag name
            return await _dbContext.BlogPosts
                .Where(post => post.Tags.Any(tag => tag.Name == tagName))
                .ToListAsync();
        }
        public async Task<IEnumerable<BlogPost>> GetPostsByTagAsync(string tagName,Guid blogId)
        {
            // Query the database to retrieve posts by tag name
            return await _dbContext.BlogPosts
                .Where(post => post.Id != blogId && post.Tags.Any(tag => tag.Name == tagName))
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.BlogPosts.CountAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetScheduledPostsAsync()
        {
            return await _dbContext.BlogPosts
                .Where(post => post.ScheduledPublishDate <= DateTime.Now && !post.Visible)
                .ToListAsync();
        }

    }
}
