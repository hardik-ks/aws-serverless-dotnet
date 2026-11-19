using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using ProjectName.Shared.Helpers;
using static Dapper.SqlMapper;

namespace ProjectName.Shared.Repository;

/// <summary>
/// Defines operations for retrieving, inserting, updating, and deleting models.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TKey">The type of the model identifier.</typeparam>
public interface IRepositoryBase<TModel, TKey> where TModel : class
{
    TModel Get(TKey id);
    Task<TModel> GetAsync(TKey id);
    IEnumerable<TModel> GetAll();
    Task<IEnumerable<TModel>> GetAllAsync();
    IEnumerable<TModel> Get<TCustomKey>(IEnumerable<TCustomKey> ids, IEnumerable<string>? getTargetFields = null);
    IEnumerable<TModel> Get<TCustomKey>(IEnumerable<TCustomKey> ids, string customKeyField, IEnumerable<string>? getTargetFields = null);
    Task<IEnumerable<TModel>> GetAsync<TCustomKey>(IEnumerable<TCustomKey> ids, IEnumerable<string>? getTargetFields = null);
    Task<IEnumerable<TModel>> GetAsync<TCustomKey>(IEnumerable<TCustomKey> ids, string customKeyField, IEnumerable<string>? getTargetFields = null);
    long Insert(TModel obj);
    long InsertMany(IEnumerable<TModel> obj);
    Task<long> InsertAsync(TModel obj);
    /// <inheritdoc cref="IRepositoryBase{TModel}.InsertMany(IEnumerable{TModel})"/>
    Task<long> InsertManyAsync(IEnumerable<TModel> obj);
    bool Update(TModel obj);
    Task<bool> UpdateAsync(TModel obj);
    Task<bool> DeleteAsync(TModel obj);
    bool Delete(TModel obj);
    int Delete(IEnumerable<TKey> ids);
    Task<int> DeleteAsync(IEnumerable<TKey> ids);
}

/// <summary>
/// Provides base repository operations for retrieving, inserting, updating, and deleting data for a mapped model.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TKey">The primary key type.</typeparam>
public abstract class RepositoryBase<TModel, TKey> : IRepositoryBase<TModel, TKey> where TModel : class
{
    /// <summary>
    /// The factory used to create database connections.
    /// </summary>
    protected readonly IConnectionFactory connectionFactory;

