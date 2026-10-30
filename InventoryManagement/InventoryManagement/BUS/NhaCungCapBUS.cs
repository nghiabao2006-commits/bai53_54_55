using System.Data.SqlClient;
using InventoryManagement.DAL;
using InventoryManagement.DTO;

namespace InventoryManagement.BUS;

public class NhaCungCapBUS
{
    private readonly NhaCungCapDAL _dal = new();

    public List<NhaCungCapDTO> Search(string? keyword = null) => _dal.Search(keyword);

    public void Save(NhaCungCapDTO nhaCungCap, bool isNew)
    {
        nhaCungCap.TenNCC = nhaCungCap.TenNCC?.Trim() ?? string.Empty;
        nhaCungCap.DienThoai = string.IsNullOrWhiteSpace(nhaCungCap.DienThoai)
            ? null
            : nhaCungCap.DienThoai.Trim();
        nhaCungCap.DiaChi = string.IsNullOrWhiteSpace(nhaCungCap.DiaChi)
            ? null
            : nhaCungCap.DiaChi.Trim();

        if (string.IsNullOrWhiteSpace(nhaCungCap.TenNCC))
        {
            throw new ArgumentException("Tên nhà cung cấp không được để trống.");
        }

        if (nhaCungCap.TenNCC.Length > 150)
        {
            throw new ArgumentException("Tên nhà cung cấp tối đa 150 ký tự.");
        }

        if (nhaCungCap.DienThoai is { Length: > 15 })
        {
            throw new ArgumentException("Số điện thoại tối đa 15 ký tự.");
        }

        if (nhaCungCap.DiaChi is { Length: > 255 })
        {
            throw new ArgumentException("Địa chỉ tối đa 255 ký tự.");
        }

        try
        {
            if (isNew)
            {
                _dal.Insert(nhaCungCap);
            }
            else
            {
                _dal.Update(nhaCungCap);
            }
        }
        catch (SqlException ex) when (ex.Number is 2627 or 2601)
        {
            throw new InvalidOperationException("Số điện thoại của nhà cung cấp đã tồn tại.");
        }
    }

    public void Delete(int maNCC) => _dal.Delete(maNCC);
}
