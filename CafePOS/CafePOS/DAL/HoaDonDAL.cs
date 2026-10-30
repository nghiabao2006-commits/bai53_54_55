using System.Data;
using System.Data.SqlClient;
using CafePOS.DTO;

namespace CafePOS.DAL;

public class HoaDonDAL
{
    public HoaDonDTO? GetOpenInvoice(int maBan)
    {
        const string sql = @"SELECT TOP (1) MaHoaDon, MaBan, GioVao, GioRa, TongTien, TrangThai
                             FROM dbo.HoaDon
                             WHERE MaBan = @MaBan AND TrangThai = 0
                             ORDER BY MaHoaDon DESC";

        using var connection = DbHelper.OpenConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@MaBan", SqlDbType.Int).Value = maBan;

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new HoaDonDTO
        {
            MaHoaDon = reader.GetInt32(0),
            MaBan = reader.GetInt32(1),
            GioVao = reader.GetDateTime(2),
            GioRa = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
            TongTien = reader.GetDecimal(4),
            TrangThai = reader.GetBoolean(5)
        };
    }

    public int CreateOpenInvoice(int maBan)
    {
        using var connection = DbHelper.OpenConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string findSql = @"SELECT TOP (1) MaHoaDon
                                     FROM dbo.HoaDon WITH (UPDLOCK, HOLDLOCK)
                                     WHERE MaBan = @MaBan AND TrangThai = 0
                                     ORDER BY MaHoaDon DESC";

            using (var findCommand = new SqlCommand(findSql, connection, transaction))
            {
                findCommand.Parameters.Add("@MaBan", SqlDbType.Int).Value = maBan;
                var existing = findCommand.ExecuteScalar();
                if (existing != null)
                {
                    transaction.Commit();
                    return Convert.ToInt32(existing);
                }
            }

            const string insertSql = @"INSERT INTO dbo.HoaDon (MaBan, GioVao, GioRa, TongTien, TrangThai)
                                       OUTPUT INSERTED.MaHoaDon
                                       VALUES (@MaBan, GETDATE(), NULL, 0, 0)";

            int maHoaDon;
            using (var insertCommand = new SqlCommand(insertSql, connection, transaction))
            {
                insertCommand.Parameters.Add("@MaBan", SqlDbType.Int).Value = maBan;
                maHoaDon = Convert.ToInt32(insertCommand.ExecuteScalar());
            }

            const string updateSql = @"UPDATE dbo.BanAn
                                       SET TrangThai = N'Có khách'
                                       WHERE MaBan = @MaBan";

            using (var updateCommand = new SqlCommand(updateSql, connection, transaction))
            {
                updateCommand.Parameters.Add("@MaBan", SqlDbType.Int).Value = maBan;
                updateCommand.ExecuteNonQuery();
            }

            transaction.Commit();
            return maHoaDon;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public DataTable CreateEmptyOrderTable()
    {
        var table = new DataTable();
        table.Columns.Add("MaHoaDon", typeof(int));
        table.Columns.Add("MaMon", typeof(int));
        table.Columns.Add("TenMon", typeof(string));
        table.Columns.Add("SoLuong", typeof(int));
        table.Columns.Add("DonGia", typeof(decimal));
        table.Columns.Add("ThanhTien", typeof(decimal), "SoLuong * DonGia");
        return table;
    }

    public DataTable LoadOrder(int maHoaDon)
    {
        const string sql = @"SELECT ct.MaHoaDon, ct.MaMon, m.TenMon, ct.SoLuong, ct.DonGia
                             FROM dbo.ChiTietHoaDon AS ct
                             INNER JOIN dbo.MonAn AS m ON m.MaMon = ct.MaMon
                             WHERE ct.MaHoaDon = @MaHoaDon
                             ORDER BY m.TenMon";

        var table = CreateEmptyOrderTable();

        using var connection = DbHelper.OpenConnection();
        using var adapter = new SqlDataAdapter(sql, connection);
        adapter.SelectCommand!.Parameters.Add("@MaHoaDon", SqlDbType.Int).Value = maHoaDon;
        adapter.Fill(table);
        return table;
    }

    public void SaveOrderDetails(DataTable table)
    {
        if (table.GetChanges() is null)
        {
            return;
        }

        using var connection = DbHelper.OpenConnection();
        using var adapter = new SqlDataAdapter();

        var insert = new SqlCommand(
            @"INSERT INTO dbo.ChiTietHoaDon (MaHoaDon, MaMon, SoLuong, DonGia)
              VALUES (@MaHoaDon, @MaMon, @SoLuong, @DonGia)", connection);
        insert.Parameters.Add("@MaHoaDon", SqlDbType.Int, 0, "MaHoaDon");
        insert.Parameters.Add("@MaMon", SqlDbType.Int, 0, "MaMon");
        insert.Parameters.Add("@SoLuong", SqlDbType.Int, 0, "SoLuong");
        var donGia = insert.Parameters.Add("@DonGia", SqlDbType.Decimal, 0, "DonGia");
        donGia.Precision = 18;
        donGia.Scale = 2;
        insert.UpdatedRowSource = UpdateRowSource.None;

        var update = new SqlCommand(
            @"UPDATE dbo.ChiTietHoaDon
              SET SoLuong = @SoLuong
              WHERE MaHoaDon = @MaHoaDon AND MaMon = @MaMon", connection);
        update.Parameters.Add("@SoLuong", SqlDbType.Int, 0, "SoLuong");
        update.Parameters.Add("@MaHoaDon", SqlDbType.Int, 0, "MaHoaDon");
        update.Parameters.Add("@MaMon", SqlDbType.Int, 0, "MaMon");
        update.UpdatedRowSource = UpdateRowSource.None;

        var delete = new SqlCommand(
            @"DELETE FROM dbo.ChiTietHoaDon
              WHERE MaHoaDon = @MaHoaDon AND MaMon = @MaMon", connection);
        delete.Parameters.Add("@MaHoaDon", SqlDbType.Int, 0, "MaHoaDon");
        delete.Parameters.Add("@MaMon", SqlDbType.Int, 0, "MaMon");
        delete.UpdatedRowSource = UpdateRowSource.None;

        adapter.InsertCommand = insert;
        adapter.UpdateCommand = update;
        adapter.DeleteCommand = delete;
        adapter.Update(table);
    }

    public void Pay(int maHoaDon, decimal tongTien)
    {
        using var connection = DbHelper.OpenConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string invoiceSql = @"SELECT MaBan
                                        FROM dbo.HoaDon WITH (UPDLOCK, HOLDLOCK)
                                        WHERE MaHoaDon = @MaHoaDon AND TrangThai = 0";

            int maBan;
            using (var command = new SqlCommand(invoiceSql, connection, transaction))
            {
                command.Parameters.Add("@MaHoaDon", SqlDbType.Int).Value = maHoaDon;
                var value = command.ExecuteScalar();
                if (value == null)
                {
                    throw new InvalidOperationException("Hóa đơn không tồn tại hoặc đã được thanh toán.");
                }

                maBan = Convert.ToInt32(value);
            }

            const string updateInvoiceSql = @"UPDATE dbo.HoaDon
                                              SET GioRa = GETDATE(),
                                                  TongTien = @TongTien,
                                                  TrangThai = 1
                                              WHERE MaHoaDon = @MaHoaDon AND TrangThai = 0";

            using (var command = new SqlCommand(updateInvoiceSql, connection, transaction))
            {
                var tongTienParameter = command.Parameters.Add("@TongTien", SqlDbType.Decimal);
                tongTienParameter.Precision = 18;
                tongTienParameter.Scale = 2;
                tongTienParameter.Value = tongTien;
                command.Parameters.Add("@MaHoaDon", SqlDbType.Int).Value = maHoaDon;
                if (command.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException("Không thể cập nhật hóa đơn thanh toán.");
                }
            }

            const string updateTableSql = @"UPDATE dbo.BanAn
                                            SET TrangThai = N'Trống'
                                            WHERE MaBan = @MaBan";

            using (var command = new SqlCommand(updateTableSql, connection, transaction))
            {
                command.Parameters.Add("@MaBan", SqlDbType.Int).Value = maBan;
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

    public void TransferTable(int maHoaDon, int maBanDich)
    {
        using var connection = DbHelper.OpenConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string sourceSql = @"SELECT MaBan
                                       FROM dbo.HoaDon WITH (UPDLOCK, HOLDLOCK)
                                       WHERE MaHoaDon = @MaHoaDon AND TrangThai = 0";

            int maBanNguon;
            using (var command = new SqlCommand(sourceSql, connection, transaction))
            {
                command.Parameters.Add("@MaHoaDon", SqlDbType.Int).Value = maHoaDon;
                var value = command.ExecuteScalar();
                if (value == null)
                {
                    throw new InvalidOperationException("Hóa đơn không còn mở để chuyển bàn.");
                }

                maBanNguon = Convert.ToInt32(value);
            }

            if (maBanNguon == maBanDich)
            {
                throw new InvalidOperationException("Bàn đích phải khác bàn hiện tại.");
            }

            const string targetSql = @"SELECT TrangThai
                                       FROM dbo.BanAn WITH (UPDLOCK, HOLDLOCK)
                                       WHERE MaBan = @MaBan";

            using (var command = new SqlCommand(targetSql, connection, transaction))
            {
                command.Parameters.Add("@MaBan", SqlDbType.Int).Value = maBanDich;
                var value = command.ExecuteScalar();
                if (value == null)
                {
                    throw new InvalidOperationException("Bàn đích không tồn tại.");
                }

                if (value.ToString() != "Trống")
                {
                    throw new InvalidOperationException("Bàn đích đang có khách, vui lòng chọn bàn khác.");
                }
            }

            const string checkInvoiceSql = @"SELECT COUNT(*)
                                             FROM dbo.HoaDon WITH (UPDLOCK, HOLDLOCK)
                                             WHERE MaBan = @MaBan AND TrangThai = 0";

            using (var command = new SqlCommand(checkInvoiceSql, connection, transaction))
            {
                command.Parameters.Add("@MaBan", SqlDbType.Int).Value = maBanDich;
                if (Convert.ToInt32(command.ExecuteScalar()) > 0)
                {
                    throw new InvalidOperationException("Bàn đích đang có hóa đơn chưa thanh toán.");
                }
            }

            using (var command = new SqlCommand(
                "UPDATE dbo.HoaDon SET MaBan = @MaBanDich WHERE MaHoaDon = @MaHoaDon", connection, transaction))
            {
                command.Parameters.Add("@MaBanDich", SqlDbType.Int).Value = maBanDich;
                command.Parameters.Add("@MaHoaDon", SqlDbType.Int).Value = maHoaDon;
                command.ExecuteNonQuery();
            }

            using (var command = new SqlCommand(
                "UPDATE dbo.BanAn SET TrangThai = N'Trống' WHERE MaBan = @MaBan", connection, transaction))
            {
                command.Parameters.Add("@MaBan", SqlDbType.Int).Value = maBanNguon;
                command.ExecuteNonQuery();
            }

            using (var command = new SqlCommand(
                "UPDATE dbo.BanAn SET TrangThai = N'Có khách' WHERE MaBan = @MaBan", connection, transaction))
            {
                command.Parameters.Add("@MaBan", SqlDbType.Int).Value = maBanDich;
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
