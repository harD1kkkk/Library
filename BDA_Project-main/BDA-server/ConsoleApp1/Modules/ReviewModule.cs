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
    public class ReviewModule : NancyModule
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewModule> _logger;

        public ReviewModule(IReviewService reviewService, ILogger<ReviewModule> logger) : base("/api/reviews")
        {
            _reviewService = reviewService;
            _logger = logger;

            // Create a review
            Post("/addReview", parameters =>
            {
                _logger.LogInformation("Request to create a new review.");
                var review = this.Bind<Review>();

                // Model validation
                var validationErrors = ValidateReview(review);
                if (validationErrors.Count > 0)
                {
                    _logger.LogWarning("Validation error when creating review: {Errors}", string.Join(", ", validationErrors));
                    return Response.AsJson(new { errors = validationErrors }, HttpStatusCode.BadRequest);
                }

                try
                {
                    _reviewService.AddReview(review);
                    _logger.LogInformation("Review for Book ID '{BookId}' successfully created.", review.BookId);
                    return Response.AsJson(review, HttpStatusCode.Created);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Conflict while creating review for Book ID '{BookId}'", review.BookId);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.Conflict);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid data when creating review for Book ID '{BookId}'", review.BookId);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while creating review for Book ID '{BookId}'", review.BookId);
                    return Response.AsJson(new { message = "An error occurred while creating the review.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Get all reviews
            Get("/allReviews", parameters =>
            {
                _logger.LogInformation("Request to retrieve all reviews.");
                try
                {
                    var reviews = _reviewService.GetAllReviews();
                    _logger.LogInformation("Reviews retrieved successfully.");
                    return Response.AsJson(reviews, HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while retrieving reviews.");
                    return Response.AsJson(new { message = "An error occurred while retrieving reviews.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Get a review by ID
            Get("/{id:int}", parameters =>
            {
                int id = parameters.id;
                _logger.LogInformation("Request to retrieve review with ID {Id}", id);
                try
                {
                    var review = _reviewService.GetReviewById(id);
                    if (review == null)
                    {
                        _logger.LogWarning("Review with ID {Id} not found.", id);
                        return Response.AsJson(new { message = "Review not found." }, HttpStatusCode.NotFound);
                    }
                    _logger.LogInformation("Review with ID {Id} retrieved successfully.", id);
                    return Response.AsJson(review, HttpStatusCode.OK);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request to retrieve review with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while retrieving review with ID {Id}", id);
                    return Response.AsJson(new { message = "An error occurred while retrieving the review.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Update a review by ID
            Put("/{id:int}", parameters =>
            {
                int id = parameters.id;
                _logger.LogInformation("Request to update review with ID {Id}", id);
                var updatedReview = this.Bind<Review>();

                // Model validation
                var validationErrors = ValidateReview(updatedReview);
                if (validationErrors.Count > 0)
                {
                    _logger.LogWarning("Validation error when updating review with ID {Id}: {Errors}", id, string.Join(", ", validationErrors));
                    return Response.AsJson(new { errors = validationErrors }, HttpStatusCode.BadRequest);
                }

                try
                {
                    updatedReview.Id = id;
                    _reviewService.UpdateReview(updatedReview);
                    _logger.LogInformation("Review with ID {Id} successfully updated.", id);
                    return Response.AsJson(updatedReview, HttpStatusCode.OK);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Conflict while updating review with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.Conflict);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request to update review with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while updating review with ID {Id}", id);
                    return Response.AsJson(new { message = "An error occurred while updating the review.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Delete a review by ID
            Delete("/{id:int}", parameters =>
            {
                int id = parameters.id;
                _logger.LogInformation("Request to delete review with ID {Id}", id);
                try
                {
                    _reviewService.DeleteReview(id);
                    _logger.LogInformation("Review with ID {Id} successfully deleted.", id);
                    return HttpStatusCode.NoContent;
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request to delete review with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while deleting review with ID {Id}", id);
                    return Response.AsJson(new { message = "An error occurred while deleting the review.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });
        }

        private static List<string> ValidateReview(Review review)
        {
            var results = new List<string>();
            var context = new ValidationContext(review);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(review, context, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    results.Add(validationResult.ErrorMessage);
                }
            }

            if (review.Rating < 1 || review.Rating > 5)
            {
                results.Add("Rating must be between 1 and 5.");
            }

            return results;
        }
    }
}
