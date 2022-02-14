using CardsAgainstMySanity.Domain.Auth.Tokens;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        public DbSet<UserDbModel> Users { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserTypeDbModel>(builder =>
            {
                builder.ToTable("user_type");

                builder.HasKey(userType => userType.Id);

                builder.Property(userType => userType.Id)
                    .HasColumnName("id");

                builder.Property(userType => userType.Name)
                    .HasColumnName("name")
                    .HasMaxLength(24)
                    .IsRequired();

                builder.HasData(
                    new UserTypeDbModel
                    {
                        Id = 1,
                        Name = "account"
                    },
                    new UserTypeDbModel
                    {
                        Id = 2,
                        Name = "guest"
                    },
                    new UserTypeDbModel
                    {
                        Id = 3,
                        Name = "admin"
                    }
                );
            });

            modelBuilder.Entity<UserDbModel>(builder =>
            {
                builder.ToTable("user");

                builder.HasKey(user => user.Id);

                builder.Property(user => user.Id)
                    .HasColumnName("id");

                builder.Property(user => user.AvatarUrl)
                    .HasColumnName("avatar_url")
                    .HasMaxLength(255)
                    .IsRequired();

                builder.Property(user => user.Username)
                    .HasColumnName("username")
                    .HasMaxLength(24)
                    .IsRequired();

                builder.Property(user => user.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsRequired(false);

                builder.Property(user => user.PasswordHash)
                    .HasColumnName("password_hash")
                    .HasMaxLength(255)
                    .IsRequired(false);

                builder.Property(user => user.IpAddress)
                    .HasColumnName("ip_address")
                    .HasMaxLength(15)
                    .IsRequired();

                builder.Property(user => user.UserTypeId)
                    .HasColumnName("user_type_id")
                    .IsRequired();

                builder.Property(user => user.AccessToken)
                    .HasColumnName("access_token")
                    .HasColumnType("character varying");

                builder.Property(user => user.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                builder.Property(user => user.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                builder.Property(user => user.LastPong)
                    .HasColumnName("last_pong")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                builder.HasMany(user => user.RefreshTokens)
                    .WithOne()
                    .HasForeignKey(refreshToken => refreshToken.UserId);

                builder.HasOne(user => user.UserType)
                    .WithMany(userType => userType.Users)
                    .HasForeignKey(user => user.UserTypeId);
            });

            modelBuilder.Entity<RefreshToken>(builder =>
            {
                builder.ToTable("refresh_token");

                builder.HasKey(token => token.Token);

                builder.Property(user => user.Token)
                    .HasColumnName("token");

                builder.Property(token => token.ExpiresAt)
                    .HasColumnName("expires_at")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                builder.Property(token => token.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                builder.Property(token => token.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();
            });
        }
    }
}