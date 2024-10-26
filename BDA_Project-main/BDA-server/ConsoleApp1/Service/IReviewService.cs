using ConsoleApp1.Entity;
using System.Collections.Generic;

namespace ConsoleApp1.Service
{
    public interface IReviewService
    {
        void AddReview(Review review);
        Review GetReviewById(int id);
        IEnumerable<Review> GetAllReviews();
        void DeleteReview(int id);
        void UpdateReview(Review review);
    }
}
