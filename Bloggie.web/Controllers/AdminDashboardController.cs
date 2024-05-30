using Bloggie.web.Models.Domain;
using Bloggie.web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OfficeOpenXml;

namespace Bloggie.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBlogPostCommentRepository _blogPostCommentRepository;
        private readonly IPostEditLogRepository _postEditLogRepository;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;

        public AdminDashboardController(
            IBlogPostRepository blogPostRepository,
            UserManager<IdentityUser> userManager,
            IBlogPostCommentRepository blogPostCommentRepository,
            IPostEditLogRepository postEditLogRepository,
            ICompositeViewEngine viewEngine,
            ITempDataProvider tempDataProvider)
        {
            _blogPostRepository = blogPostRepository;
            _userManager = userManager;
            _blogPostCommentRepository = blogPostCommentRepository;
            _postEditLogRepository = postEditLogRepository;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        public async Task<IActionResult> Index()
        {
            var numberOfPosts = await _blogPostRepository.CountAsync();
            var numberOfUsers = _userManager.Users.Count();

            ViewBag.NumberOfPosts = numberOfPosts;
            ViewBag.NumberOfUsers = numberOfUsers;

            return View();
        }

        public async Task<IActionResult> History()
        {
            var postEditLogs = await _postEditLogRepository.GetAllPostEditLogs();
            return View(postEditLogs);
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var logs = await _postEditLogRepository.GetAllPostEditLogs();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("PostEditLogs");
                worksheet.Cells["A1"].Value = "Post ID";
                worksheet.Cells["B1"].Value = "Post Title";
                worksheet.Cells["C1"].Value = "Author";
                worksheet.Cells["D1"].Value = "Edited By";
                worksheet.Cells["E1"].Value = "Edit Timestamp";
                worksheet.Cells["F1"].Value = "Post Showing on Home Page";
                worksheet.Cells["G1"].Value = "Edit Description";

                int row = 2;
                foreach (var log in logs)
                {
                    worksheet.Cells[row, 1].Value = log.PostID;
                    worksheet.Cells[row, 2].Value = log.BlogPost.Heading;
                    worksheet.Cells[row, 3].Value = log.BlogPost.Author;
                    worksheet.Cells[row, 4].Value = log.AdminEmail;
                    worksheet.Cells[row, 5].Value = log.EditTimestamp;
                    worksheet.Cells[row, 6].Value = log.BlogPost.Visible ? "Yes" : "No";
                    worksheet.Cells[row, 7].Value = log.EditDescription;
                    row++;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PostEditLogs.xlsx");
            }
        }

    }
}
