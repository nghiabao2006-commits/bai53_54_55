using System.Data.SqlClient;
using InventoryManagement.DAL;
using InventoryManagement.DTO;

namespace InventoryManagement.BUS;

public class VatTuBUS
{
    private readonly VatTuDAL _dal = new();

    public List<VatTuDTO> Search(string? keyword = null) => _dal.Search(keyword);

    public List<VatTuDTO> GetLowStock(decimal threshold)
    {
        if (threshold < 0)
        {
            throw new ArgumentException("Ngưỡng tồn kho phải lớn hơn hoặc bằng 0.");
        }

        return _dal.Search(null, true, threshold);
    }

    public VatTuDTO? GetByMa(string maVatTu) => _dal.GetByMa(maVatTu);

    public void Save(VatTuDTO vatTu, bool isNew)
    {
        vatTu.MaVatTu = vatTu.MaVatTu?.Trim() ?? string.Empty;
        vatTu.TenVatTu = vatTu.TenVatTu?.Trim() ?? string.Empty;
        vatTu.DVT = vatTu.DVT?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(vatTu.MaVatTu))
        {
            throw new ArgumentException("Mã vật tư không được để trống.");
        }

        if (vatTu.MaVatTu.Length > 20)
        {
            throw new ArgumentException("Mã vật tư tối đa 20 ký tự.");
        }

        if (string.IsNullOrWhiteSpace(vatTu.TenVatTu))
        {
            throw new ArgumentException("Tên vật tư không được để trống.");
        }

        if (vatTu.TenVatTu.Length > 150)
        {
            throw new ArgumentException("Tên vật tư tối đa 150 ký tự.");
        }

        if (string.IsNullOrWhiteSpace(vatTu.DVT))
        {
            throw new ArgumentException("Đơn vị tính không được để trống.");
        }

        if (vatTu.DVT.Length > 20)
        {
            throw new ArgumentException("Đơn vị tính tối đa 20 ký tự.");
        }

        if (vatTu.SoLuongTon < 0)
        {
            throw new ArgumentException("Số lượng tồn phải lớn hơn hoặc bằng 0.");
        }

        try
        {
            if (isNew)
            {
                _dal.Insert(vatTu);
            }
            else
            {
                _dal.Update(vatTu);
            }
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            throw new InvalidOperationException($"Mã vật tư '{vatTu.MaVatTu}' đã tồn tại.");
        }
    }

    public void Delete(string maVatTu) => _dal.Delete(maVatTu);

    public decimal GetTotalInventoryValue() => _dal.GetTotalInventoryValue();
}
