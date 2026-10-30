using System.Data;

namespace HotelManagement.DTO;

public class CheckoutDTO
{
    public int MaPhieu { get; set; }
    public string SoPhong { get; set; } = string.Empty;
    public string LoaiPhong { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string CCCD { get; set; } = string.Empty;
    public DateTime NgayNhanPhong { get; set; }
    public DateTime NgayTraPhong { get; set; }
    public decimal SoGio { get; set; }
    public decimal TienPhong { get; set; }
    public decimal TienDichVu { get; set; }
    public decimal TongTien { get; set; }
    public DataTable DichVu { get; set; } = new();
}
