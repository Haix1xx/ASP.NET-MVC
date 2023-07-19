using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC.Models.Blog;
using MVC.Models.Contact;

namespace MVC.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName() ?? "";
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });

            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.HasKey(p => new {p.PostId, p.CategoryId});
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(p => p.Slug).IsUnique();
            });

        }

        public virtual DbSet<Contact.Contact> Contacts { get; set; } = default!;
        public virtual DbSet<Blog.Category> Categories { get; set; } = default!;

        public virtual DbSet<Blog.Post> Posts { get; set; } = default!; 
        public virtual DbSet<Blog.PostCategory> PostCategories { get; set; } = default!;
    }
}
