using System;

namespace QuanLyTourDuLich.ViewModels;

public class ChiPhiDoanListVM
{
    public int MaChiPhi { get; set; }
    public string TenDoan { get; set; } = null!;
    public string LoaiChiPhi { get; set; } = null!;
    public string NoiDungChi { get; set; } = null!;
    public decimal SoTien { get; set; }
    public DateTime NgayChi { get; set; }
    public string TenNhanVienLap { get; set; } = null!;
}
