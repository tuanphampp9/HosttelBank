using Btl_web_nc.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Btl_web_nc.Models
{
    public class PostRepository : IPostRepositories
    {

        private readonly AppDbContext _dbContext;
        public PostRepository(AppDbContext db)
        {
            _dbContext = db;
        }

        public async Task<Boolean> DeletePostAsync(int postId)
        {
            try
            {
                // Tìm bài đăng cần xóa
                var post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.postId.Equals(postId));

                // Nếu bài đăng không tồn tại, không làm gì
                if (post == null)
                {
                    return false;
                }

                // Xóa bài đăng khỏi DbContext
                _dbContext.Posts.Remove(post);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ khi xóa bị lỗi (ví dụ: ghi log, ném lại ngoại lệ, ...)
                throw new Exception("Xóa bài đăng không thành công", ex);
            }
        }

        public List<Post> GetAllPosts()
        {
            return _dbContext.Posts.ToList();
        }

        public async Task<Post> GetPostByIdAsync(int postID)
        {
            var post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.postId == postID);
            return post ?? new Post();
        }

        public async Task<PostManageViewModel> GetPostViewModelByIdAsync(int id)
        {
            var post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.postId == id);
            if (post == null)
            {
                return null!;
            }

            // Chuyển đổi entity sang ViewModel nếu cần
            return new PostManageViewModel(post);
            
        }

        public async Task<Post> UpdatePostAsync(PostManageViewModel post)
        {
            try
            {
                var existingPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.postId == post.PostId);

                if (existingPost != null)
                {
                    existingPost.title = post.Title;
                    existingPost.address = post.Address;
                    existingPost.description = post.Description;
                    existingPost.price = post.Price;
                    existingPost.status = post.Status;
                    existingPost.imageUrls = post.ImageUrls;
                    existingPost.typeId = post.TypeId;
                    existingPost.updatedDate = DateTime.Now;

                    _dbContext.Entry(existingPost).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                }

                return existingPost!;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ khi cập nhật bị lỗi (ví dụ: ghi log, ném lại ngoại lệ, ...)
                throw new Exception("Cập nhật bài đăng không thành công", ex);
            }
        }


    }
}
