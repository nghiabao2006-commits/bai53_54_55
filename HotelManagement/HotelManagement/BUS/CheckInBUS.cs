using System.Data;
using HotelManagement.DAL;
using HotelManagement.DTO;

namespace HotelManagement.BUS;

public class CheckInBUS
{
    private readonly KhachHangDAL _khachHangDal = new();
    private readonly PhieuDatPhongDAL _phieuDal = new();

    public KhachHangDTO? Lookup(string cccd)
    {
        cccd = cccd.Trim();
        ValidateCCCD(cccd);
        return _khachHangDal.FindByCCCD(cccd);
    }

    public int CheckIn(string soPhong, string hoTen, string cccd, string soDienThoai)
    {
        hoTen = hoTen.Trim();
        cccd = cccd.Trim();
        soDienThoai = soDienThoai.Trim();

        if (string.IsNullOrWhiteSpace(soPhong))
        {
            throw new ArgumentException("Vui lòng chọn phòng trống.");
        }

        if (string.IsNullOrWhiteSpace(hoTen))
        {
            throw new ArgumentException("Họ tên khách hàng không được để trống.");
        }

        ValidateCCCD(cccd);

        if (soDienThoai.Length > 15)
        {
            throw new ArgumentException("Số điện thoại không được vượt quá 15 ký tự.");
        }

        return _phieuDal.CheckIn(soPhong, hoTen, cccd, soDienThoai);
    }

    public DataTable GetActiveInvoices() => _phieuDal.GetActiveInvoices();

    private static void ValidateCCCD(string cccd)
    {
        if (cccd.Length is < 9 or > 12 || !cccd.All(char.IsDigit))
        {
            throw new ArgumentException("CCCD phải gồm 9 đến 12 chữ số.");
        }
    }
}
