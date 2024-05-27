using Bloggie.web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IBlogPostCommentRepository _blogPostCommentRepository;
        private readonly IPostEditLogRepository postEditLogRepository;

        public AdminDashboardController(IBlogPostRepository blogPostRepository, UserManager<IdentityUser> userManager, IBlogPostCommentRepository blogPostCommentRepository, IPostEditLogRepository postEditLogRepository)
        {
            _blogPostRepository = blogPostRepository;
            this.userManager = userManager;
            _blogPostCommentRepository = blogPostCommentRepository;
            this.postEditLogRepository = postEditLogRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Query number of posts, users, and comments
            var numberOfPosts = await _blogPostRepository.CountAsync();
            var numberOfUsers =  userManager.Users.Count(); ;

            // Pass data to view
            ViewBag.NumberOfPosts = numberOfPosts;
            ViewBag.NumberOfUsers = numberOfUsers;

            return View();
        }

        public async Task<IActionResult> History()
        {
            // Query post edit logs
            var postEditLogs = await postEditLogRepository.GetAllPostEditLogs();
            return View(postEditLogs);
        }
    }
}
