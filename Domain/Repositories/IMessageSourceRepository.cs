using System.Data;
using Domain.Models.MessageSources;

namespace Domain.Repositories;

public interface IMessageSourceRepository : IBaseRepository
{ 
    Task<int?> InsertMessageSourceAsync<T>(T messageSource, IDbTransaction? transaction = null) where T : MessageSource, new();
    Task<int> DeleteMessageSourceAsync(int id, IDbTransaction? transaction = null);
    Task DeleteAllMessageSourcesAsync(IDbTransaction? transaction = null);
    Task<int> UpdateMessageSourceAsync<T>(T messageSource, IDbTransaction? transaction = null) where T : MessageSource, new();
    Task<MessageSource?> GetMessageSourceAsync(int id, IDbTransaction? transaction = null);
    Task<IEnumerable<MessageSource>> GetMessageSourcesByWorkerIdAsync(int workerId, IDbTransaction? transaction = null);
    Task<IEnumerable<MessageSource>> GetAllMessageSourcesAsync(IDbTransaction? transaction = null);
}