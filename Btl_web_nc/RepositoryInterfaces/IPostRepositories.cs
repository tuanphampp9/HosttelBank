using Btl_web_nc.Models;

namespace Btl_web_nc.RepositoryInterfaces
{
    public interface IPostRepositories
    {
        public Task<Post> AddNewPost(Post post);
        public List<Post> GetAllPosts();
        public Post GetPostById(long id);
    }
}
