using System.Data;
using HotelManagement.DAL;
using HotelManagement.DTO;

namespace HotelManagement.BUS;

public class PhongBUS
{
    private readonly PhongDAL _dal = new();

    public DataTable GetAllData() => _dal.GetAllData();

    public List<PhongDTO> GetEmptyRooms() => _dal.GetEmptyRooms();

    public PhongDTO? GetBySoPhong(string soPhong) => _dal.GetBySoPhong(soPhong);
}