    /// <summary>
    /// Initializes a new instance of the repository base class.
    /// </summary>
    /// <param name="connectionFactory">The factory used to create and manage database connections.</param>
    protected RepositoryBase(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Retrieves a model using the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the model to retrieve.</param>
    /// <returns>The model associated with the specified identifier.</returns>
    public virtual TModel Get(TKey id)
    {
        using var db = connectionFactory.CreateConnection();
        return db.Get<TModel>(id);
    }

    /// <summary>
    /// Asynchronously retrieves a model by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the model to retrieve.</param>
    /// <returns>The model that matches the specified identifier.</returns>
    public virtual async Task<TModel> GetAsync(TKey id)
    {
        using var db = connectionFactory.CreateConnection();
        return await db.GetAsync<TModel>(id);
    }

    /// <summary>
    /// Retrieves all models.
    /// </summary>
    /// <returns>An enumerable containing all models.</returns>
    public virtual IEnumerable<TModel> GetAll()
    {
        using var db = connectionFactory.CreateConnection();
        return db.GetAll<TModel>();
    }

    /// <summary>
    /// Retrieves all models.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains all models.</returns>
    public virtual async Task<IEnumerable<TModel>> GetAllAsync()
    {
        using var db = connectionFactory.CreateConnection();
        return await db.GetAllAsync<TModel>();
    }

    /// <summary>
    /// Inserts the specified model.
    /// </summary>
    /// <param name="obj">The model to insert.</param>
    /// <returns>The identifier of the inserted record.</returns>
    public virtual long Insert(TModel obj)
    {
        using var db = connectionFactory.CreateConnection();
        return db.Insert(obj);
    }

    /// <summary>
    /// Inserts multiple models into the database.
    /// </summary>
    /// <param name="obj">The models to insert.</param>
    /// <returns>The number of rows inserted.</returns>
    public virtual long InsertMany(IEnumerable<TModel> obj)
    {
        using var db = connectionFactory.CreateConnection();
        return db.Insert(obj);
    }

    /// <summary>
    /// Inserts the specified model into the database.
    /// </summary>
    /// <param name="obj">The model to insert.</param>
    /// <returns>The identifier of the inserted record.</returns>
    public virtual async Task<long> InsertAsync(TModel obj)
    {
        using var db = connectionFactory.CreateConnection();
        return await db.InsertAsync(obj);
    }

    /// <summary>
    /// Inserts multiple models into the data store asynchronously.
    /// </summary>
    /// <param name="obj">The models to insert.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of inserted records.</returns>
    public virtual async Task<long> InsertManyAsync(IEnumerable<TModel> obj)
    {
        using var db = connectionFactory.CreateConnection();
        return await db.InsertAsync(obj);
    }

    /// <summary>
    /// Updates the specified model in the data store.
    /// </summary>
    /// <param name="obj">The model to update.</param>
    /// <returns>true if the update succeeds; otherwise, false.</returns>
    public virtual bool Update(TModel obj)
    {
        using var db = connectionFactory.CreateConnection();
        return db.Update(obj);
    }

    /// <summary>
    /// Updates the specified model asynchronously.
    /// </summary>
    /// <param name="obj">The model to update.</param>
    /// <returns>A task whose result is <see langword="true"/> when the model was updated; otherwise, <see langword="false"/>.</returns>
    public virtual async Task<bool> UpdateAsync(TModel obj)
    {
        using var db = connectionFactory.CreateConnection();
        return await db.UpdateAsync(obj);
    }

    /// <summary>
    /// Deletes the specified model asynchronously.
    /// </summary>
    /// <param name="obj">The model to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation. The task result is <see langword="true"/> when the
    /// model is deleted successfully; otherwise, <see langword="false"/>.</returns>
    public virtual async Task<bool> DeleteAsync(TModel obj)
    {
        using var db = connectionFactory.CreateConnection();
        return await db.DeleteAsync(obj);
    }

    /// <summary>
    /// Deletes the specified model from the database.
    /// </summary>
    /// <param name="obj">The model to delete.</param>
    /// <returns><see langword="true"/> if the model is deleted successfully; otherwise, <see langword="false"/>.</returns>
    public virtual bool Delete(TModel obj)
    {
        using var db = connectionFactory.CreateConnection();
        return db.Delete(obj);
    }

    /// <summary>
    /// Deletes records matching the specified keys.
    /// </summary>
    /// <param name="ids">The keys of the records to delete.</param>
    /// <returns>The number of records deleted.</returns>
    public virtual int Delete(IEnumerable<TKey> ids)
    {
        using var db = connectionFactory.CreateConnection();
        if (ids?.Any() != true) return 0;

        var modelType = typeof(TModel);
        var (table, key) = GetDb(modelType);

        return db.Execute(
                $@"DELETE
                FROM {table}
                WHERE {key} IN  @ids",
                new { ids }
            );
    }

    /// <summary>
    /// Deletes records matching the specified identifiers.
    /// </summary>
    /// <remarks>Returns zero when no identifiers are supplied.</remarks>
    /// <param name="ids">The identifiers of the records to delete.</param>
    /// <returns>The number of records deleted.</returns>
    public virtual async Task<int> DeleteAsync(IEnumerable<TKey> ids)
    {
        using var db = connectionFactory.CreateConnection();
        if (ids?.Any() != true) return 0;

        var modelType = typeof(TModel);
        var (table, key) = GetDb(modelType);

        return await db.ExecuteAsync(
                $@"DELETE
                FROM {table}
                WHERE {key} IN @ids",
                new { ids }
            );
    }

    /// <summary>
    /// Gets models by their custom keys.
    /// </summary>
    /// <typeparam name="TCustomKey">The type of the custom keys.</typeparam>
    /// <param name="ids">The custom keys identifying the models to retrieve.</param>
    /// <param name="getTargetFields">The fields to include in the retrieved models.</param>
    /// <returns>The retrieved models.</returns>
    public IEnumerable<TModel> Get<TCustomKey>(IEnumerable<TCustomKey> ids, IEnumerable<string>? getTargetFields = null)
        => this.Get(ids, string.Empty, getTargetFields);

    /// <summary>
    /// Retrieves models whose custom-key values match the specified identifiers.
    /// </summary>
    /// <remarks>When customKeyField is empty or whitespace, the model's configured database key is
    /// used.</remarks>
    /// <typeparam name="TCustomKey">The type of the custom-key values.</typeparam>
    /// <param name="ids">The custom-key values used to filter the records.</param>
    /// <param name="customKeyField">The field name containing the custom key.</param>
    /// <param name="getTargetFields">The fields to select; all fields are selected when omitted or empty.</param>
    /// <returns>The matching models, or an empty collection when no identifiers are supplied.</returns>
    public IEnumerable<TModel> Get<TCustomKey>(IEnumerable<TCustomKey> ids, string customKeyField, IEnumerable<string>? getTargetFields = null)
    {
        if (ids?.Any() != true) return new List<TModel>();

        var modelType = typeof(TModel);
        var (table, key) = GetDb(modelType);
        if (!String.IsNullOrWhiteSpace(customKeyField))
            key = customKeyField;

        using (var db = connectionFactory.CreateConnection())
        {
            // MySQL doesn't support table-valued parameters, so we use direct IN clause
            // Dapper will handle the array expansion for MySQL
            var result = db.Query<TModel>(
                $@"SELECT 
                    {(getTargetFields?.Any() == true ? getTargetFields.ToCSV() : "*")} 
                FROM {table}
                WHERE {key} IN @ids",
                new { ids }
            );
            return result.ToList();
        }
    }

    /// <summary>
    /// Retrieves models matching the specified custom keys.
    /// </summary>
    /// <typeparam name="TCustomKey">The type of the custom keys.</typeparam>
    /// <param name="ids">The custom keys identifying the models to retrieve.</param>
    /// <param name="getTargetFields">The fields to retrieve; when omitted, all fields are retrieved.</param>
    /// <returns>A task whose result contains the matching models.</returns>
    public Task<IEnumerable<TModel>> GetAsync<TCustomKey>(IEnumerable<TCustomKey> ids, IEnumerable<string>? getTargetFields = null)
        => this.GetAsync(ids, string.Empty, getTargetFields);

    public async Task<IEnumerable<TModel>> GetAsync<TCustomKey>(IEnumerable<TCustomKey> ids, string customKeyField, IEnumerable<string>? getTargetFields = null)
    {
        if (ids?.Any() != true) return new List<TModel>();

        var modelType = typeof(TModel);
        var (table, key) = GetDb(modelType);
        if (!String.IsNullOrWhiteSpace(customKeyField))
            key = customKeyField;

        using (var db = connectionFactory.CreateConnection())
        {
            // MySQL doesn't support table-valued parameters, so we use direct IN clause
            // Dapper will handle the array expansion for MySQL
            var result = await db.QueryAsync<TModel>(
                $@"SELECT 
                    {(getTargetFields?.Any() == true ? getTargetFields.ToCSV() : "*")} 
                FROM {table}
                WHERE {key} IN @ids",
                new { ids }
            );

            return result.ToList();
        }
    }

    /// <summary>
    /// Attempts to create a custom SQL list parameter from the specified collection.
    /// </summary>
    /// <remarks>Supports collections of integers, strings, and GUIDs.</remarks>
    /// <typeparam name="TCustomKey">The element type of the collection.</typeparam>
    /// <param name="keys">The collection of keys to convert into a SQL list parameter.</param>
    /// <param name="listVariable">When this method returns, contains the custom query parameter if the collection type is supported; otherwise,
    /// <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if a custom SQL list parameter was created; otherwise, <see langword="false"/>.</returns>
    public bool TryGetCustomListVariable<TCustomKey>(IEnumerable<TCustomKey> keys, out ICustomQueryParameter listVariable)
    {
        var keyType = typeof(TCustomKey);
        // to avoid dapper maximum list item count issue
        ICustomQueryParameter? customQueryParameter = keys switch
        {
            IEnumerable<int> _iv => _iv?.ToSqlList(),
            IEnumerable<string> _sv => _sv?.ToSqlList(),
            IEnumerable<Guid> _gv => _gv.Select(o => o.ToString())?.ToSqlList(),
            // may add more types as we need
            _ => null
        };
        listVariable = customQueryParameter;
        return listVariable != null;

    }

    /// <summary>
    /// Gets the database table name and key field name associated with a model type.
    /// </summary>
    /// <param name="modelType">The model type whose table and key mappings are retrieved.</param>
    /// <returns>A tuple containing the mapped table name and key field name.</returns>
    /// <exception cref="Exception">Thrown when the model type has no table mapping or no key field mapping.</exception>
    public virtual (string tableName, string keyName) GetDb(Type modelType)
    {
        var props = modelType.GetProperties();
        var mappedTableAttr = modelType.GetCustomAttributes(typeof(TableAttribute), true)?.OfType<TableAttribute>()?.First();
        if (mappedTableAttr == null) throw new Exception("No explicit table is associated with the model definition. Use Table attribute to set the mapped table.");

        var keyFieldAttr = props.FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(KeyAttribute)) || Attribute.IsDefined(prop, typeof(ExplicitKeyAttribute)));
        if (keyFieldAttr == null) throw new Exception("No explicit key field is declared at the model definition. Use Key attribute to set the key field.");

        return (mappedTableAttr.Name, keyFieldAttr.Name);
    }
}
