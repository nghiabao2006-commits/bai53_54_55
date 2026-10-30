using System.Data;
using HotelManagement.DAL;
using HotelManagement.DTO;

namespace HotelManagement.BUS;

public class DichVuBUS
{
    private readonly DichVuDAL _dichVuDal = new();
    private readonly PhieuDatPhongDAL _phieuDal = new();

    public List<DichVuDTO> GetAll() => _dichVuDal.GetAll();

    public DataTable GetUsedServices(int maPhieu) => _phieuDal.GetUsedServices(maPhieu);

    public decimal GetServiceTotal(int maPhieu) => _phieuDal.GetServiceTotal(maPhieu);

    public void AddService(int maPhieu, int maDV, int soLuong)
    {
        if (maPhieu <= 0)
        {
            throw new ArgumentException("Vui lòng chọn phiếu lưu trú.");
        }

        if (maDV <= 0)
        {
            throw new ArgumentException("Vui lòng chọn dịch vụ.");
        }

        if (soLuong <= 0)
        {
            throw new ArgumentException("Số lượng dịch vụ phải lớn hơn 0.");
        }

        _phieuDal.AddService(maPhieu, maDV, soLuong);
    }
}
