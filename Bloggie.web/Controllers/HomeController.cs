using Bloggie.web.Models;
using Bloggie.web.Models.Domain;
using Bloggie.web.Models.ViewModel;
using Bloggie.web.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bloggie.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogPostRepository _postRepository;
        private readonly ITagRepository _tagRepository;

        public HomeController(IBlogPostRepository postRepository, ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _tagRepository = tagRepository;
        }

        public async Task<IActionResult> Index(string selectedTagName, string query = null, int pageSize = 9, int pageNumber = 1)
        {
            var model = await GetHomeViewModelAsync(selectedTagName, query, pageSize, pageNumber);
            return View(model);
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index");
            }

            var model = await GetHomeViewModelAsync(query);
            return View("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetPostsByTag(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                return RedirectToAction("Index");
            }

            var model = await GetHomeViewModelAsync(tagName);
            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<HomeViewModel> GetHomeViewModelAsync(string selectedTagName = null, string searchQuery = null, int pageSize = 9, int pageNumber = 1)
        {
            var totalRecords = await _postRepository.CountAsync();
            var totalPages = Math.Ceiling((decimal)totalRecords / pageSize);

            if (pageNumber > totalPages)
            {
                pageNumber = (int)totalPages;
            }

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            var blogPosts = await _postRepository.GetAllPostsAsync(searchQuery, pageSize, pageNumber);
            var tags = await _tagRepository.GetAllAsync();

            // Filter posts by selected tag name
            if (!string.IsNullOrWhiteSpace(selectedTagName))
            {
                blogPosts = blogPosts.Where(post => post.Tags.Any(tag => tag.Name.Equals(selectedTagName, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            ViewBag.SearchQuery = searchQuery;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;
            ViewData["SelectedTagName"] = selectedTagName;

            return new HomeViewModel
            {
                BlogPosts = blogPosts,
                Tags = tags
            };
        }


        public async Task<IActionResult> PostsByCategory(string categoryName, int pageSize = 9, int pageNumber = 1)
        {
            var model = await GetHomeViewModelAsync(categoryName, null, pageSize, pageNumber);
            ViewBag.CategoryName = categoryName;
            return View(model);
        }



    }
}
