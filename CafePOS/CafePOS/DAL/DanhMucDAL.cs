using System.Data;
using System.Data.SqlClient;

namespace CafePOS.DAL;

public class DanhMucDAL
{
    public DataTable GetAll()
    {
        using var connection = DbHelper.OpenConnection();
        using var adapter = new SqlDataAdapter(
            "SELECT MaDanhMuc, TenDanhMuc FROM dbo.DanhMuc ORDER BY TenDanhMuc", connection);

        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public int Insert(string tenDanhMuc)
    {
        const string sql = @"INSERT INTO dbo.DanhMuc (TenDanhMuc)
                             OUTPUT INSERTED.MaDanhMuc
                             VALUES (@TenDanhMuc)";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@TenDanhMuc", SqlDbType.NVarChar, 100).Value = tenDanhMuc;
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int Update(int maDanhMuc, string tenDanhMuc)
    {
        const string sql = @"UPDATE dbo.DanhMuc
                             SET TenDanhMuc = @TenDanhMuc
                             WHERE MaDanhMuc = @MaDanhMuc";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@TenDanhMuc", SqlDbType.NVarChar, 100).Value = tenDanhMuc;
        command.Parameters.Add("@MaDanhMuc", SqlDbType.Int).Value = maDanhMuc;
        return command.ExecuteNonQuery();
    }

    public int Delete(int maDanhMuc)
    {
        const string sql = "DELETE FROM dbo.DanhMuc WHERE MaDanhMuc = @MaDanhMuc";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaDanhMuc", SqlDbType.Int).Value = maDanhMuc;
        return command.ExecuteNonQuery();
    }
}
