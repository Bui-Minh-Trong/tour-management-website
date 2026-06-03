using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class KhachDiTour
{
    public int MaDangKy { get; set; }

    public int MaDoan { get; set; }

    public int MaKh { get; set; }

    public int? MaKm { get; set; }

    // MaNvKinhDoanh: Tương ứng với SaleID trong Class Diagram
    // Nhân viên Kinh doanh lập phiếu đặt tour cho khách hàng
    public int? MaNvKinhDoanh { get; set; }

    public DateTime NgayDangKy { get; set; }

    public int SoNguoiDi { get; set; }

    public decimal DonGia { get; set; }

    public decimal? SoTienGiam { get; set; }

    public string TrangThaiThanhToan { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual ICollection<ChiTietKhachDiTour> ChiTietKhachDiTours { get; set; } = new List<ChiTietKhachDiTour>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual DoanDuLich MaDoanNavigation { get; set; } = null!;

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual KhuyenMai? MaKmNavigation { get; set; }

    // Navigation: Nhân viên Kinh doanh tạo đặt tour (tương ứng SaleID trong Class Diagram)
    public virtual NhanVien? MaNvKinhDoanhNavigation { get; set; }
}
