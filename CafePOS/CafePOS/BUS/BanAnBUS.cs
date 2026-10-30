using CafePOS.DAL;
using CafePOS.DTO;

namespace CafePOS.BUS;

public class BanAnBUS
{
    private readonly BanAnDAL _dal = new();

    public List<BanAnDTO> GetAll() => _dal.GetAllDto();

    public BanAnDTO? GetById(int maBan) => _dal.GetById(maBan);

    public List<BanAnDTO> GetEmptyTables(int? excludeMaBan = null)
    {
        return GetAll()
            .Where(ban => ban.TrangThai == "Trống" && ban.MaBan != excludeMaBan)
            .ToList();
    }
}
