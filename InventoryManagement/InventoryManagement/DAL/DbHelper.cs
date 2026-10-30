using System.Data.SqlClient;

namespace InventoryManagement.DAL;

internal static class DbHelper
{
    private static string? _connectionString;

    public static string ConnectionString =>
        _connectionString
        ?? throw new InvalidOperationException("Chưa khởi tạo kết nối cơ sở dữ liệu.");

    public static void Initialize()
    {
        _connectionString = DatabaseInitializer.ResolveConnectionString("InventoryDB");
    }

    public static SqlConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}
