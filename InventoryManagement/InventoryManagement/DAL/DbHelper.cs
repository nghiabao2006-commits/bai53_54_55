using System.Configuration;
using System.Data.SqlClient;

namespace InventoryManagement.DAL;

internal static class DbHelper
{
    public static string ConnectionString =>
        ConfigurationManager.ConnectionStrings["InventoryDB"]?.ConnectionString
        ?? throw new InvalidOperationException("Chưa cấu hình connectionString InventoryDB trong App.config.");

    public static SqlConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}
