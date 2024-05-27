using Bloggie.web.Data;
using Bloggie.web.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bloggie.web.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly BloggieDBContext _dbContext;

        public TagRepository(BloggieDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tag> AddAsync(Tag tag)
        {
            _dbContext.Tags.Add(tag);
            await _dbContext.SaveChangesAsync();
            return tag;
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Tags.CountAsync();
        }


        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var tag = await _dbContext.Tags.FindAsync(id);
            if (tag != null)
            {
                _dbContext.Tags.Remove(tag);
                await _dbContext.SaveChangesAsync();
            }
            return tag;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync(string? searchQuery, string? sortBy,
            string? sortDirection, int pageSize = 100, int pageNumber = 1)
        {
            var query = _dbContext.Tags.Include(t => t.BlogPosts).AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(x => x.Name.Contains(searchQuery) ||
                                         x.DisplayName.Contains(searchQuery));
            }

            // Pagination
            // Skip 0 result take 5  -> page 1 of 5 result 

            var skipResult = (pageNumber - 1) * pageSize;
            query = query.Skip(skipResult).Take(pageSize);
            // Sorting
            if(string.IsNullOrWhiteSpace(sortBy) == false)
            {
                var isDesc = string.Equals(sortDirection, "Desc", StringComparison.OrdinalIgnoreCase);
                if(string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDesc ? query.OrderByDescending(x => x.Name): query.OrderBy(x => x.Name);
                }
                if (string.Equals(sortBy, "DisplayName", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDesc ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName);
                }

            }

            return await query.ToListAsync();
        }


        public async Task<Tag?> GetAsync(Guid id)
        {
            return await _dbContext.Tags.FindAsync(id);
        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
            _dbContext.Entry(tag).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return tag;
        }
    }
}
