using ConsoleApp1.Entity;
using ConsoleApp1.Service;
using Microsoft.Extensions.Logging;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Modules
{
    public class PostModule : NancyModule
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostModule> _logger; 

        public PostModule(IPostService postService, ILogger<PostModule> logger) : base("/api/posts")
        {
            _postService = postService;
            _logger = logger;

            // Create post
            Post("/", parameters =>
            {
                _logger.LogInformation("Request to create a new post.");
                var post = this.Bind<Post>();

                // Model validation
                var validationErrors = ValidatePost(post);
                if (validationErrors.Count > 0)
                {
                    _logger.LogWarning("Validation error when creating post: {Errors}", string.Join(", ", validationErrors));
                    return Response.AsJson(new { errors = validationErrors }, HttpStatusCode.BadRequest);
                }

                try
                {
                    _postService.CreatePost(post);
                    _logger.LogInformation("Post '{title}' successfully created.", post.title);
                    return HttpStatusCode.Created;
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Conflict while creating post '{title}'", post.title);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.Conflict);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid data when creating post '{title}'", post.title);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while creating post '{title}'", post.title);
                    return Response.AsJson(new { message = "An error occurred while creating the post.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Get all posts
            Get("/", parameters =>
            {
                _logger.LogInformation("Request to retrieve all posts.");
                try
                {
                    var posts = _postService.GetAllPosts();
                    _logger.LogInformation("Posts retrieved successfully.");
                    return Response.AsJson(posts, HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while retrieving posts.");
                    return Response.AsJson(new { message = "An error occurred while retrieving posts.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Get post by ID
            Get("/{id:int}", parameters =>
            {
                int id = parameters.id;
                _logger.LogInformation("Request to retrieve post with ID {Id}", id);
                try
                {
                    var post = _postService.GetPostById(id);
                    if (post == null)
                    {
                        _logger.LogWarning("Post with ID {Id} not found.", id);
                        return Response.AsJson(new { message = "Post not found." }, HttpStatusCode.NotFound);
                    }
                    _logger.LogInformation("Post with ID {Id} retrieved successfully.", id);
                    return Response.AsJson(post, HttpStatusCode.OK);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request to retrieve post with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while retrieving post with ID {Id}", id);
                    return Response.AsJson(new { message = "An error occurred while retrieving the post.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Delete post by ID
            Delete("/{id:int}", parameters =>
            {
                int id = parameters.id;
                _logger.LogInformation("Request to delete post with ID {Id}", id);
                try
                {
                    _postService.DeletePost(id);
                    _logger.LogInformation("Post with ID {Id} successfully deleted.", id);
                    return HttpStatusCode.NoContent;
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request to delete post with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while deleting post with ID {Id}", id);
                    return Response.AsJson(new { message = "An error occurred while deleting the post.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });
        }

        private static List<string> ValidatePost(Post post)
        {
            var results = new List<string>();
            var context = new ValidationContext(post);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(post, context, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    results.Add(validationResult.ErrorMessage);
                }
            }
            return results;
        }
    }
}
