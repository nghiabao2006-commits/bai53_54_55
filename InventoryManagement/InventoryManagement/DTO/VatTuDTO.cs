namespace InventoryManagement.DTO;

public class VatTuDTO
{
    public string MaVatTu { get; set; } = string.Empty;
    public string TenVatTu { get; set; } = string.Empty;
    public string DVT { get; set; } = string.Empty;
    public decimal SoLuongTon { get; set; }
    public string HienThi => $"{MaVatTu} - {TenVatTu}";
}
