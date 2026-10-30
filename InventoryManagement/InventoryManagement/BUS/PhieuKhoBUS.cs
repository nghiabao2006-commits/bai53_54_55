using InventoryManagement.DAL;
using InventoryManagement.DTO;

namespace InventoryManagement.BUS;

public class PhieuKhoBUS
{
    private readonly PhieuKhoDAL _dal = new();

    public List<PhieuKhoDTO> GetAll() => _dal.GetAll();

    public List<ChiTietPhieuKhoDTO> GetDetails(int maPhieu) => _dal.GetDetails(maPhieu);

    public int Create(PhieuKhoDTO phieu)
    {
        if (phieu.LoaiPhieu is not ("NHAP" or "XUAT"))
        {
            throw new ArgumentException("Loại phiếu chỉ có thể là NHAP hoặc XUAT.");
        }

        if (phieu.LoaiPhieu == "NHAP" && phieu.MaNCC is null)
        {
            throw new ArgumentException("Phiếu nhập kho bắt buộc phải chọn nhà cung cấp.");
        }

        if (phieu.ChiTiet.Count == 0)
        {
            throw new ArgumentException("Phiếu kho phải có ít nhất một vật tư.");
        }

        var maVatTuDaCo = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var chiTiet in phieu.ChiTiet)
        {
            if (string.IsNullOrWhiteSpace(chiTiet.MaVatTu))
            {
                throw new ArgumentException("Vật tư trong phiếu không hợp lệ.");
            }

            if (!maVatTuDaCo.Add(chiTiet.MaVatTu))
            {
                throw new ArgumentException($"Vật tư '{chiTiet.MaVatTu}' bị trùng trong cùng một phiếu.");
            }

            if (chiTiet.SoLuong <= 0)
            {
                throw new ArgumentException($"Số lượng của vật tư '{chiTiet.MaVatTu}' phải lớn hơn 0.");
            }

            if (chiTiet.DonGia < 0)
            {
                throw new ArgumentException($"Đơn giá của vật tư '{chiTiet.MaVatTu}' không được âm.");
            }
        }

        phieu.TongTien = phieu.ChiTiet.Sum(ct => ct.SoLuong * ct.DonGia);
        return _dal.Create(phieu);
    }
}
