using Microsoft.EntityFrameworkCore;
namespace Btl_web_nc.Models
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<role> Roles { get; set; }
        public DbSet<post> Posts { get; set; }
        public DbSet<favourite> Favourites { get; set; }
        public DbSet<notify> Notifies { get; set; }
        public DbSet<type> Types { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // relationship between 1-n Role and Users
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.roleId)
                .OnDelete(DeleteBehavior.NoAction);

            // relationship between 1-n User and Posts
            modelBuilder.Entity<post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.userId)
                .OnDelete(DeleteBehavior.NoAction);

            // relationship between 1-n User and Favourites
            modelBuilder.Entity<favourite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favourites)
                .HasForeignKey(f => f.userId)
                .OnDelete(DeleteBehavior.NoAction);

            // relationship between 1-n User and Notifies
            modelBuilder.Entity<notify>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifies)
                .HasForeignKey(n => n.userId)
                .OnDelete(DeleteBehavior.NoAction);

            // relationship between 1-n Post and Favourites
            modelBuilder.Entity<favourite>()
                .HasOne(f => f.Post)
                .WithMany(p => p.Favourites)
                .HasForeignKey(f => f.postId)
                .OnDelete(DeleteBehavior.NoAction);

            // relationship between 1-n Post and Notifies
            modelBuilder.Entity<notify>()
                .HasOne(n => n.Post)
                .WithMany(p => p.Notifies)
                .HasForeignKey(n => n.postId)
                .OnDelete(DeleteBehavior.NoAction);

            // relationship between 1-n Type and Post
            modelBuilder.Entity<post>()
                .HasOne(p => p.Type)
                .WithMany(t => t.Posts)
                .HasForeignKey(p => p.typeId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }

    }
}
