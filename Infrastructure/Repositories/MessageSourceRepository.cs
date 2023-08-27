using System.Data;
using System.Data.Common;
using Dapper;
using Domain.Models.MessageSources;
using Domain.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public class MessageSourceRepository : BaseRepository,  IMessageSourceRepository
{
    private const string SourcesTableName = "\"Sources\"";

    public MessageSourceRepository()
    { 
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
    }

    public async Task<int?> InsertMessageSourceAsync<T>(T messageSource, IDbTransaction? transaction = null)
        where T : MessageSource, new() => await InsertAsync(messageSource, transaction);

    public async Task<int> DeleteMessageSourceAsync(int id, IDbTransaction? transaction = null) =>
        await DeleteAsync<MessageSource>(id, transaction);

    public async Task DeleteAllMessageSourcesAsync(IDbTransaction? transaction = null) =>
        await DeleteAllAsync<MessageSource>(transaction);
    
    public async Task<int> UpdateMessageSourceAsync<T>(T messageSource, IDbTransaction? transaction = null) 
        where T : MessageSource, new() => await UpdateAsync(messageSource, transaction);

    public async Task<MessageSource?> GetMessageSourceAsync(int id, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var messageSources = await GetMessageSourcesByQueryAsync(
            connection,
            $"SELECT * FROM {SourcesTableName} WHERE \"Id\" = @Id",
            new { Id = id }, transaction);
        return messageSources.FirstOrDefault(x => x.Id == id);
    }
    
    public async Task<IEnumerable<MessageSource>> GetMessageSourcesByWorkerIdAsync(int workerId, IDbTransaction? transaction = null)
    {
        var workersSourcesRepository = new WorkerSourcesRepository();
        await using var connection = new NpgsqlConnection(ConnectionString);
        var workerSources = await workersSourcesRepository.GetWorkerSourcesByWorkerIdAsync(workerId);
        return await GetMessageSourcesByQueryAsync(connection,
            $"SELECT * FROM {SourcesTableName} WHERE \"Id\" = ANY (@Ids)",
            new { Ids = workerSources?.SourcesIds ?? new int[1] }, transaction);
    }

    public async Task<IEnumerable<MessageSource>> GetAllMessageSourcesAsync(IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await GetMessageSourcesByQueryAsync(connection, $"SELECT * FROM {SourcesTableName}", transaction: transaction);
    }

    private async Task<IEnumerable<MessageSource>> GetMessageSourcesByQueryAsync(DbConnection connection, string query, object? parameters = null, IDbTransaction? transaction = null)
    {
        var sources = new List<MessageSource>();

        using var reader = await connection.ExecuteReaderAsync(query, parameters, transaction);
        var emailParser = reader.GetRowParser<EmailSource>();
        var messengerParser = reader.GetRowParser<MessengerSource>();
        var phoneParser = reader.GetRowParser<PhoneSource>();
    
        var messagesRepository = new MessageRepository();
        while(await reader.ReadAsync())
        {
            var discriminator = (SourceType)reader.GetInt32(reader.GetOrdinal(nameof(SourceType)));
            MessageSource? source = null;
            switch(discriminator)
            {
                case SourceType.Email:
                    source = emailParser(reader);
                    break;
                case SourceType.Messenger:
                    source = messengerParser(reader);
                    break;
                case SourceType.Phone:
                    source = phoneParser(reader);
                    break;
            }

            if (source is null) continue;
            var incomingMessages = await messagesRepository.GetMessagesByRecipientIdAsync(source.Id);
            var outgoingMessages = await messagesRepository.GetMessagesBySenderIdAsync(source.Id);
            source.IncomingMessages = incomingMessages.ToList();
            source.OutgoingMessages = outgoingMessages.ToList();
            sources.Add(source);
        }

        return sources;
    }
}