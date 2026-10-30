using System.Data;
using System.Data.SqlClient;
using InventoryManagement.DTO;

namespace InventoryManagement.DAL;

public class VatTuDAL
{
    public List<VatTuDTO> Search(string? keyword = null, bool onlyLowStock = false, decimal threshold = 10)
    {
        var sql = @"SELECT MaVatTu, TenVatTu, DVT, SoLuongTon
                    FROM dbo.VatTu
                    WHERE (@Keyword = N'' OR MaVatTu LIKE @Keyword OR TenVatTu LIKE @Keyword)";

        if (onlyLowStock)
        {
            sql += " AND SoLuongTon < @Nguong";
        }

        sql += " ORDER BY TenVatTu";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Keyword", SqlDbType.NVarChar, 152).Value = BuildKeyword(keyword);
        if (onlyLowStock)
        {
            var nguong = command.Parameters.Add("@Nguong", SqlDbType.Decimal);
            nguong.Precision = 18;
            nguong.Scale = 2;
            nguong.Value = threshold;
        }

        var result = new List<VatTuDTO>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(Map(reader));
        }

        return result;
    }

    public VatTuDTO? GetByMa(string maVatTu)
    {
        const string sql = @"SELECT MaVatTu, TenVatTu, DVT, SoLuongTon
                             FROM dbo.VatTu
                             WHERE MaVatTu = @MaVatTu";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaVatTu", SqlDbType.VarChar, 20).Value = maVatTu;

        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public int Insert(VatTuDTO vatTu)
    {
        const string sql = @"INSERT INTO dbo.VatTu (MaVatTu, TenVatTu, DVT, SoLuongTon)
                             VALUES (@MaVatTu, @TenVatTu, @DVT, @SoLuongTon)";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaVatTu", SqlDbType.VarChar, 20).Value = vatTu.MaVatTu;
        command.Parameters.Add("@TenVatTu", SqlDbType.NVarChar, 150).Value = vatTu.TenVatTu;
        command.Parameters.Add("@DVT", SqlDbType.NVarChar, 20).Value = vatTu.DVT;
        AddDecimal(command, "@SoLuongTon", vatTu.SoLuongTon);
        return command.ExecuteNonQuery();
    }

    public int Update(VatTuDTO vatTu)
    {
        const string sql = @"UPDATE dbo.VatTu
                             SET TenVatTu = @TenVatTu,
                                 DVT = @DVT,
                                 SoLuongTon = @SoLuongTon
                             WHERE MaVatTu = @MaVatTu";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@TenVatTu", SqlDbType.NVarChar, 150).Value = vatTu.TenVatTu;
        command.Parameters.Add("@DVT", SqlDbType.NVarChar, 20).Value = vatTu.DVT;
        AddDecimal(command, "@SoLuongTon", vatTu.SoLuongTon);
        command.Parameters.Add("@MaVatTu", SqlDbType.VarChar, 20).Value = vatTu.MaVatTu;
        return command.ExecuteNonQuery();
    }

    public int Delete(string maVatTu)
    {
        const string sql = "DELETE FROM dbo.VatTu WHERE MaVatTu = @MaVatTu";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaVatTu", SqlDbType.VarChar, 20).Value = maVatTu;

        try
        {
            return command.ExecuteNonQuery();
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            throw new InvalidOperationException("Vật tư đã phát sinh phiếu kho nên không thể xóa.");
        }
    }

    public decimal GetTotalInventoryValue()
    {
        const string sql = @"
            SELECT ISNULL(SUM(vt.SoLuongTon * ISNULL(gia.DonGia, 0)), 0)
            FROM dbo.VatTu AS vt
            OUTER APPLY
            (
                SELECT TOP 1 ct.DonGia
                FROM dbo.ChiTietPhieuKho AS ct
                INNER JOIN dbo.PhieuKho AS pk ON pk.MaPhieu = ct.MaPhieu
                WHERE ct.MaVatTu = vt.MaVatTu
                ORDER BY pk.NgayLap DESC, pk.MaPhieu DESC
            ) AS gia";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        return Convert.ToDecimal(command.ExecuteScalar());
    }

    internal static void AddDecimal(SqlCommand command, string name, decimal value)
    {
        var parameter = command.Parameters.Add(name, SqlDbType.Decimal);
        parameter.Precision = 18;
        parameter.Scale = 2;
        parameter.Value = value;
    }

    private static string BuildKeyword(string? keyword)
    {
        var value = keyword?.Trim();
        return string.IsNullOrEmpty(value) ? string.Empty : $"%{value}%";
    }

    private static VatTuDTO Map(SqlDataReader reader) => new()
    {
        MaVatTu = reader.GetString(0),
        TenVatTu = reader.GetString(1),
        DVT = reader.GetString(2),
        SoLuongTon = reader.GetDecimal(3)
    };
}
