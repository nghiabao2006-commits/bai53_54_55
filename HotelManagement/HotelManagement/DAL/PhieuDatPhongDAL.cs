using System.Data;
using System.Data.SqlClient;
using HotelManagement.DTO;

namespace HotelManagement.DAL;

public class PhieuDatPhongDAL
{
    public int CheckIn(string soPhong, string hoTen, string cccd, string soDienThoai)
    {
        using var connection = DbHelper.OpenConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string roomSql = @"SELECT TrangThai
                                     FROM dbo.Phong WITH (UPDLOCK, HOLDLOCK)
                                     WHERE SoPhong = @SoPhong";

            string? trangThaiPhong;
            using (var command = new SqlCommand(roomSql, connection, transaction))
            {
                command.Parameters.Add("@SoPhong", SqlDbType.VarChar, 10).Value = soPhong;
                var value = command.ExecuteScalar();
                if (value is null)
                {
                    throw new InvalidOperationException("Phòng không tồn tại.");
                }

                trangThaiPhong = value.ToString();
            }

            if (trangThaiPhong != "Trống")
            {
                throw new InvalidOperationException($"Phòng {soPhong} hiện không còn trống.");
            }

            int maKH;
            const string findCustomerSql = "SELECT MaKH FROM dbo.KhachHang WHERE CCCD = @CCCD";

            using (var command = new SqlCommand(findCustomerSql, connection, transaction))
            {
                command.Parameters.Add("@CCCD", SqlDbType.VarChar, 20).Value = cccd;
                var existing = command.ExecuteScalar();
                if (existing is not null)
                {
                    maKH = Convert.ToInt32(existing);
                }
                else
                {
                    const string insertCustomerSql = @"INSERT INTO dbo.KhachHang (HoTen, CCCD, SoDienThoai)
                                                       OUTPUT INSERTED.MaKH
                                                       VALUES (@HoTen, @CCCD, @SoDienThoai)";

                    using var insertCommand = new SqlCommand(insertCustomerSql, connection, transaction);
                    insertCommand.Parameters.Add("@HoTen", SqlDbType.NVarChar, 100).Value = hoTen;
                    insertCommand.Parameters.Add("@CCCD", SqlDbType.VarChar, 20).Value = cccd;
                    insertCommand.Parameters.Add("@SoDienThoai", SqlDbType.VarChar, 15).Value =
                        string.IsNullOrWhiteSpace(soDienThoai) ? DBNull.Value : soDienThoai;
                    maKH = Convert.ToInt32(insertCommand.ExecuteScalar());
                }
            }

            const string insertInvoiceSql = @"INSERT INTO dbo.PhieuDatPhong
                                               (SoPhong, MaKH, NgayNhanPhong, NgayTraPhong, TongTienThanhToan)
                                               OUTPUT INSERTED.MaPhieu
                                               VALUES (@SoPhong, @MaKH, GETDATE(), NULL, 0)";

            int maPhieu;
            using (var command = new SqlCommand(insertInvoiceSql, connection, transaction))
            {
                command.Parameters.Add("@SoPhong", SqlDbType.VarChar, 10).Value = soPhong;
                command.Parameters.Add("@MaKH", SqlDbType.Int).Value = maKH;
                maPhieu = Convert.ToInt32(command.ExecuteScalar());
            }

            const string updateRoomSql = @"UPDATE dbo.Phong
                                           SET TrangThai = N'Đang ở'
                                           WHERE SoPhong = @SoPhong";

            using (var command = new SqlCommand(updateRoomSql, connection, transaction))
            {
                command.Parameters.Add("@SoPhong", SqlDbType.VarChar, 10).Value = soPhong;
                command.ExecuteNonQuery();
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

    public DataTable GetActiveInvoices()
    {
        const string sql = @"SELECT p.MaPhieu,
                                    p.SoPhong,
                                    k.HoTen,
                                    k.CCCD,
                                    p.NgayNhanPhong,
                                    p.SoPhong + N' - ' + k.HoTen AS ThongTin
                             FROM dbo.PhieuDatPhong AS p
                             INNER JOIN dbo.KhachHang AS k ON k.MaKH = p.MaKH
                             WHERE p.NgayTraPhong IS NULL
                             ORDER BY p.SoPhong";

        using var connection = DbHelper.OpenConnection();
        using var adapter = new SqlDataAdapter(sql, connection);
        var table = new DataTable();
        adapter.Fill(table);
        return table;
    }

    public PhieuDatPhongDTO? GetCheckoutInfo(int maPhieu)
    {
        const string sql = @"SELECT p.MaPhieu, p.SoPhong, p.MaKH, k.HoTen, k.CCCD,
                                    ph.LoaiPhong, ph.GiaTheoGio,
                                    p.NgayNhanPhong, p.NgayTraPhong, p.TongTienThanhToan
                             FROM dbo.PhieuDatPhong AS p
                             INNER JOIN dbo.KhachHang AS k ON k.MaKH = p.MaKH
                             INNER JOIN dbo.Phong AS ph ON ph.SoPhong = p.SoPhong
                             WHERE p.MaPhieu = @MaPhieu AND p.NgayTraPhong IS NULL";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new PhieuDatPhongDTO
        {
            MaPhieu = reader.GetInt32(0),
            SoPhong = reader.GetString(1),
            MaKH = reader.GetInt32(2),
            HoTen = reader.GetString(3),
            CCCD = reader.GetString(4),
            LoaiPhong = reader.GetString(5),
            GiaTheoGio = reader.GetDecimal(6),
            NgayNhanPhong = reader.GetDateTime(7),
            NgayTraPhong = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
            TongTienThanhToan = reader.GetDecimal(9)
        };
    }

    public void AddService(int maPhieu, int maDV, int soLuong)
    {
        using var connection = DbHelper.OpenConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string checkSql = @"SELECT COUNT(*)
                                      FROM dbo.PhieuDatPhong WITH (UPDLOCK, HOLDLOCK)
                                      WHERE MaPhieu = @MaPhieu AND NgayTraPhong IS NULL";

            using (var command = new SqlCommand(checkSql, connection, transaction))
            {
                command.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;
                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                {
                    throw new InvalidOperationException("Phiếu đã trả phòng hoặc không tồn tại.");
                }
            }

            const string existsSql = @"SELECT COUNT(*)
                                       FROM dbo.SuDungDichVu
                                       WHERE MaPhieu = @MaPhieu AND MaDV = @MaDV";

            bool daSuDung;
            using (var command = new SqlCommand(existsSql, connection, transaction))
            {
                command.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;
                command.Parameters.Add("@MaDV", SqlDbType.Int).Value = maDV;
                daSuDung = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }

            if (daSuDung)
            {
                const string updateSql = @"UPDATE dbo.SuDungDichVu
                                           SET SoLuong = SoLuong + @SoLuong,
                                               ThoiGian = GETDATE()
                                           WHERE MaPhieu = @MaPhieu AND MaDV = @MaDV";

                using var command = new SqlCommand(updateSql, connection, transaction);
                command.Parameters.Add("@SoLuong", SqlDbType.Int).Value = soLuong;
                command.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;
                command.Parameters.Add("@MaDV", SqlDbType.Int).Value = maDV;
                command.ExecuteNonQuery();
            }
            else
            {
                const string insertSql = @"INSERT INTO dbo.SuDungDichVu (MaPhieu, MaDV, SoLuong, ThoiGian)
                                           VALUES (@MaPhieu, @MaDV, @SoLuong, GETDATE())";

                using var command = new SqlCommand(insertSql, connection, transaction);
                command.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;
                command.Parameters.Add("@MaDV", SqlDbType.Int).Value = maDV;
                command.Parameters.Add("@SoLuong", SqlDbType.Int).Value = soLuong;
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public DataTable GetUsedServices(int maPhieu)
    {
        const string sql = @"SELECT ctdv.MaDV, dv.TenDV, ctdv.SoLuong, dv.DonGia, ctdv.ThoiGian
                             FROM dbo.SuDungDichVu AS ctdv
                             INNER JOIN dbo.DichVu AS dv ON dv.MaDV = ctdv.MaDV
                             WHERE ctdv.MaPhieu = @MaPhieu
                             ORDER BY ctdv.ThoiGian";

        using var connection = DbHelper.OpenConnection();
        using var adapter = new SqlDataAdapter(sql, connection);
        adapter.SelectCommand!.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;

        var table = new DataTable();
        adapter.Fill(table);
        table.Columns.Add("ThanhTien", typeof(decimal), "SoLuong * DonGia");
        return table;
    }

    public decimal GetServiceTotal(int maPhieu)
    {
        const string sql = @"SELECT ISNULL(SUM(CAST(SoLuong AS DECIMAL(18,2)) * DonGia), 0)
                             FROM dbo.SuDungDichVu
                             WHERE MaPhieu = @MaPhieu";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;
        return Convert.ToDecimal(command.ExecuteScalar());
    }

    public void CheckOut(int maPhieu, decimal tongTien, DateTime thoiGianRa)
    {
        using var connection = DbHelper.OpenConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string infoSql = @"SELECT SoPhong
                                     FROM dbo.PhieuDatPhong WITH (UPDLOCK, HOLDLOCK)
                                     WHERE MaPhieu = @MaPhieu AND NgayTraPhong IS NULL";

            string soPhong;
            using (var command = new SqlCommand(infoSql, connection, transaction))
            {
                command.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;
                var value = command.ExecuteScalar();
                if (value is null)
                {
                    throw new InvalidOperationException("Phiếu đã được trả phòng hoặc không tồn tại.");
                }

                soPhong = value.ToString() ?? string.Empty;
            }

            const string updateInvoiceSql = @"UPDATE dbo.PhieuDatPhong
                                              SET NgayTraPhong = @NgayTraPhong,
                                                  TongTienThanhToan = @TongTien
                                              WHERE MaPhieu = @MaPhieu AND NgayTraPhong IS NULL";

            using (var command = new SqlCommand(updateInvoiceSql, connection, transaction))
            {
                command.Parameters.Add("@NgayTraPhong", SqlDbType.DateTime).Value = thoiGianRa;
                var tongTienParameter = command.Parameters.Add("@TongTien", SqlDbType.Decimal);
                tongTienParameter.Precision = 18;
                tongTienParameter.Scale = 2;
                tongTienParameter.Value = tongTien;
                command.Parameters.Add("@MaPhieu", SqlDbType.Int).Value = maPhieu;

                if (command.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException("Không thể cập nhật phiếu trả phòng.");
                }
            }

            const string updateRoomSql = @"UPDATE dbo.Phong
                                           SET TrangThai = N'Đang dọn dẹp'
                                           WHERE SoPhong = @SoPhong";

            using (var command = new SqlCommand(updateRoomSql, connection, transaction))
            {
                command.Parameters.Add("@SoPhong", SqlDbType.VarChar, 10).Value = soPhong;
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
