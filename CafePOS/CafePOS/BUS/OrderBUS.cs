using System.Data;
using CafePOS.DAL;
using CafePOS.DTO;

namespace CafePOS.BUS;

public class OrderBUS
{
    private readonly HoaDonDAL _hoaDonDal = new();

    public OrderSessionDTO OpenTable(int maBan)
    {
        var hoaDon = _hoaDonDal.GetOpenInvoice(maBan);
        return new OrderSessionDTO
        {
            HoaDon = hoaDon,
            ChiTiet = hoaDon is null
                ? _hoaDonDal.CreateEmptyOrderTable()
                : _hoaDonDal.LoadOrder(hoaDon.MaHoaDon)
        };
    }

    public HoaDonDTO EnsureInvoice(int maBan)
    {
        var hoaDon = _hoaDonDal.GetOpenInvoice(maBan);
        if (hoaDon is not null)
        {
            return hoaDon;
        }

        var maHoaDon = _hoaDonDal.CreateOpenInvoice(maBan);
        return _hoaDonDal.GetOpenInvoice(maBan)
            ?? new HoaDonDTO { MaHoaDon = maHoaDon, MaBan = maBan, GioVao = DateTime.Now };
    }

    public void AddDish(DataTable chiTiet, HoaDonDTO hoaDon, MonAnDTO mon, int soLuong)
    {
        if (soLuong <= 0)
        {
            throw new ArgumentException("Số lượng phải lớn hơn 0.");
        }

        if (mon.DonGia < 0)
        {
            throw new ArgumentException("Đơn giá món không hợp lệ.");
        }

        try
        {
            var rows = chiTiet.Select($"MaMon = {mon.MaMon}");
            if (rows.Length > 0)
            {
                var row = rows[0];
                row["SoLuong"] = Convert.ToInt32(row["SoLuong"]) + soLuong;
            }
            else
            {
                var row = chiTiet.NewRow();
                row["MaHoaDon"] = hoaDon.MaHoaDon;
                row["MaMon"] = mon.MaMon;
                row["TenMon"] = mon.TenMon;
                row["SoLuong"] = soLuong;
                row["DonGia"] = mon.DonGia;
                chiTiet.Rows.Add(row);
            }

            _hoaDonDal.SaveOrderDetails(chiTiet);
        }
        catch
        {
            chiTiet.RejectChanges();
            throw;
        }
    }

    public void RemoveDish(DataTable chiTiet, DataRow row)
    {
        try
        {
            row.Delete();
            _hoaDonDal.SaveOrderDetails(chiTiet);
        }
        catch
        {
            chiTiet.RejectChanges();
            throw;
        }
    }

    public decimal GetTotal(DataTable chiTiet)
    {
        decimal total = 0;
        foreach (DataRow row in chiTiet.Rows)
        {
            if (row.RowState != DataRowState.Deleted)
            {
                total += Convert.ToDecimal(row["ThanhTien"]);
            }
        }

        return total;
    }

    public void Pay(HoaDonDTO hoaDon, DataTable chiTiet)
    {
        if (chiTiet.Rows.Count == 0)
        {
            throw new InvalidOperationException("Bàn chưa gọi món nên không thể thanh toán.");
        }

        _hoaDonDal.SaveOrderDetails(chiTiet);
        _hoaDonDal.Pay(hoaDon.MaHoaDon, GetTotal(chiTiet));
        chiTiet.Clear();
    }

    public void TransferTable(HoaDonDTO hoaDon, int maBanDich)
    {
        _hoaDonDal.TransferTable(hoaDon.MaHoaDon, maBanDich);
    }
}
