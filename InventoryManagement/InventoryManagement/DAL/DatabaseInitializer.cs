using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

namespace InventoryManagement.DAL;

internal static class DatabaseInitializer
{
    private static readonly string[] CandidateServers =
    {
        @".\MSSQLSERVER03",
        @".",
        @".\SQLEXPRESS",
        @"(localdb)\MSSQLLocalDB"
    };

    public static string ResolveConnectionString(string databaseName)
    {
        var errors = new List<string>();

        foreach (var candidate in BuildCandidates(databaseName))
        {
            var masterBuilder = new SqlConnectionStringBuilder(candidate)
            {
                InitialCatalog = "master",
                ConnectTimeout = 3
            };

            try
            {
                using var connection = new SqlConnection(masterBuilder.ConnectionString);
                connection.Open();
                ApplySchema(connection);

                var result = new SqlConnectionStringBuilder(candidate)
                {
                    InitialCatalog = databaseName
                };
                return result.ConnectionString;
            }
            catch (Exception ex)
            {
                errors.Add($"{masterBuilder.DataSource}: {ex.Message}");
            }
        }

        throw new InvalidOperationException(
            "Không kết nối được SQL Server nào. Đã thử:\n - " + string.Join("\n - ", errors) +
            "\n\nHãy cài SQL Server (Express/Developer) hoặc LocalDB rồi chạy lại.");
    }

    private static IEnumerable<string> BuildCandidates(string databaseName)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var configured = ConfigurationManager.ConnectionStrings[databaseName]?.ConnectionString;
        if (!string.IsNullOrWhiteSpace(configured))
        {
            string server = string.Empty;
            try
            {
                server = new SqlConnectionStringBuilder(configured).DataSource;
            }
            catch (ArgumentException)
            {
            }

            if (!string.IsNullOrWhiteSpace(server) && seen.Add(server))
            {
                yield return configured;
            }
        }

        foreach (var server in CandidateServers)
        {
            if (!seen.Add(server))
            {
                continue;
            }

            yield return new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = "master",
                IntegratedSecurity = true,
                TrustServerCertificate = true
            }.ConnectionString;
        }
    }

    private static void ApplySchema(SqlConnection connection)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ScriptDB.sql")
            ?? throw new InvalidOperationException("Không tìm thấy script khởi tạo cơ sở dữ liệu.");
        using var reader = new StreamReader(stream, Encoding.UTF8);

        foreach (var batch in SplitBatches(reader.ReadToEnd()))
        {
            using var command = new SqlCommand(batch, connection) { CommandTimeout = 120 };
            command.ExecuteNonQuery();
        }
    }

    private static IEnumerable<string> SplitBatches(string script)
    {
        var batch = new StringBuilder();
        using var reader = new StringReader(script);
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                if (batch.Length > 0)
                {
                    yield return batch.ToString();
                    batch.Clear();
                }
                continue;
            }

            batch.AppendLine(line);
        }

        if (batch.Length > 0)
        {
            yield return batch.ToString();
        }
    }
}
