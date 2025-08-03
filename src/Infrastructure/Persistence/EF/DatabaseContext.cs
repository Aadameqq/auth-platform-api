using Core.Domain;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.EF;

public class DatabaseContext(IOptions<DatabaseOptions> databaseConfig) : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<OAuthConnection> OAuthConnections { get; set; }
    public DbSet<Confirmation> Confirmations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(databaseConfig.Value.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(accountBuilder =>
        {
            accountBuilder.Property<Guid>("Id");
            accountBuilder.Property<bool>("activated").HasColumnName("Activated");

            accountBuilder.Property(u => u.Role)
                .HasConversion(
                    role => role.ToString(),
                    name => Enum.Parse<Role>(name)
                )
                .IsUnicode();

            accountBuilder.OwnsMany<AuthSession>(
                "sessions",
                sessionBuilder =>
                {
                    sessionBuilder.Property<Guid>("Id");
                    sessionBuilder.Property<DateTime>("expiresAt").HasColumnName("ExpiresAt");
                    sessionBuilder.Property<Guid>("AccountId").HasColumnName("AccountId");
                    sessionBuilder.Property<Guid>("currentTokenId").HasColumnName("CurrentTokenId");
                    sessionBuilder.WithOwner().HasForeignKey("AccountId");
                }
            );
        });

        modelBuilder.Entity<OAuthConnection>(builder =>
        {
            builder.Property<Guid>("Id");
            builder
                .HasOne<Account>()
                .WithMany()
                .HasForeignKey("AccountId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property<string>("OAuthId");
            builder.Property<string>("Provider");
        });

        modelBuilder.Entity<Confirmation>(b =>
        {
            b.HasKey(c => c.Id);

            b.Property(c => c.OwnerId).IsRequired();
            b.HasOne<Account>()
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Property(c => c.Action).HasConversion<string>().IsUnicode().IsRequired();

            b.Property(c => c.Method).HasConversion<string>().IsUnicode().IsRequired();

            b.Property(c => c.Code).IsUnicode(false).IsRequired(false);

            b.Property<DateTime>("createdAt").HasColumnName("CreatedAt").IsRequired();
        });
    }
}
