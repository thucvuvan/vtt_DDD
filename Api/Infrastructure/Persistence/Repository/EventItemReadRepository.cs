using Application.Abstractions;
using Dapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Infrastructure.Persistence.Repository;

public class EventItemReadRepository : IEventItemReadRepository
{
    private const string SelectAll = """
        SELECT
          id AS Id,
          ev_it_title AS EvItTitle,
          ev_it_subtitle AS EvItSubtitle,
          ev_it_description AS EvItDescription,
          ev_it_initial_stock AS EvItInitialStock,
          ev_it_available_stock AS EvItAvailableStock,
          ev_it_is_stock_prepared AS EvItIsStockPrepared,
          ev_it_original_price AS EvItOriginalPrice,
          ev_it_flash_price AS EvItFlashPrice,
          ev_it_start_time AS EvItStartTime,
          ev_it_end_time AS EvItEndTime,
          ev_it_rules AS EvItRules,
          ev_it_status AS EvItStatus,
          ev_it_activity_id AS EvItActivityId,
          ev_it_updated_at AS EvItUpdatedAt,
          ev_it_created_at AS EvItCreatedAt
        FROM event_item
        """;

    private readonly string _connectionString;

    public EventItemReadRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Thiếu ConnectionStrings:Default.");
    }

    public async Task<IReadOnlyList<EventItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        var rows = await connection.QueryAsync<EventItem>(
            new CommandDefinition(SelectAll, cancellationToken: cancellationToken));
        return rows.ToList();
    }
}
