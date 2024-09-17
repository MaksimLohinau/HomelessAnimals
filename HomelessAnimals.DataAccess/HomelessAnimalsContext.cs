using HomelessAnimals.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Numerics;


namespace HomelessAnimals.DataAccess
{
    public class HomelessAnimalsContext : DbContext
    {

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<RoleAssignment> RoleAssignments { get; set; }
        public DbSet<Scope> Scopes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<SignUpRequest> SignUpRequests { get; set; }

        public HomelessAnimalsContext(DbContextOptions<HomelessAnimalsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
            .HasKey(x => x.VolunteerId);

            modelBuilder.Entity<Account>()
                .HasMany<Role>()
                .WithMany()
                .UsingEntity<RoleAssignment>(j => j.ToTable("RoleAssignments"));

            modelBuilder.Entity<RoleAssignment>()
                .HasMany(ra => ra.Scopes)
                .WithOne()
                .HasForeignKey(s => s.RoleAssignmentId);

            modelBuilder.Entity<Role>()
                .HasMany(x => x.Permissions)
                .WithMany()
                .UsingEntity("RolePermissions",
                    j =>
                    {
                        j.Property("PermissionsId").HasColumnName("PermissionId");
                    });

            modelBuilder.Entity<Scope>()
                .Property(x => x.Level)
                .HasConversion<string>();

            modelBuilder.Entity<PasswordResetToken>()
               .HasKey(x => new { x.AccountId, x.Token });

            modelBuilder.Entity<City>()
                .HasMany<Volunteer>()
                .WithOne(x => x.City);

            modelBuilder.Entity<Volunteer>()
                .HasOne(x => x.Account)
                .WithOne(x => x.Volunteer)
                .HasForeignKey<Account>(x => x.VolunteerId);

            modelBuilder.Entity<Animal>()
                .HasOne<Volunteer>()
                .WithMany(x => x.Animals)
                .HasForeignKey(x => x.VolunteerId);

            modelBuilder.Entity<Volunteer>()
                .HasMany<Animal>()
                .WithOne(x => x.Volunteer)
                .HasForeignKey(x => x.VolunteerId);

            modelBuilder.Entity<SignUpRequest>()
                .Property(x => x.Status)
                .HasConversion<string>();


            modelBuilder.Entity<City>()
                .HasMany<SignUpRequest>()
                .WithOne(x => x.City);
        }
    }
}
