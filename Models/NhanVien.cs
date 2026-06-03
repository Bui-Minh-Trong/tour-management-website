using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class NhanVien
{
    public int MaNv { get; set; }

    public string HoTen { get; set; } = null!;

    public string ChucVu { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string? Email { get; set; }

    public DateOnly NgayVaoLam { get; set; }

    public virtual ICollection<ChiPhiDoan> ChiPhiDoans { get; set; } = new List<ChiPhiDoan>();

    public virtual ICollection<DoanDuLich> DoanDuLiches { get; set; } = new List<DoanDuLich>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual TaiKhoan? TaiKhoan { get; set; }
}
