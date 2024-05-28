using Azure;
using Bloggie.web.Models.Domain;
using Bloggie.web.Models.ViewModel;
using Bloggie.web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bloggie.web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IBlogPostLikeRepository _blogPostLikeRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBlogPostCommentRepository _blogPostCommentRepository;
        private readonly ITagRepository _tagRepository;

        public BlogsController(IBlogPostRepository blogPostRepository, IBlogPostLikeRepository blogPostLikeRepository,
            SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,
            IBlogPostCommentRepository blogPostCommentRepository, ITagRepository tagRepository)
        {
            _blogPostRepository = blogPostRepository;
            _blogPostLikeRepository = blogPostLikeRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _blogPostCommentRepository = blogPostCommentRepository;
            _tagRepository = tagRepository;
        }

        [HttpGet("/Blogs/{tagName}/{urlHandle}")]
        public async Task<IActionResult> Index(string tagName, string urlHandle)
        {
            var blogPost = await _blogPostRepository.GetByUrlHangleAsync(urlHandle);
            if (blogPost == null) return NotFound();

            var totalLikes = await _blogPostLikeRepository.GetTotalLikes(blogPost.Id);
            var liked = await IsUserLikedBlog(blogPost.Id);

            var blogComments = await GetBlogComments(blogPost.Id);
            var totalComments = blogComments.Count;

            var tagNames = blogPost.Tags.Select(t => t.Name).ToList();

            // Fetch related posts by tag
            var relatedPosts = await _blogPostRepository.GetPostsByTagAsync(tagName, blogPost.Id);

            var blogDetailsViewModel = new BlogDetailsViewModel
            {
                Id = blogPost.Id,
                Content = blogPost.Content,
                PageTitle = blogPost.PageTitle,
                Author = blogPost.Author,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                Heading = blogPost.Heading,
                PublishDate = blogPost.PublishDate,
                ShortDescription = blogPost.ShortDescription,
                UrlHandle = blogPost.UrlHandle,
                Visible = blogPost.Visible,
                Tags = blogPost.Tags,
                TotalLikes = totalLikes,
                Liked = liked,
                Comments = blogComments,
                TotalComments = totalComments,
                MetaKeywords = blogPost.MetaKeywords,
                MetaDescription = blogPost.MetaDescription,
                MetaTitle = blogPost.MetaTitle,
                TagName = tagName,
                RelatedPosts = relatedPosts.ToList(),
            };

            return View(blogDetailsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var userId = _userManager.GetUserId(User);
                if (Guid.TryParse(userId, out var parsedUserId))
                {
                    var domainModel = new BlogPostComment
                    {
                        BlogPostId = blogDetailsViewModel.Id,
                        Description = blogDetailsViewModel.CommentDescription,
                        UserId = parsedUserId,
                        DateAdded = DateTime.Now
                    };

                    await _blogPostCommentRepository.AddAsync(domainModel);
                    return RedirectToAction("Index", new { tagName = blogDetailsViewModel.TagName, urlHandle = blogDetailsViewModel.UrlHandle });
                }
            }

            return View(blogDetailsViewModel);
        }


        private async Task<bool> IsUserLikedBlog(Guid blogPostId)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var userId = _userManager.GetUserId(User);
                if (Guid.TryParse(userId, out var parsedUserId))
                {
                    var likesForBlog = await _blogPostLikeRepository.GetLikesForBlog(blogPostId);
                    return likesForBlog.Any(x => x.UserId == parsedUserId);
                }
            }
            return false;
        }

        private async Task<List<BlogComment>> GetBlogComments(Guid blogPostId)
        {
            var blogCommentsDomainModel = await _blogPostCommentRepository.GetCommentByBlogIdAsync(blogPostId);
            var blogCommentForView = new List<BlogComment>();

            foreach (var blogComment in blogCommentsDomainModel)
            {
                var user = await _userManager.FindByIdAsync(blogComment.UserId.ToString());
                blogCommentForView.Add(new BlogComment
                {
                    DateAdded = blogComment.DateAdded,
                    Description = blogComment.Description,
                    Username = user?.UserName
                });
            }

            return blogCommentForView;
        }
    }
}
