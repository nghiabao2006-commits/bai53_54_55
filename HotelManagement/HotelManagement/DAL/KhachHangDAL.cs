using System.Data;
using System.Data.SqlClient;
using HotelManagement.DTO;

namespace HotelManagement.DAL;

public class KhachHangDAL
{
    public KhachHangDTO? FindByCCCD(string cccd)
    {
        const string sql = @"SELECT MaKH, HoTen, CCCD, SoDienThoai
                             FROM dbo.KhachHang
                             WHERE CCCD = @CCCD";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@CCCD", SqlDbType.VarChar, 20).Value = cccd;

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new KhachHangDTO
        {
            MaKH = reader.GetInt32(0),
            HoTen = reader.GetString(1),
            CCCD = reader.GetString(2),
            SoDienThoai = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
        };
    }
}
