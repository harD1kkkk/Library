using ConsoleApp1.Entity;

namespace ConsoleApp1.Service
{
    public interface IPostService
    {
        void CreatePost(Post post);
        Post GetPostById(int id);
        IEnumerable<Post> GetAllPosts();
        void DeletePost(int id);
    }
}
