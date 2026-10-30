namespace CafePOS.DTO;

public class MonAnDTO
{
    public int MaMon { get; set; }
    public string TenMon { get; set; } = string.Empty;
    public int MaDanhMuc { get; set; }
    public string TenDanhMuc { get; set; } = string.Empty;
    public decimal DonGia { get; set; }
}
