namespace HotelManagement.DTO;

public class SuDungDichVuDTO
{
    public int MaPhieu { get; set; }
    public int MaDV { get; set; }
    public string TenDV { get; set; } = string.Empty;
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public DateTime ThoiGian { get; set; }
    public decimal ThanhTien => SoLuong * DonGia;
}
