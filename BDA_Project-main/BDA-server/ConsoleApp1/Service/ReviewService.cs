using ConsoleApp1.Entity;
using ConsoleApp1.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ConsoleApp1.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IReviewRepository reviewRepository, ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

        public void AddReview(Review review)
        {
            if (review == null)
            {
                _logger.LogWarning("Attempted to add a null review.");
                throw new ArgumentNullException(nameof(review), "Review cannot be null");
            }

            try
            {
                _logger.LogInformation("Adding review for Book ID {BookId} by User ID {UserId}.", review.BookId, review.UserId);
                _reviewRepository.AddReview(review);
                _logger.LogInformation("Review added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while adding review.");
                throw new ApplicationException("An error occurred while adding the review.", ex);
            }
        }

        public Review GetReviewById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to retrieve a review with invalid ID {Id}.", id);
                throw new ArgumentException("Invalid review ID", nameof(id));
            }

            return _reviewRepository.GetReviewById(id);
        }

        public IEnumerable<Review> GetAllReviews()
        {
            return _reviewRepository.GetAllReviews();
        }

        public void DeleteReview(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to delete a review with invalid ID {Id}.", id);
                throw new ArgumentException("Invalid review ID", nameof(id));
            }

            _reviewRepository.DeleteReview(id);
        }

        public void UpdateReview(Review review)
        {
            if (review == null)
            {
                _logger.LogWarning("Attempted to update a null review.");
                throw new ArgumentNullException(nameof(review), "Review cannot be null");
            }

            _reviewRepository.UpdateReview(review);
        }
    }
}
