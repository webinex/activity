using Microsoft.EntityFrameworkCore;

namespace Webinex.Activity.EntityFrameworkCore.Tests.DataAccess;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;

    public void CreateDatabase()
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<User>(user =>
        {
            user.ToTable("Users");
            user.UseActivity(true);
            user.HasKey(x => x.Id);

            user.OwnsOne(x => x.Contact, contact =>
            {
                contact.OwnsOne(
                    x => x.PrimaryPhone,
                    phone => phone.Property(x => x.Value).HasColumnName("Phone"));

                contact.OwnsOne(
                    x => x.AdditionalPhone,
                    phone => phone.Property(x => x.Value).HasColumnName("AdditionalPhone"));
            });

            user.OwnsMany(x => x.Contacts, contacts =>
            {
                contacts.WithOwner().HasForeignKey("UserId");
                contacts.ToTable("User_Contacts");
                contacts.Property<Guid>("Id").ValueGeneratedOnAdd();

                contacts.HasKey("Id", "UserId");

                contacts.OwnsOne(
                    x => x.PrimaryPhone,
                    phone => phone.Property(x => x.Value).HasColumnName("Phone"));

                contacts.OwnsOne(
                    x => x.AdditionalPhone,
                    phone => phone.Property(x => x.Value).HasColumnName("AdditionalPhone"));

            });
        });
        
        base.OnModelCreating(model);
    }
}