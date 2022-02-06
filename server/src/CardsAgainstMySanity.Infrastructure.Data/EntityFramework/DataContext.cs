using CardsAgainstMySanity.Domain.Auth;
using CardsAgainstMySanity.Domain.Auth.Tokens;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstMySanity.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Guest> Guests { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Guest>(builder =>
            {
                builder.HasKey(guest => guest.Id);

                builder.Property(guest => guest.Id)
                    .HasColumnName("id");

                builder.Property(guest => guest.Username)
                    .HasColumnName("username")
                    .HasMaxLength(24)
                    .IsRequired();

                builder.Property(guest => guest.IpAddress)
                    .HasColumnName("ip_address")
                    .HasMaxLength(15)
                    .IsRequired();

                builder.Property(guest => guest.AccessToken)
                    .HasColumnName("access_token")
                    .HasColumnType("character varying");

                builder.Property(guest => guest.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                builder.Property(guest => guest.LastPong)
                    .HasColumnName("last_pong")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                builder.HasMany(guest => guest.RefreshTokens)
                    .WithOne()
                    .HasForeignKey(refreshToken => refreshToken.UserId);
            });

            modelBuilder.Entity<RefreshToken>(builder =>
            {
                builder.HasKey(token => token.Token);

                builder.Property(guest => guest.Token)
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