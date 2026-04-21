using Microsoft.EntityFrameworkCore;
using Technical_Challenge.Domain;
using Technical_Challenge.Domain.Entities;

namespace Technical_Challenge.Infrastructure.Persistence.Context;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<CurrencyQuote> Quotes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CurrencyQuote>(entity =>
        {
            entity.ToTable("quotes");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Currency).HasColumnName("currency").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Bid).HasColumnName("bid").HasColumnType("numeric(18,6)").IsRequired();
            entity.Property(e => e.Ask).HasColumnName("ask").HasColumnType("numeric(18,6)").IsRequired();
            entity.Property(e => e.High).HasColumnName("high").HasColumnType("numeric(18,6)");
            entity.Property(e => e.Low).HasColumnName("low").HasColumnType("numeric(18,6)");
            entity.Property(e => e.CapturedAt)
                  .HasColumnName("captured_at")
                  .HasColumnType("timestamptz")
                  .IsRequired()
                  .HasDefaultValueSql("NOW()");

            entity.HasIndex(e => e.Currency).HasDatabaseName("idx_quotes_currency");
            entity.HasIndex(e => e.CapturedAt).HasDatabaseName("idx_quotes_captured_at").IsDescending();
        });
    }
}