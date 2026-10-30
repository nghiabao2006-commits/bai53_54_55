using System.Data;
using System.Data.SqlClient;
using HotelManagement.DTO;

namespace HotelManagement.DAL;

public class PhongDAL
{
    public DataTable GetAllData()
    {
        const string sql = @"SELECT SoPhong, LoaiPhong, GiaTheoGio, TrangThai
                             FROM dbo.Phong
                             ORDER BY SoPhong";

        using var connection = DbHelper.OpenConnection();
        using var adapter = new SqlDataAdapter(sql, connection);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public List<PhongDTO> GetAll()
    {
        var table = GetAllData();
        var result = new List<PhongDTO>();

        foreach (DataRow row in table.Rows)
        {
            result.Add(new PhongDTO
            {
                SoPhong = row["SoPhong"].ToString() ?? string.Empty,
                LoaiPhong = row["LoaiPhong"].ToString() ?? string.Empty,
                GiaTheoGio = Convert.ToDecimal(row["GiaTheoGio"]),
                TrangThai = row["TrangThai"].ToString() ?? "Trống"
            });
        }

        return result;
    }

    public List<PhongDTO> GetEmptyRooms()
    {
        return GetAll().Where(room => room.TrangThai == "Trống").ToList();
    }

    public PhongDTO? GetBySoPhong(string soPhong)
    {
        const string sql = @"SELECT SoPhong, LoaiPhong, GiaTheoGio, TrangThai
                             FROM dbo.Phong
                             WHERE SoPhong = @SoPhong";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@SoPhong", SqlDbType.VarChar, 10).Value = soPhong;

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new PhongDTO
        {
            SoPhong = reader.GetString(0),
            LoaiPhong = reader.GetString(1),
            GiaTheoGio = reader.GetDecimal(2),
            TrangThai = reader.GetString(3)
        };
    }
}
