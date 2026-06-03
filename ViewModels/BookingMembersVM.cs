using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyTourDuLich.ViewModels;

public class BookingMembersVM
{
    public int MaDangKy { get; set; }
    public string TenKhachHangDaiDien { get; set; } = "";
    public string TenDoan { get; set; } = "";
    public int SoNguoiDi { get; set; }

    public List<MemberInfoVM> Members { get; set; } = new();

    // Dùng để thêm thành viên mới
    public int NewMaKh { get; set; }
    public List<SelectListItem> CustomerSelectList { get; set; } = new();
}

public class MemberInfoVM
{
    public int MaKh { get; set; }
    public string HoTen { get; set; } = "";
    public string Sdt { get; set; } = "";
    public string Email { get; set; } = "";
    public string Cccd { get; set; } = "";
    public bool LaNguoiDaiDien { get; set; }
}
