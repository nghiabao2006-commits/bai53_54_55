using System.Data;
using System.Data.SqlClient;
using CafePOS.DTO;

namespace CafePOS.DAL;

public class BanAnDAL
{
    public DataTable GetAll()
    {
        using var connection = DbHelper.OpenConnection();
        using var adapter = new SqlDataAdapter(
            "SELECT MaBan, TenBan, TrangThai FROM dbo.BanAn ORDER BY MaBan", connection);

        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public List<BanAnDTO> GetAllDto()
    {
        var result = new List<BanAnDTO>();
        var table = GetAll();

        foreach (DataRow row in table.Rows)
        {
            result.Add(new BanAnDTO
            {
                MaBan = Convert.ToInt32(row["MaBan"]),
                TenBan = row["TenBan"].ToString() ?? string.Empty,
                TrangThai = row["TrangThai"].ToString() ?? "Trống"
            });
        }

        return result;
    }

    public BanAnDTO? GetById(int maBan)
    {
        const string sql = @"SELECT MaBan, TenBan, TrangThai
                             FROM dbo.BanAn
                             WHERE MaBan = @MaBan";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaBan", SqlDbType.Int).Value = maBan;

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new BanAnDTO
        {
            MaBan = reader.GetInt32(0),
            TenBan = reader.GetString(1),
            TrangThai = reader.GetString(2)
        };
    }
}
