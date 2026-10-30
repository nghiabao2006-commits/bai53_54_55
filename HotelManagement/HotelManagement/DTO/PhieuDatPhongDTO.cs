namespace HotelManagement.DTO;

public class PhieuDatPhongDTO
{
    public int MaPhieu { get; set; }
    public string SoPhong { get; set; } = string.Empty;
    public int MaKH { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string CCCD { get; set; } = string.Empty;
    public string LoaiPhong { get; set; } = string.Empty;
    public decimal GiaTheoGio { get; set; }
    public DateTime NgayNhanPhong { get; set; }
    public DateTime? NgayTraPhong { get; set; }
    public decimal TongTienThanhToan { get; set; }
}
