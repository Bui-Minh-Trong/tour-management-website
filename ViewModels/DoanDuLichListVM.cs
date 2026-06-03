using System;

namespace QuanLyTourDuLich.ViewModels;

public class DoanDuLichListVM
{
    public int MaDoan { get; set; }
    public string TenDoan { get; set; } = null!;
    public string TenTour { get; set; } = null!;
    public DateOnly NgayKhoiHanh { get; set; }
    public DateOnly NgayKetThuc { get; set; }
    public string TenHuongDanVien { get; set; } = null!;
    public int SoLuongToiDa { get; set; }
    public int SoKhachHienTai { get; set; }
    public string TrangThaiDoan { get; set; } = null!;
}
