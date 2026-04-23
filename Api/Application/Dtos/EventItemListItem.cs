namespace Application.Dtos;

/// <summary>Read model (DTO) trả về tầng ứng dụng / API, tách với entity miền.</summary>
public sealed class EventItemListItem
{
    public long Id { get; set; }
    public string EvItTitle { get; set; } = string.Empty;
    public string? EvItSubtitle { get; set; }
    public string? EvItDescription { get; set; }
    public int EvItInitialStock { get; set; }
    public int EvItAvailableStock { get; set; }
    public int EvItIsStockPrepared { get; set; }
    public long EvItOriginalPrice { get; set; }
    public long EvItFlashPrice { get; set; }
    public DateTime EvItStartTime { get; set; }
    public DateTime EvItEndTime { get; set; }
    public string? EvItRules { get; set; }
    public int EvItStatus { get; set; }
    public long EvItActivityId { get; set; }
    public DateTime EvItUpdatedAt { get; set; }
    public DateTime EvItCreatedAt { get; set; }
}
