using System;

namespace QuanLyTourDuLich.ViewModels;

public class HoaDonListVM
{
    public int MaHoaDon { get; set; }
    public int MaDangKy { get; set; }
    public string TenKhachHang { get; set; } = null!;
    public string TenDoan { get; set; } = null!;
    public DateTime NgayLap { get; set; }
    public decimal SoTien { get; set; }
    public string LoaiHoaDon { get; set; } = null!;
    public string HinhThucTt { get; set; } = null!;
    public string TenNhanVienLap { get; set; } = null!;
}
