using Microsoft.EntityFrameworkCore;
using SearchEngineService.Models;

namespace SearchEngineService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ContentItem> Contents => Set<ContentItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContentItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => new { e.Provider, e.ExternalId })
                .IsUnique()
                .HasDatabaseName("IX_Contents_Provider_ExternalId");
            
            entity.HasIndex(e => e.Title)
                .HasDatabaseName("IX_Contents_Title");
            
            entity.HasIndex(e => e.Type)
                .HasDatabaseName("IX_Contents_Type");
            
            entity.HasIndex(e => e.PublishedAtUtc)
                .HasDatabaseName("IX_Contents_PublishedAtUtc");
            
            entity.HasIndex(e => e.FinalScore)
                .HasDatabaseName("IX_Contents_FinalScore");
            
            entity.HasIndex(e => e.PopularityScore)
                .HasDatabaseName("IX_Contents_PopularityScore");
            
            entity.Property(e => e.ExternalId)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Provider)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.Property(e => e.LastSyncedAtUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
                    });
    }
}
