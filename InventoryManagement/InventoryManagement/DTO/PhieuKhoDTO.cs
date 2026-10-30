namespace InventoryManagement.DTO;

public class PhieuKhoDTO
{
    public int MaPhieu { get; set; }
    public string LoaiPhieu { get; set; } = "NHAP";
    public DateTime NgayLap { get; set; } = DateTime.Now;
    public int? MaNCC { get; set; }
    public string TenNCC { get; set; } = string.Empty;
    public decimal TongTien { get; set; }
    public List<ChiTietPhieuKhoDTO> ChiTiet { get; set; } = new();
    public string LoaiPhieuHienThi => LoaiPhieu == "NHAP" ? "Nhập kho" : "Xuất kho";
}
