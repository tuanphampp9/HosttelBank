using Btl_web_nc.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace Btl_web_nc.Models
{
    public class PostRepository:IPostRepositories
    {
        private readonly AppDbContext _dbContext;
        public PostRepository(AppDbContext db)
        {
            _dbContext = db;
        }
        public Task<Post> AddNewPost(Post post)
        {
            _dbContext.Posts.Add(post);
            _dbContext.SaveChanges();
            return Task.FromResult(post);
        }
        public List<Post> GetAllPosts()
        {
            List<Post> posts = _dbContext.Posts.ToList<Post>();
            return posts;
        }
        public Post GetPostById(long id)
        {
            Post post = _dbContext.Posts.FirstOrDefault(p=> p.postId==id);
            return post;
        }
    }
}
