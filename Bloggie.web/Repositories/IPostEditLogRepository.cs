using Bloggie.web.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bloggie.web.Repositories
{
    public interface IPostEditLogRepository
    {
        Task<IEnumerable<PostEditLog>> GetAllPostEditLogs();
        Task<PostEditLog> GetPostEditLogById(Guid editId);
        Task AddPostEditLog(PostEditLog editLog);
        Task UpdatePostEditLog(PostEditLog editLog);
        Task DeletePostEditLog(int editId);
    }
}
