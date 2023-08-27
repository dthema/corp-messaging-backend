using System.Data;
using Dapper;
using Domain.Repositories;
using Npgsql;

namespace Infrastructure.Repositories;

public abstract class BaseRepository : IBaseRepository
{
    protected const string ConnectionString = "Host=localhost;Username=postgres;Password=root;Database=lab-6-db";

    public BaseRepository()
    { 
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
    }

    protected async Task<int?> InsertAsync<T>(T model, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.InsertAsync(model, transaction);
    }

    protected async Task<int> DeleteAsync<T>(int id, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.DeleteAsync<T>(id, transaction);
    }
    
    protected async Task DeleteAllAsync<T>(IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.DeleteListAsync<T>("WHERE 1 = 1", transaction: transaction);
    }

    protected async Task<T> GetAsync<T>(int id, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.GetAsync<T>(id, transaction);
    }

    protected async Task<IEnumerable<T>> GetListAsync<T>(IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.GetListAsync<T>() ?? new List<T>();
    }

    public async Task<int> UpdateAsync<T>(T model, IDbTransaction? transaction = null)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.UpdateAsync(model, transaction);
    }

    public async Task<IDbTransaction> BeginTransactionAsync()
    {
        var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        return await connection.BeginTransactionAsync();
    }
}