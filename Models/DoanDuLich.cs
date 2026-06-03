using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class DoanDuLich
{
    public int MaDoan { get; set; }

    public string TenDoan { get; set; } = null!;

    public int MaTour { get; set; }

    public DateOnly NgayKhoiHanh { get; set; }

    public DateOnly NgayKetThuc { get; set; }

    public int MaNvHuongDanVien { get; set; }

    // MaNvDieuHanh: Tương ứng với OperatorID trong Class Diagram
    // Nhân viên Điều hành phụ trách lập kế hoạch và giám sát đoàn
    public int? MaNvDieuHanh { get; set; }

    public int SoLuongToiDa { get; set; }

    public int SoKhachHienTai { get; set; }

    public string TrangThaiDoan { get; set; } = null!;

    public virtual ICollection<ChiPhiDoan> ChiPhiDoans { get; set; } = new List<ChiPhiDoan>();

    public virtual ICollection<KhachDiTour> KhachDiTours { get; set; } = new List<KhachDiTour>();

    public virtual ICollection<LuuTru> LuuTrus { get; set; } = new List<LuuTru>();

    public virtual NhanVien MaNvHuongDanVienNavigation { get; set; } = null!;

    // Navigation: Nhân viên Điều hành (tương ứng OperatorID trong Class Diagram)
    public virtual NhanVien? MaNvDieuHanhNavigation { get; set; }

    public virtual Tour MaTourNavigation { get; set; } = null!;

    public virtual ICollection<PhancongVanchuyen> PhancongVanchuyens { get; set; } = new List<PhancongVanchuyen>();
}
