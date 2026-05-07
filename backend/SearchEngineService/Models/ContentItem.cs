using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchEngineService.Models;

/// <summary>
/// Content item entity with scoring and metadata
/// </summary>
[Table("Contents")]
public class ContentItem
{
    /// <summary>
    /// Unique identifier for the content item
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// External identifier from the provider
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string ExternalId { get; set; }
    
    /// <summary>
    /// Provider name (e.g., "provider-json", "provider-xml")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string Provider { get; set; }
    
    /// <summary>
    /// Content title
    /// </summary>
    [Required]
    [MaxLength(500)]
    public required string Title { get; set; }
    
    /// <summary>
    /// Content type (Video or Text)
    /// </summary>
    public ContentType Type { get; set; }
    
    /// <summary>
    /// Number of views
    /// </summary>
    public double Views { get; set; }
    
    /// <summary>
    /// Number of likes
    /// </summary>
    public double Likes { get; set; }
    
    /// <summary>
    /// Number of reactions
    /// </summary>
    public double Reactions { get; set; }
    
    /// <summary>
    /// Reading time in minutes
    /// </summary>
    public double ReadingTime { get; set; }
    
    /// <summary>
    /// Popularity score calculated from views and likes
    /// </summary>
    public double PopularityScore { get; set; }
    
    /// <summary>
    /// Relevance score based on content matching
    /// </summary>
    public double RelevanceScore { get; set; }
    
    /// <summary>
    /// Final calculated score for ranking
    /// </summary>
    public double FinalScore { get; set; }
    
    /// <summary>
    /// Publication date in UTC
    /// </summary>
    public DateTime PublishedAtUtc { get; set; }
    
    /// <summary>
    /// Creation date in UTC
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last synchronization date in UTC
    /// </summary>
    public DateTime LastSyncedAtUtc { get; set; } = DateTime.UtcNow;
    
    }
