using System.Data;
using CafePOS.DAL;
using CafePOS.DTO;

namespace CafePOS.BUS;

public class MenuBUS
{
    private readonly DanhMucDAL _danhMucDal = new();
    private readonly MonAnDAL _monAnDal = new();

    public DataTable GetCategories() => _danhMucDal.GetAll();

    public DataTable GetDishes(int? maDanhMuc = null) => _monAnDal.GetData(maDanhMuc);

    public int AddCategory(string tenDanhMuc)
    {
        tenDanhMuc = tenDanhMuc.Trim();
        if (string.IsNullOrWhiteSpace(tenDanhMuc))
        {
            throw new ArgumentException("Tên danh mục không được để trống.");
        }

        return _danhMucDal.Insert(tenDanhMuc);
    }

    public void UpdateCategory(int maDanhMuc, string tenDanhMuc)
    {
        tenDanhMuc = tenDanhMuc.Trim();
        if (string.IsNullOrWhiteSpace(tenDanhMuc))
        {
            throw new ArgumentException("Tên danh mục không được để trống.");
        }

        if (_danhMucDal.Update(maDanhMuc, tenDanhMuc) == 0)
        {
            throw new InvalidOperationException("Không tìm thấy danh mục cần cập nhật.");
        }
    }

    public void DeleteCategory(int maDanhMuc)
    {
        if (_danhMucDal.Delete(maDanhMuc) == 0)
        {
            throw new InvalidOperationException("Không tìm thấy danh mục cần xóa.");
        }
    }

    public int AddDish(MonAnDTO mon)
    {
        ValidateDish(mon);
        return _monAnDal.Insert(mon);
    }

    public void UpdateDish(MonAnDTO mon)
    {
        ValidateDish(mon);
        if (_monAnDal.Update(mon) == 0)
        {
            throw new InvalidOperationException("Không tìm thấy món cần cập nhật.");
        }
    }

    public void DeleteDish(int maMon)
    {
        if (_monAnDal.Delete(maMon) == 0)
        {
            throw new InvalidOperationException("Không tìm thấy món cần xóa.");
        }
    }

    private static void ValidateDish(MonAnDTO mon)
    {
        if (string.IsNullOrWhiteSpace(mon.TenMon))
        {
            throw new ArgumentException("Tên món không được để trống.");
        }

        if (mon.DonGia < 0)
        {
            throw new ArgumentException("Đơn giá không được âm.");
        }

        if (mon.MaDanhMuc <= 0)
        {
            throw new ArgumentException("Vui lòng chọn danh mục cho món ăn.");
        }

        mon.TenMon = mon.TenMon.Trim();
    }
}
