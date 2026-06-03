using System;

namespace QuanLyTourDuLich.ViewModels;

public class BookingListVM
{
    public int MaDangKy { get; set; }
    public string TenKhachHang { get; set; } = null!;
    public string TenDoan { get; set; } = null!;
    public int SoNguoiDi { get; set; }
    public DateTime NgayDangKy { get; set; }
    public decimal DonGia { get; set; }
    public decimal SoTienGiam { get; set; }
    public decimal TongTien => (SoNguoiDi * DonGia) - SoTienGiam;
    public string TrangThaiThanhToan { get; set; } = null!;
}
