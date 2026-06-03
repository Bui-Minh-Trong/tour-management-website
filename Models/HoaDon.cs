using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class HoaDon
{
    public int MaHoaDon { get; set; }

    public int MaDangKy { get; set; }

    public DateTime NgayLap { get; set; }

    public decimal SoTien { get; set; }

    public string LoaiHoaDon { get; set; } = null!;

    public string HinhThucTt { get; set; } = null!;

    // GhiChu: Tương ứng với Note trong Class Diagram lớp PhieuThu
    public string? GhiChu { get; set; }

    public int MaNvLap { get; set; }

    public virtual KhachDiTour MaDangKyNavigation { get; set; } = null!;

    public virtual NhanVien MaNvLapNavigation { get; set; } = null!;
}
