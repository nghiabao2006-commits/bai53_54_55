using System.Data;
using System.Data.SqlClient;
using HotelManagement.DTO;

namespace HotelManagement.DAL;

public class DichVuDAL
{
    public List<DichVuDTO> GetAll()
    {
        const string sql = @"SELECT MaDV, TenDV, DonGia
                             FROM dbo.DichVu
                             ORDER BY TenDV";

        using var connection = DbHelper.OpenConnection();
        using var adapter = new SqlDataAdapter(sql, connection);
        var table = new DataTable();
        adapter.Fill(table);

        var result = new List<DichVuDTO>();
        foreach (DataRow row in table.Rows)
        {
            result.Add(new DichVuDTO
            {
                MaDV = Convert.ToInt32(row["MaDV"]),
                TenDV = row["TenDV"].ToString() ?? string.Empty,
                DonGia = Convert.ToDecimal(row["DonGia"])
            });
        }

        return result;
    }
}
