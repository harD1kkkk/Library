using ConsoleApp1.Entity;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConsoleApp1.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ReviewRepository> _logger;

        public ReviewRepository(string connectionString, ILogger<ReviewRepository> logger)
        {
            _connectionString = connectionString;
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
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO reviews (user_id, book_id, review_text, rating, created_at, is_moderated) " +
                              "VALUES (@UserId, @BookId, @ReviewText, @Rating, @createdAt, @IsModerated)";
                    db.Execute(sql, review);
                    _logger.LogInformation("Review added successfully for User ID {UserId} and Book ID {BookId}.", review.UserId, review.BookId);
                }
            }
            catch (MySqlException ex)
            {
                HandleMySqlException(ex, review);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while adding a review for User ID {UserId}.", review.UserId);
                throw new ApplicationException("An error occurred while adding the review.", ex);
            }
        }

        public Review GetReviewById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to retrieve a review with invalid ID {ReviewId}.", id);
                throw new ArgumentException("Invalid review ID", nameof(id));
            }

            try
            {
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var review = db.QueryFirstOrDefault<Review>("SELECT * FROM reviews WHERE id = @Id", new { Id = id });
                    if (review == null)
                    {
                        _logger.LogWarning("Review with ID {ReviewId} not found.", id);
                    }
                    else
                    {
                        _logger.LogInformation("Review with ID {ReviewId} retrieved successfully.", id);
                    }
                    return review;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while retrieving review with ID {ReviewId}.", id);
                throw new ApplicationException("An error occurred while retrieving the review.", ex);
            }
        }

        public IEnumerable<Review> GetAllReviews()
        {
            try
            {
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    return db.Query<Review>("SELECT * FROM reviews").ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while retrieving all reviews.");
                throw new ApplicationException("An error occurred while retrieving all reviews.", ex);
            }
        }

        public void DeleteReview(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to delete a review with invalid ID {ReviewId}.", id);
                throw new ArgumentException("Invalid review ID", nameof(id));
            }

            try
            {
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var sql = "DELETE FROM reviews WHERE id = @Id";
                    int rowsAffected = db.Execute(sql, new { Id = id });
                    if (rowsAffected > 0)
                    {
                        _logger.LogInformation("Review with ID {ReviewId} deleted successfully.", id);
                    }
                    else
                    {
                        _logger.LogWarning("No review found with ID {ReviewId} to delete.", id);
                    }
                }
            }
            catch (MySqlException ex)
            {
                HandleMySqlException(ex, null); // Handle specific to Review context
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while deleting review with ID {ReviewId}.", id);
                throw new ApplicationException("An error occurred while deleting the review.", ex);
            }
        }

        public void UpdateReview(Review review)
        {
            _logger.LogInformation("Updating review with ID {ReviewId} for User ID {UserId} and Book ID {BookId}.", review.Id, review.UserId, review.BookId);
            if (review == null)
            {
                _logger.LogWarning("Attempted to update a null review.");
                throw new ArgumentNullException(nameof(review), "Review cannot be null");
            }

            try
            {
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var sql = "UPDATE reviews SET user_id = @UserId, book_id = @BookId, review_text = @ReviewText, " +
                              "rating = @Rating, created_at = @createdAt, is_moderated = @IsModerated WHERE id = @Id";
                    int rowsAffected = db.Execute(sql, review);
                    if (rowsAffected > 0)
                    {
                        _logger.LogInformation("Review with ID {ReviewId} updated successfully.", review.Id);
                    }
                    else
                    {
                        _logger.LogWarning("No review found with ID {ReviewId} to update.", review.Id);
                    }
                }
            }
            catch (MySqlException ex)
            {
                HandleMySqlException(ex, review);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while updating review with ID {ReviewId}.", review.Id);
                throw new ApplicationException("An error occurred while updating the review.", ex);
            }
        }

        private void HandleMySqlException(MySqlException ex, Review? review)
        {
            // Log the MySQL specific errors
            switch (ex.Number)
            {
                case 1062: // Duplicate entry
                    _logger.LogWarning("Attempt to add review failed: Duplicate entry. Review: {ReviewText}", review?.ReviewText);
                    throw new InvalidOperationException("Duplicate entry for review.", ex);
                case 1045: // Access denied
                    _logger.LogError(ex, "Access denied while trying to add/update/delete review. Review: {ReviewText}", review?.ReviewText);
                    throw new InvalidOperationException("Access denied. Please check your database credentials.", ex);
                case 1049: // Unknown database
                    _logger.LogError(ex, "The specified database does not exist when accessing review. Review: {ReviewText}", review?.ReviewText);
                    throw new InvalidOperationException("The specified database does not exist.", ex);
                case 2002: // Connection error
                    _logger.LogError(ex, "Could not connect to the database server while accessing review. Review: {ReviewText}", review?.ReviewText);
                    throw new InvalidOperationException("Could not connect to the database server. Please check your connection settings.", ex);
                case 1054: // Unknown column
                    _logger.LogError(ex, "One or more columns in the query statement do not exist for review. Review: {ReviewText}", review?.ReviewText);
                    throw new InvalidOperationException("One or more columns in the query statement do not exist.", ex);
                case 1146: // Table doesn't exist
                    _logger.LogError(ex, "The specified table does not exist when accessing review. Review: {ReviewText}", review?.ReviewText);
                    throw new InvalidOperationException("The specified table does not exist.", ex);
                case 1213: // Deadlock
                    _logger.LogWarning("A deadlock occurred while trying to access review. Review: {ReviewText}", review?.ReviewText);
                    throw new InvalidOperationException("A deadlock occurred. Please try again.", ex);
                default:
                    _logger.LogError(ex, "An error occurred while accessing review. Review: {ReviewText}", review?.ReviewText);
                    throw new InvalidOperationException("An error occurred while accessing the review.", ex);
            }
        }
    }
}
