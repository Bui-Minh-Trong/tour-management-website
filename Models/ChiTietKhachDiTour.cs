using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class ChiTietKhachDiTour
{
    public int MaDangKy { get; set; }

    public int MaKh { get; set; }

    public bool LaNguoiDaiDien { get; set; }

    public virtual KhachDiTour MaDangKyNavigation { get; set; } = null!;

    public virtual KhachHang MaKhNavigation { get; set; } = null!;
}
