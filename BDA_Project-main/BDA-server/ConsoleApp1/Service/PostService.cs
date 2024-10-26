using ConsoleApp1.Entity;
using ConsoleApp1.Repository;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PostService> _logger;

        public PostService(IPostRepository postRepository, IUserRepository userRepository, ILogger<PostService> logger)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public void CreatePost(Post post)
        {
            if (post == null)
            {
                _logger.LogWarning("Attempted to create a null post.");
                throw new ArgumentNullException(nameof(post), "Post cannot be null");
            }

            try
            {
                _logger.LogInformation("Creating a new post with title: {title}, authorId: {authorId}", post.title, post.authorId);

                // Check for the presence of the author
                var author = _userRepository.GetUserById(post.authorId);
                if (author == null)
                {
                    _logger.LogWarning("author with ID {authorId} does not exist. Cannot create post.", post.authorId);
                    throw new InvalidOperationException("author does not exist.");
                }

                _postRepository.CreatePost(post);
                _logger.LogInformation("Post '{title}' created successfully.", post.title);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation while creating post '{title}'.", post?.title);
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception while creating post '{title}'.", post?.title);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while creating post '{title}'.", post?.title);
                throw new ApplicationException("An error occurred while creating the post.", ex);
            }
        }

        public Post GetPostById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to retrieve a post with invalid ID: {PostId}.", id);
                throw new ArgumentException("Invalid post ID", nameof(id));
            }

            try
            {
                _logger.LogInformation("Retrieving post with ID: {PostId}.", id);
                var post = _postRepository.GetPostById(id);
                if (post == null)
                {
                    _logger.LogWarning("Post with ID {PostId} not found.", id);
                }
                else
                {
                    _logger.LogInformation("Post '{title}' retrieved successfully.", post.title);
                }
                return post;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception while retrieving post with ID {PostId}.", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while retrieving post with ID {PostId}.", id);
                throw new ApplicationException("An error occurred while retrieving the post.", ex);
            }
        }

        public IEnumerable<Post> GetAllPosts()
        {
            try
            {
                _logger.LogInformation("Retrieving all posts.");
                var posts = _postRepository.GetAllPosts();
                _logger.LogInformation("Retrieved {PostCount} posts successfully.", posts?.Count() ?? 0);
                return posts;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while retrieving all posts.");
                throw new ApplicationException("An error occurred while retrieving the posts.", ex);
            }
        }

        public void DeletePost(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to delete a post with invalid ID: {PostId}.", id);
                throw new ArgumentException("Invalid post ID", nameof(id));
            }

            try
            {
                _logger.LogInformation("Deleting post with ID: {PostId}.", id);
                _postRepository.DeletePost(id);
                _logger.LogInformation("Post with ID {PostId} deleted successfully.", id);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation while deleting post with ID {PostId}.", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while deleting post with ID {PostId}.", id);
                throw new ApplicationException("An error occurred while deleting the post.", ex);
            }
        }
    }
}
