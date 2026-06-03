using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.Models;

public partial class ChiPhiDoan
{
    public int MaChiPhi { get; set; }

    public int MaDoan { get; set; }

    // LoaiChiPhi: Tương ứng với ExpenseType trong Class Diagram
    public string LoaiChiPhi { get; set; } = "Khác";

    public string NoiDungChi { get; set; } = null!;

    public decimal SoTien { get; set; }

    public DateTime NgayChi { get; set; }

    public int MaNvChi { get; set; }

    public virtual DoanDuLich MaDoanNavigation { get; set; } = null!;

    public virtual NhanVien MaNvChiNavigation { get; set; } = null!;
}
