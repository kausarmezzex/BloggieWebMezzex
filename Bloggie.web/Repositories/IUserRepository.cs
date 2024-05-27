using Microsoft.AspNetCore.Identity;

namespace Bloggie.web.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<IdentityUser>> getAll();
    }
}
