using Bloggie.web.Models.Domain;
using Bloggie.web.Models.ViewModel;
using Bloggie.web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Bloggie.web.Controllers
{
    public class AdminBlogPostsController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IPostEditLogRepository _postEditLogRepository;

        public AdminBlogPostsController(ITagRepository tagRepository, IBlogPostRepository postRepository, IPostEditLogRepository postEditLogRepository)
        {
            _tagRepository = tagRepository;
            _blogPostRepository = postRepository;
            _postEditLogRepository = postEditLogRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddBlogPostRequest
            {
                Tags = await GetTagsAsync()
            };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            return await HandleBlogPostOperation(async () =>
            {
                var blogPost = MapToDomain(addBlogPostRequest);
                blogPost.Tags = await GetSelectedTagsAsync(addBlogPostRequest.SelectedTags);
                await _blogPostRepository.AddAsync(blogPost);
                TempData["success"] = "Blog post created successfully.";
                return RedirectToAction("Add");
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> List(string searchQuery = null, int pageSize = 10, int pageNumber = 1)
        {
            var totalRecords = await _blogPostRepository.CountAsync();
            var totalPages = Math.Ceiling((decimal)totalRecords / pageSize);
            var blogPosts = await _blogPostRepository.GetAllPostsAsync(searchQuery, pageSize, pageNumber);

            ViewBag.SearchQuery = searchQuery;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            return View(blogPosts);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null)
            {
                return View(null);
            }

            var model = new EditBlogPostRequest
            {
                Id = blogPost.Id,
                Heading = blogPost.Heading,
                Author = blogPost.Author,
                Visible = blogPost.Visible,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                UrlHandle = blogPost.UrlHandle,
                ShortDescription = blogPost.ShortDescription,
                PageTitle = blogPost.PageTitle,
                PublishDate = blogPost.PublishDate,
                MetaKeywords = blogPost.MetaKeywords,
                MetaTitle = blogPost.MetaTitle,
                MetaDescription = blogPost.MetaDescription,
                ScheduledPublishDate = blogPost.ScheduledPublishDate,
                Tags = await GetTagsAsync(),
                SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray(),
            };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPost)
        {
            return await HandleBlogPostOperation(async () =>
            {
                var originalBlogPost = await _blogPostRepository.GetByIdAsync(editBlogPost.Id);
                if (originalBlogPost == null)
                {
                    TempData["error"] = "Blog post not found.";
                    return NotFound();
                }

                var updatedBlogPost = MapToDomain(editBlogPost);
                updatedBlogPost.Tags = await GetSelectedTagsAsync(editBlogPost.SelectedTags);

                var changes = DetectChanges(originalBlogPost, updatedBlogPost);
                var editDescription = string.Join("; ", changes);

                var updatedBlog = await _blogPostRepository.UpdateAsync(updatedBlogPost);
                if (updatedBlog != null)
                {
                    await LogEditAsync(editBlogPost.Id, editDescription, updatedBlog);
                    TempData["success"] = "Blog post updated successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["error"] = "An error occurred while updating the blog post.";
                    return RedirectToAction("Edit", new { id = editBlogPost.Id });
                }
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPost)
        {
            var deletedPost = await _blogPostRepository.DeleteAsync(editBlogPost.Id);
            if (deletedPost != null)
            {
                TempData["success"] = "Blog post deleted successfully.";
                return RedirectToAction("List");
            }

            TempData["error"] = "Failed to delete blog post.";
            return RedirectToAction("Edit", new { id = editBlogPost.Id });
        }

        private async Task<IEnumerable<SelectListItem>> GetTagsAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            return tags.Select(x => new SelectListItem { Text = x.DisplayName, Value = x.Id.ToString() });
        }

        private async Task<List<Tag>> GetSelectedTagsAsync(IEnumerable<string> selectedTagIds)
        {
            var selectedTags = new List<Tag>();
            foreach (var selectedTagId in selectedTagIds)
            {
                if (Guid.TryParse(selectedTagId, out var tagId))
                {
                    var existingTag = await _tagRepository.GetAsync(tagId);
                    if (existingTag != null)
                    {
                        selectedTags.Add(existingTag);
                    }
                }
            }
            return selectedTags;
        }

        private BlogPost MapToDomain(AddBlogPostRequest request)
        {
            return new BlogPost
            {
                Heading = request.Heading,
                PageTitle = request.PageTitle,
                Content = request.Content,
                ShortDescription = request.ShortDescription,
                FeaturedImageUrl = request.FeaturedImageUrl,
                UrlHandle = request.UrlHandle,
                PublishDate = request.PublishDate,
                Author = request.Author,
                Visible = request.Visible,
                MetaDescription = request.MetaDescription,
                MetaKeywords = request.MetaKeywords,
                MetaTitle = request.MetaTitle,
                ScheduledPublishDate = request.ScheduledPublishDate,
            };
        }

        private BlogPost MapToDomain(EditBlogPostRequest request)
        {
            return new BlogPost
            {
                Id = request.Id,
                Heading = request.Heading,
                PageTitle = request.PageTitle,
                Content = request.Content,
                ShortDescription = request.ShortDescription,
                FeaturedImageUrl = request.FeaturedImageUrl,
                UrlHandle = request.UrlHandle,
                PublishDate = request.PublishDate,
                Author = request.Author,
                Visible = request.Visible,
                MetaDescription = request.MetaDescription,
                MetaKeywords = request.MetaKeywords,
                MetaTitle = request.MetaTitle,
                ScheduledPublishDate = request.ScheduledPublishDate,
            };
        }

        private List<string> DetectChanges(BlogPost originalBlogPost, BlogPost updatedBlogPost)
        {
            var changes = new List<string>();

            if (originalBlogPost.Heading != updatedBlogPost.Heading)
                changes.Add($"Heading changed from '{originalBlogPost.Heading}' to '{updatedBlogPost.Heading}'");
            if (originalBlogPost.Content != updatedBlogPost.Content)
                changes.Add("Content changed");
            if (originalBlogPost.MetaKeywords != updatedBlogPost.MetaKeywords)
                changes.Add($"MetaKeywords changed from '{originalBlogPost.MetaKeywords}' to '{updatedBlogPost.MetaKeywords}'");
            if (originalBlogPost.MetaTitle != updatedBlogPost.MetaTitle)
                changes.Add($"MetaTitle changed from '{originalBlogPost.MetaTitle}' to '{updatedBlogPost.MetaTitle}'");
            if (originalBlogPost.MetaDescription != updatedBlogPost.MetaDescription)
                changes.Add($"MetaDescription changed from '{originalBlogPost.MetaDescription}' to '{updatedBlogPost.MetaDescription}'");
            if (originalBlogPost.PageTitle != updatedBlogPost.PageTitle)
                changes.Add($"Page Title changed from '{originalBlogPost.PageTitle}' to '{updatedBlogPost.PageTitle}'");
            if (originalBlogPost.ShortDescription != updatedBlogPost.ShortDescription)
                changes.Add($"Short Description changed from '{originalBlogPost.ShortDescription}' to '{updatedBlogPost.ShortDescription}'");
            if (originalBlogPost.FeaturedImageUrl != updatedBlogPost.FeaturedImageUrl)
                changes.Add($"Featured Image URL changed from '{originalBlogPost.FeaturedImageUrl}' to '{updatedBlogPost.FeaturedImageUrl}'");
            if (originalBlogPost.UrlHandle != updatedBlogPost.UrlHandle)
                changes.Add($"URL Handle changed from '{originalBlogPost.UrlHandle}' to '{updatedBlogPost.UrlHandle}'");
            if (originalBlogPost.PublishDate != updatedBlogPost.PublishDate)
                changes.Add($"Publish Date changed from '{originalBlogPost.PublishDate}' to '{updatedBlogPost.PublishDate}'");
            if (originalBlogPost.Author != updatedBlogPost.Author)
                changes.Add($"Author changed from '{originalBlogPost.Author}' to '{updatedBlogPost.Author}'");
            if (originalBlogPost.Visible != updatedBlogPost.Visible)
                changes.Add($"Visibility changed from '{originalBlogPost.Visible}' to '{updatedBlogPost.Visible}'");

            var originalTagNames = originalBlogPost.Tags.Select(t => t.Name).ToHashSet();
            var updatedTagNames = updatedBlogPost.Tags.Select(t => t.Name).ToHashSet();
            var addedTags = updatedTagNames.Except(originalTagNames);
            var removedTags = originalTagNames.Except(updatedTagNames);

            if (addedTags.Any())
                changes.Add($"Added tags: {string.Join(", ", addedTags)}");
            if (removedTags.Any())
                changes.Add($"Removed tags: {string.Join(", ", removedTags)}");

            return changes;
        }

        private async Task LogEditAsync(Guid postId, string editDescription, BlogPost updatedBlogPost)
        {
            var adminId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var adminEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);

            var istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var localEditTimestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istTimeZone);

            var editLog = new PostEditLog
            {
                PostID = postId,
                AdminID = adminId,
                EditTimestamp = localEditTimestamp,
                EditDescription = editDescription,
                AdminEmail = adminEmail,
                BlogPost = updatedBlogPost
            };

            await _postEditLogRepository.AddPostEditLog(editLog);
        }

        private async Task<IActionResult> HandleBlogPostOperation(Func<Task<IActionResult>> blogPostOperation)
        {
            try
            {
                return await blogPostOperation();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Error");
            }
        }
    }
}
