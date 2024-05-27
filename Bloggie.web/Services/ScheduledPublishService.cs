using Bloggie.web.Data;
using Bloggie.web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.web.Services
{
    public class ScheduledPostPublisher : BackgroundService
    {
        private readonly ILogger<ScheduledPostPublisher> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScheduledPostPublisher(ILogger<ScheduledPostPublisher> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var serviceProvider = scope.ServiceProvider;
                        var dbContext = serviceProvider.GetRequiredService<BloggieDBContext>();
                        var blogPostRepository = serviceProvider.GetRequiredService<IBlogPostRepository>();

                        // Check for scheduled posts and publish them
                        await PublishScheduledPostsAsync(dbContext, blogPostRepository);
                    }

                    // Wait for some time before checking again (e.g., every minute)
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
               }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while publishing scheduled posts.");
                }
            }
        }

        private async Task PublishScheduledPostsAsync(BloggieDBContext dbContext, IBlogPostRepository blogPostRepository)
        {
            var now = DateTime.Now;

            // Fetch scheduled posts that have a ScheduledPublishDate in the past
            var scheduledPosts = await dbContext.BlogPosts
                .Where(post => post.ScheduledPublishDate <= now && !post.Visible)
                .ToListAsync();

            foreach (var post in scheduledPosts)
            {
                // Update the post to mark it as published
                post.PublishDate = now;
                post.Visible = true;
                post.ScheduledPublishDate = now;

                // Save the changes to the database
                await blogPostRepository.UpdateAsync(post);

                _logger.LogInformation($"Published scheduled post with ID: {post.Id}");
            }
        }
    }
}
