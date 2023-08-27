using System.Data;
using System.Data.Common;
using Dapper;
using Domain.Models.Messages;
using Domain.Models.MessageSources;
using Domain.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public class MessageRepository : BaseRepository, IMessageRepository
{
    private const string MessagesTableName = "\"Messages\"";

    public MessageRepository()
    { 
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
    }

    public async Task<int?> InsertMessageAsync<T>(T message, IDbTransaction? transaction = null) where T : Message => await InsertAsync(message, transaction);

    public async Task DeleteMessageAsync(int id, IDbTransaction? transaction = null) => await DeleteAsync<Message>(id, transaction);

    public async Task DeleteAllMessagesAsync(IDbTransaction? transaction = null) => await DeleteAllAsync<Message>(transaction);

    public async Task<int> UpdateMessageAsync<T>(T message, IDbTransaction? transaction = null) where T : Message => await UpdateAsync(message, transaction);

    public async Task<Message?> GetMessageAsync(int id, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var messages = await GetMessagesByQueryAsync(
            connection,
            $"SELECT * FROM {MessagesTableName} WHERE \"Id\" = @Id",
            new {Id = id}, transaction);
        return messages.FirstOrDefault(x => x.Id == id);
    }

    public async Task<IEnumerable<Message>> GetAllMessagesAsync(IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await GetMessagesByQueryAsync(connection, $"SELECT * FROM {MessagesTableName}", transaction: transaction);
    }

    public async Task<IEnumerable<Message>> GetMessagesBySenderIdAsync(int senderSourceId, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await GetMessagesByQueryAsync(
            connection,
            $"SELECT * FROM {MessagesTableName} WHERE \"SenderSourceId\" = @Id",
            new { Id = senderSourceId }, transaction);
    }

    public async Task<IEnumerable<Message>> GetMessagesByRecipientIdAsync(int recipientSourceId, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await GetMessagesByQueryAsync(
            connection,
            $"SELECT * FROM {MessagesTableName} WHERE \"RecipientSourceId\" = @Id",
            new { Id = recipientSourceId }, transaction);
    }
    
    private async Task<IEnumerable<Message>> GetMessagesByQueryAsync(DbConnection connection, string query, object? parameters = null, IDbTransaction? transaction = null)
    {
        var messages = new List<Message>();

        using var reader = await connection.ExecuteReaderAsync(query, parameters, transaction);
        var emailParser = reader.GetRowParser<EmailMessage>();
        var messengerParser = reader.GetRowParser<MessengerMessage>();
        var phoneParser = reader.GetRowParser<PhoneMessage>();
    
        while(reader.Read())
        {
            var discriminator = (SourceType)reader.GetInt32(reader.GetOrdinal(nameof(SourceType)));
            switch(discriminator)
            {
                case SourceType.Email:
                    messages.Add(emailParser(reader));
                    break;
                case SourceType.Messenger:
                    messages.Add(messengerParser(reader));
                    break;
                case SourceType.Phone:
                    messages.Add(phoneParser(reader));
                    break;
            }
        }

        return messages;
    }
}