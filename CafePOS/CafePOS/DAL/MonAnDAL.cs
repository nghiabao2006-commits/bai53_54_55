using System.Data;
using System.Data.SqlClient;
using CafePOS.DTO;

namespace CafePOS.DAL;

public class MonAnDAL
{
    public DataTable GetData(int? maDanhMuc = null)
    {
        var sql = @"SELECT m.MaMon, m.TenMon, m.MaDanhMuc, dm.TenDanhMuc, m.DonGia
                    FROM dbo.MonAn AS m
                    INNER JOIN dbo.DanhMuc AS dm ON dm.MaDanhMuc = m.MaDanhMuc";

        if (maDanhMuc.HasValue)
        {
            sql += " WHERE m.MaDanhMuc = @MaDanhMuc";
        }

        sql += " ORDER BY dm.TenDanhMuc, m.TenMon";

        using var connection = DbHelper.OpenConnection();
        using var adapter = new SqlDataAdapter(sql, connection);
        if (maDanhMuc.HasValue)
        {
            adapter.SelectCommand!.Parameters.Add("@MaDanhMuc", SqlDbType.Int).Value = maDanhMuc.Value;
        }

        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public MonAnDTO? GetById(int maMon)
    {
        const string sql = @"SELECT m.MaMon, m.TenMon, m.MaDanhMuc, dm.TenDanhMuc, m.DonGia
                             FROM dbo.MonAn AS m
                             INNER JOIN dbo.DanhMuc AS dm ON dm.MaDanhMuc = m.MaDanhMuc
                             WHERE m.MaMon = @MaMon";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaMon", SqlDbType.Int).Value = maMon;

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new MonAnDTO
        {
            MaMon = reader.GetInt32(0),
            TenMon = reader.GetString(1),
            MaDanhMuc = reader.GetInt32(2),
            TenDanhMuc = reader.GetString(3),
            DonGia = reader.GetDecimal(4)
        };
    }

    public int Insert(MonAnDTO mon)
    {
        const string sql = @"INSERT INTO dbo.MonAn (TenMon, MaDanhMuc, DonGia)
                             OUTPUT INSERTED.MaMon
                             VALUES (@TenMon, @MaDanhMuc, @DonGia)";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@TenMon", SqlDbType.NVarChar, 100).Value = mon.TenMon;
        command.Parameters.Add("@MaDanhMuc", SqlDbType.Int).Value = mon.MaDanhMuc;
        var donGia = command.Parameters.Add("@DonGia", SqlDbType.Decimal);
        donGia.Precision = 18;
        donGia.Scale = 2;
        donGia.Value = mon.DonGia;
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int Update(MonAnDTO mon)
    {
        const string sql = @"UPDATE dbo.MonAn
                             SET TenMon = @TenMon,
                                 MaDanhMuc = @MaDanhMuc,
                                 DonGia = @DonGia
                             WHERE MaMon = @MaMon";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@TenMon", SqlDbType.NVarChar, 100).Value = mon.TenMon;
        command.Parameters.Add("@MaDanhMuc", SqlDbType.Int).Value = mon.MaDanhMuc;
        var donGia = command.Parameters.Add("@DonGia", SqlDbType.Decimal);
        donGia.Precision = 18;
        donGia.Scale = 2;
        donGia.Value = mon.DonGia;
        command.Parameters.Add("@MaMon", SqlDbType.Int).Value = mon.MaMon;
        return command.ExecuteNonQuery();
    }

    public int Delete(int maMon)
    {
        const string sql = "DELETE FROM dbo.MonAn WHERE MaMon = @MaMon";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaMon", SqlDbType.Int).Value = maMon;
        return command.ExecuteNonQuery();
    }
}
