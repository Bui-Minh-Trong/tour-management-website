using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class TaiKhoan
{
    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public int MaNv { get; set; }

    public int MaVaiTro { get; set; }

    public bool TrangThai { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; } = null!;

    public virtual VaiTro MaVaiTroNavigation { get; set; } = null!;
}
