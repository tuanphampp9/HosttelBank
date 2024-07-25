using Btl_web_nc.Models;

namespace Btl_web_nc.RepositoryInterfaces
{
    public interface IUserRepositories
    {
        // Xác thực người dùng bằng tên đăng nhập và mật khẩu
        public Task<User> AuthenticateUserAsync(string userName, string password);

        // Lấy thông tin người dùng theo tên đăng nhập
        public Task<User> GetUserByUserNameAsync(string userName);

        // Lấy thông tin người dùng theo ID
        public Task<User> GetUserByIdAsync(int userId);

        // Kiểm tra xem người dùng có tồn tại không
        public Task<bool> UserExistsAsync(int userName);
    }
}
