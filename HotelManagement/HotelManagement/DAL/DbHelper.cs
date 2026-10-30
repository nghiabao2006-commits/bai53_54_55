using System.Configuration;
using System.Data.SqlClient;

namespace HotelManagement.DAL;

internal static class DbHelper
{
    public static string ConnectionString =>
        ConfigurationManager.ConnectionStrings["HotelDB"]?.ConnectionString
        ?? throw new InvalidOperationException("Chưa cấu hình connectionString HotelDB trong App.config.");

    public static SqlConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}
