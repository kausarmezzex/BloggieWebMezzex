using Bloggie.web.Models.Domain;
using Bloggie.web.Models.ViewModel;
using Bloggie.web.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Bloggie.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostLikeController : ControllerBase
    {
        private readonly IBlogPostLikeRepository blogPostLikeRepository;

        public BlogPostLikeController(IBlogPostLikeRepository blogPostLikeRepository)
        {
            this.blogPostLikeRepository = blogPostLikeRepository;
        }

        [HttpPost("AddOrRemove")]
        public async Task<IActionResult> AddOrRemoveLike([FromBody] AddLikeRequest addLikeRequest)
        {
            var existingLike = await blogPostLikeRepository.GetLikeForBlogByUser(addLikeRequest.BlogPostId, addLikeRequest.UserId);

            if (existingLike != null)
            {
                await blogPostLikeRepository.RemoveLike(existingLike);
            }
            else
            {
                var model = new BlogPostLike
                {
                    BlogPostId = addLikeRequest.BlogPostId,
                    UserId = addLikeRequest.UserId
                };
                await blogPostLikeRepository.AddLikeForBlog(model);
            }

            return Ok();
        }

        [HttpGet("{blogPostId:Guid}/totalLikes")]
        public async Task<IActionResult> GetTotalLikesForBlog([FromRoute] Guid blogPostId)
        {
            var totalLikes = await blogPostLikeRepository.GetTotalLikes(blogPostId);
            return Ok(totalLikes);
        }
    }
}
