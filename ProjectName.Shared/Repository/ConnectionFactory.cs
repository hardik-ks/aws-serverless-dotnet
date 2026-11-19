using ProjectName.Shared.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace ProjectName.Shared.Repository;

/// <summary>
/// Creates database connections.
/// </summary>
public interface IConnectionFactory
{
	IDbConnection CreateConnection();
}

/// <summary>
/// Creates MySQL database connections using strongly typed database settings.
/// </summary>
public class ConnectionFactory : IConnectionFactory
{
	/// <summary>
	/// The connection string used to access the database.
	/// </summary>
	private readonly string _connectionString;

	/// <summary>
	/// Initializes a new connection factory using the specified database settings.
	/// </summary>
	/// <param name="dbSettings">The database settings used to build the connection string.</param>
	public ConnectionFactory(DatabaseSettings dbSettings)
	{
		// Build connection string from strongly-typed settings
        _connectionString = dbSettings.GetConnectionString();
    }

	/// <summary>
	/// Creates a MySQL database connection.
	/// </summary>
	/// <returns>A new MySQL database connection.</returns>
	public IDbConnection CreateConnection()
	{
		return new MySqlConnection(_connectionString);
	}
}

