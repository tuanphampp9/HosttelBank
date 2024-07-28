using Btl_web_nc.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;


namespace Btl_web_nc.Models
{
    public class UserRepository : IUserRepositories
    {

        private readonly AppDbContext _dbContext;
        public UserRepository(AppDbContext db)
        {
            _dbContext = db;
        }

        public async Task<User> AuthenticateUserAsync(string userName, string password)
        {

            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.username == userName );

            if (user == null)
            {
                // Tên đăng nhập không tồn tại
                return null!;
            }

            // So sánh mật khẩu, giả sử mật khẩu đã được mã hóa và lưu trữ
            bool isPasswordValid = VerifyPassword(password, user.password!);

            if (isPasswordValid)
            {
                return user;
            }

            // Mật khẩu không hợp lệ
            return null!;
        }
        private bool VerifyPassword(string password, string hashedPassword)
        {
            if(password == hashedPassword)
                return true;
            return false;
        }

        public User GetUserById(long userId)
        {
            var user =  _dbContext.Users.SingleOrDefault(u => u.userId.Equals(userId));
            return user ?? new User();
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.username == userName);
            return user ?? new User();
        }

        public async Task<bool> UserExistsAsync(int UserId)
        {
            return await _dbContext.Users.AnyAsync(u => u.userId.Equals( UserId));
        }
    }
}
