using System.Configuration;
using System.Data.SqlClient;

namespace CafePOS.DAL;

internal static class DbHelper
{
    public static string ConnectionString =>
        ConfigurationManager.ConnectionStrings["CafePOSDB"]?.ConnectionString
        ?? throw new InvalidOperationException("Chưa cấu hình connectionString CafePOSDB trong App.config.");

    public static SqlConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}
