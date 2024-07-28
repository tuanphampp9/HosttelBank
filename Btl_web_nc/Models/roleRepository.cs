using Btl_web_nc.RepositoryInterfaces;

namespace Btl_web_nc.Models
{
    public class RoleRepository:IRoleRepositories
    {
        private readonly AppDbContext _dbContext;
        public RoleRepository(AppDbContext db)
        {
            _dbContext = db;
        }
    }
}
