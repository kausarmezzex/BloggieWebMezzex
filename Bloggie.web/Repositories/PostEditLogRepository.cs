using Bloggie.web.Data;
using Bloggie.web.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bloggie.web.Repositories
{
    public class PostEditLogRepository : IPostEditLogRepository
    {
        private readonly BloggieDBContext _dbContext;

        public PostEditLogRepository(BloggieDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PostEditLog>> GetAllPostEditLogs()
        {
            return await GetPostEditLogsQuery().ToListAsync();
        }

        public async Task<PostEditLog> GetPostEditLogById(Guid editId)
        {
            return await GetPostEditLogsQuery().FirstOrDefaultAsync(log => log.ID == editId);
        }

        public async Task AddPostEditLog(PostEditLog editLog)
        {
            _dbContext.PostEditLogs.Add(editLog);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePostEditLog(PostEditLog editLog)
        {
            _dbContext.Entry(editLog).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePostEditLog(int editId)
        {
            var editLog = await _dbContext.PostEditLogs.FindAsync(editId);
            if (editLog != null)
            {
                _dbContext.PostEditLogs.Remove(editLog);
                await _dbContext.SaveChangesAsync();
            }
        }

        private IQueryable<PostEditLog> GetPostEditLogsQuery()
        {
            return _dbContext.PostEditLogs
                .Include(log => log.BlogPost);
        }
    }
}
