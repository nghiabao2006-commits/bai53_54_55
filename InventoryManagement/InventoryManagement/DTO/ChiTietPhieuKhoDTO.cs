namespace InventoryManagement.DTO;

public class ChiTietPhieuKhoDTO
{
    public int MaPhieu { get; set; }
    public string MaVatTu { get; set; } = string.Empty;
    public string TenVatTu { get; set; } = string.Empty;
    public string DVT { get; set; } = string.Empty;
    public decimal SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien => SoLuong * DonGia;
}
