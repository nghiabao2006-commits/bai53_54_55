using System.Data;
using System.Data.SqlClient;
using InventoryManagement.DTO;

namespace InventoryManagement.DAL;

public class PhieuKhoDAL
{
    public List<PhieuKhoDTO> GetAll()
    {
        const string sql = @"SELECT pk.MaPhieu, pk.LoaiPhieu, pk.NgayLap, pk.MaNCC,
                                    ISNULL(ncc.TenNCC, N''), pk.TongTien
                             FROM dbo.PhieuKho AS pk
                             LEFT JOIN dbo.NhaCungCap AS ncc ON ncc.MaNCC = pk.MaNCC
                             ORDER BY pk.NgayLap DESC, pk.MaPhieu DESC";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);

        var result = new List<PhieuKhoDTO>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new PhieuKhoDTO
            {
                MaPhieu = reader.GetInt32(0),
                LoaiPhieu = reader.GetString(1),
                NgayLap = reader.GetDateTime(2),
                MaNCC = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                TenNCC = reader.GetString(4),
                TongTien = reader.GetDecimal(5)
            });
        }

        return result;
    }

    public List<ChiTietPhieuKhoDTO> GetDetails(int maPhieu)
    {
        const string sql = @"SELECT ct.MaPhieu, ct.MaVatTu, vt.TenVatTu, vt.DVT, ct.SoLuong, ct.DonGia
                             FROM dbo.ChiTietPhieuKho AS ct
                             INNER JOIN dbo.VatTu AS vt ON vt.MaVatTu = ct.MaVatTu
                             WHERE ct.MaPhieu = @MaPhieu
                             ORDER BY vt.TenVatTu";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;

        var result = new List<ChiTietPhieuKhoDTO>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new ChiTietPhieuKhoDTO
            {
                MaPhieu = reader.GetInt32(0),
                MaVatTu = reader.GetString(1),
                TenVatTu = reader.GetString(2),
                DVT = reader.GetString(3),
                SoLuong = reader.GetDecimal(4),
                DonGia = reader.GetDecimal(5)
            });
        }

        return result;
    }

    public int Create(PhieuKhoDTO phieu)
    {
        if (phieu.LoaiPhieu is not ("NHAP" or "XUAT"))
        {
            throw new ArgumentException("Loại phiếu chỉ có thể là NHAP hoặc XUAT.");
        }

        if (phieu.ChiTiet.Count == 0)
        {
            throw new ArgumentException("Phiếu kho phải có ít nhất một vật tư.");
        }

        if (phieu.LoaiPhieu == "NHAP" && phieu.MaNCC is null)
        {
            throw new ArgumentException("Phiếu nhập kho bắt buộc phải chọn nhà cung cấp.");
        }

        var tongTien = phieu.ChiTiet.Sum(ct => ct.SoLuong * ct.DonGia);

        using var connection = DbHelper.OpenConnection();
        using var transaction = connection.BeginTransaction();
        try
        {
            var maPhieu = InsertHeader(connection, transaction, phieu, tongTien);
            foreach (var chiTiet in phieu.ChiTiet)
            {
                if (phieu.LoaiPhieu == "XUAT")
                {
                    CheckStock(connection, transaction, chiTiet);
                }

                InsertDetail(connection, transaction, maPhieu, chiTiet);
                UpdateStock(connection, transaction, chiTiet, phieu.LoaiPhieu == "NHAP" ? 1 : -1);
            }

            transaction.Commit();
            return maPhieu;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static int InsertHeader(
        SqlConnection connection,
        SqlTransaction transaction,
        PhieuKhoDTO phieu,
        decimal tongTien)
    {
        const string sql = @"INSERT INTO dbo.PhieuKho (LoaiPhieu, NgayLap, MaNCC, TongTien)
                             OUTPUT INSERTED.MaPhieu
                             VALUES (@LoaiPhieu, @NgayLap, @MaNCC, @TongTien)";

        using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.Add("@LoaiPhieu", SqlDbType.NVarChar, 10).Value = phieu.LoaiPhieu;
        command.Parameters.Add("@NgayLap", SqlDbType.DateTime).Value = phieu.NgayLap;
        command.Parameters.Add("@MaNCC", SqlDbType.Int).Value =
            phieu.LoaiPhieu == "NHAP" ? (object)phieu.MaNCC! : DBNull.Value;
        VatTuDAL.AddDecimal(command, "@TongTien", tongTien);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static void CheckStock(
        SqlConnection connection,
        SqlTransaction transaction,
        ChiTietPhieuKhoDTO chiTiet)
    {
        const string sql = @"SELECT TenVatTu, SoLuongTon
                             FROM dbo.VatTu WITH (UPDLOCK, ROWLOCK)
                             WHERE MaVatTu = @MaVatTu";

        using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.Add("@MaVatTu", SqlDbType.VarChar, 20).Value = chiTiet.MaVatTu;

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            throw new InvalidOperationException($"Không tìm thấy vật tư có mã {chiTiet.MaVatTu}.");
        }

        var tenVatTu = reader.GetString(0);
        var soLuongTon = reader.GetDecimal(1);
        reader.Close();

        if (soLuongTon < chiTiet.SoLuong)
        {
            throw new InvalidOperationException(
                $"Không đủ hàng trong kho: {tenVatTu} chỉ còn {soLuongTon}, cần xuất {chiTiet.SoLuong}.");
        }
    }

    private static void InsertDetail(
        SqlConnection connection,
        SqlTransaction transaction,
        int maPhieu,
        ChiTietPhieuKhoDTO chiTiet)
    {
        const string sql = @"INSERT INTO dbo.ChiTietPhieuKho (MaPhieu, MaVatTu, SoLuong, DonGia)
                             VALUES (@MaPhieu, @MaVatTu, @SoLuong, @DonGia)";

        using var command = new SqlCommand(sql, connection, transaction);
        command.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;
        command.Parameters.Add("@MaVatTu", SqlDbType.VarChar, 20).Value = chiTiet.MaVatTu;
        VatTuDAL.AddDecimal(command, "@SoLuong", chiTiet.SoLuong);
        VatTuDAL.AddDecimal(command, "@DonGia", chiTiet.DonGia);
        command.ExecuteNonQuery();
    }

    private static void UpdateStock(
        SqlConnection connection,
        SqlTransaction transaction,
        ChiTietPhieuKhoDTO chiTiet,
        int sign)
    {
        const string sql = @"UPDATE dbo.VatTu
                             SET SoLuongTon = SoLuongTon + @Delta
                             WHERE MaVatTu = @MaVatTu";

        using var command = new SqlCommand(sql, connection, transaction);
        VatTuDAL.AddDecimal(command, "@Delta", sign * chiTiet.SoLuong);
        command.Parameters.Add("@MaVatTu", SqlDbType.VarChar, 20).Value = chiTiet.MaVatTu;

        if (command.ExecuteNonQuery() == 0)
        {
            throw new InvalidOperationException($"Không tìm thấy vật tư có mã {chiTiet.MaVatTu}.");
        }
    }
}
