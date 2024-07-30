using Btl_web_nc.RepositoryInterfaces;

namespace Btl_web_nc.Models
{
    public class NotifyRepository : INotifyRepositories
    {
        private readonly AppDbContext _dbContext;

        public NotifyRepository(AppDbContext db)
        {
            _dbContext = db;
        }
        public Task<Notify> AddNotifyAsync(Notify notify)
        {
            _dbContext.Notifies.Add(notify);
            _dbContext.SaveChanges();
            return Task.FromResult(notify);
        }
    }
}
