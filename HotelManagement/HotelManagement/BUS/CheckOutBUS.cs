using HotelManagement.DAL;
using HotelManagement.DTO;

namespace HotelManagement.BUS;

public class CheckOutBUS
{
    private readonly PhieuDatPhongDAL _phieuDal = new();

    public CheckoutDTO Preview(int maPhieu)
    {
        var info = _phieuDal.GetCheckoutInfo(maPhieu)
            ?? throw new InvalidOperationException("Phiếu đã trả phòng hoặc không tồn tại.");

        var ngayTraPhong = DateTime.Now;
        var soGio = (decimal)Math.Ceiling((ngayTraPhong - info.NgayNhanPhong).TotalHours);
        if (soGio < 1)
        {
            soGio = 1;
        }

        var tienPhong = info.GiaTheoGio * soGio;
        var tienDichVu = _phieuDal.GetServiceTotal(maPhieu);

        return new CheckoutDTO
        {
            MaPhieu = info.MaPhieu,
            SoPhong = info.SoPhong,
            LoaiPhong = info.LoaiPhong,
            HoTen = info.HoTen,
            CCCD = info.CCCD,
            NgayNhanPhong = info.NgayNhanPhong,
            NgayTraPhong = ngayTraPhong,
            SoGio = soGio,
            TienPhong = tienPhong,
            TienDichVu = tienDichVu,
            TongTien = tienPhong + tienDichVu,
            DichVu = _phieuDal.GetUsedServices(maPhieu)
        };
    }

    public void Confirm(CheckoutDTO checkout)
    {
        if (checkout.TongTien < 0)
        {
            throw new ArgumentException("Tổng tiền thanh toán không hợp lệ.");
        }

        _phieuDal.CheckOut(checkout.MaPhieu, checkout.TongTien, checkout.NgayTraPhong);
    }
}
