using ConsoleApp1.Entity;
using System.Collections.Generic;
namespace ConsoleApp1.Repository
{
    public interface IPostRepository
    {
        void CreatePost(Post post);
        Post GetPostById(int id);
        IEnumerable<Post> GetAllPosts();
        void DeletePost(int id);
    }
}

