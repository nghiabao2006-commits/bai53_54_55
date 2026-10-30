namespace HotelManagement.DTO;

public class DichVuDTO
{
    public int MaDV { get; set; }
    public string TenDV { get; set; } = string.Empty;
    public decimal DonGia { get; set; }

    public string HienThi => $"{TenDV} - {DonGia:N0} đ";
}
