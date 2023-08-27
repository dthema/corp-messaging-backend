using System.Data;
using Domain.Models.Messages;
using Npgsql;

namespace Domain.Repositories;

public interface IMessageRepository : IBaseRepository
{
    Task<int?> InsertMessageAsync<T>(T message, IDbTransaction? transaction = null) where T : Message;
    Task DeleteMessageAsync(int id, IDbTransaction? transaction = null);
    Task DeleteAllMessagesAsync(IDbTransaction? transaction = null);
    Task<int> UpdateMessageAsync<T>(T message, IDbTransaction? transaction = null)  where T : Message;
    Task<Message?> GetMessageAsync(int id, IDbTransaction? transaction = null);
    Task<IEnumerable<Message>> GetAllMessagesAsync(IDbTransaction? transaction = null);
    Task<IEnumerable<Message>> GetMessagesBySenderIdAsync(int senderSourceId, IDbTransaction? transaction = null);
    Task<IEnumerable<Message>> GetMessagesByRecipientIdAsync(int recipientSourceId, IDbTransaction? transaction = null);
}