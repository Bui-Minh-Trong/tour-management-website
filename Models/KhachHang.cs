using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class KhachHang
{
    public int MaKh { get; set; }

    public string HoTen { get; set; } = null!;

    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string Sdt { get; set; } = null!;

    public string? Email { get; set; }

    public string? Cccd { get; set; }

    public string? DiaChi { get; set; }

    public virtual ICollection<ChiTietKhachDiTour> ChiTietKhachDiTours { get; set; } = new List<ChiTietKhachDiTour>();

    public virtual ICollection<KhachDiTour> KhachDiTours { get; set; } = new List<KhachDiTour>();
}
