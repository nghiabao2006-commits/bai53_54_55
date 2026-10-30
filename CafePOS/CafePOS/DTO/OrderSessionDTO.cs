using System.Data;

namespace CafePOS.DTO;

public class OrderSessionDTO
{
    public HoaDonDTO? HoaDon { get; set; }
    public DataTable ChiTiet { get; set; } = new();
}
