namespace CafePOS.DTO;

public class BanAnDTO
{
    public int MaBan { get; set; }
    public string TenBan { get; set; } = string.Empty;
    public string TrangThai { get; set; } = "Trống";

    public bool CoKhach => TrangThai == "Có khách";
}
