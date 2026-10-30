using System.Data;
using System.Data.SqlClient;
using InventoryManagement.DTO;

namespace InventoryManagement.DAL;

public class NhaCungCapDAL
{
    public List<NhaCungCapDTO> Search(string? keyword = null)
    {
        const string sql = @"SELECT MaNCC, TenNCC, DienThoai, DiaChi
                             FROM dbo.NhaCungCap
                             WHERE (@Keyword = N'' OR TenNCC LIKE @Keyword OR DienThoai LIKE @Keyword)
                             ORDER BY TenNCC";

        var value = keyword?.Trim();
        var parameterValue = string.IsNullOrEmpty(value) ? string.Empty : $"%{value}%";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Keyword", SqlDbType.NVarChar, 152).Value = parameterValue;

        var result = new List<NhaCungCapDTO>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new NhaCungCapDTO
            {
                MaNCC = reader.GetInt32(0),
                TenNCC = reader.GetString(1),
                DienThoai = reader.IsDBNull(2) ? null : reader.GetString(2),
                DiaChi = reader.IsDBNull(3) ? null : reader.GetString(3)
            });
        }

        return result;
    }

    public int Insert(NhaCungCapDTO nhaCungCap)
    {
        const string sql = @"INSERT INTO dbo.NhaCungCap (TenNCC, DienThoai, DiaChi)
                             VALUES (@TenNCC, @DienThoai, @DiaChi)";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@TenNCC", SqlDbType.NVarChar, 150).Value = nhaCungCap.TenNCC;
        command.Parameters.Add("@DienThoai", SqlDbType.VarChar, 15).Value =
            (object?)nhaCungCap.DienThoai ?? DBNull.Value;
        command.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 255).Value =
            (object?)nhaCungCap.DiaChi ?? DBNull.Value;
        return command.ExecuteNonQuery();
    }

    public int Update(NhaCungCapDTO nhaCungCap)
    {
        const string sql = @"UPDATE dbo.NhaCungCap
                             SET TenNCC = @TenNCC,
                                 DienThoai = @DienThoai,
                                 DiaChi = @DiaChi
                             WHERE MaNCC = @MaNCC";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@TenNCC", SqlDbType.NVarChar, 150).Value = nhaCungCap.TenNCC;
        command.Parameters.Add("@DienThoai", SqlDbType.VarChar, 15).Value =
            (object?)nhaCungCap.DienThoai ?? DBNull.Value;
        command.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 255).Value =
            (object?)nhaCungCap.DiaChi ?? DBNull.Value;
        command.Parameters.Add("@MaNCC", SqlDbType.Int).Value = nhaCungCap.MaNCC;
        return command.ExecuteNonQuery();
    }

    public int Delete(int maNCC)
    {
        const string sql = "DELETE FROM dbo.NhaCungCap WHERE MaNCC = @MaNCC";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaNCC", SqlDbType.Int).Value = maNCC;

        try
        {
            return command.ExecuteNonQuery();
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            throw new InvalidOperationException("Nhà cung cấp đã phát sinh phiếu nhập nên không thể xóa.");
        }
    }
}
