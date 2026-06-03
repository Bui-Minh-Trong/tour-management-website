using System;
using System.Collections.Generic;

namespace QuanLyTourDuLich.ViewModels;

public class DoanSettlementVM
{
    public int MaDoan { get; set; }
    public string TenDoan { get; set; } = "";
    public string TenTour { get; set; } = "";
    public DateOnly NgayKhoiHanh { get; set; }
    public DateOnly NgayKetThuc { get; set; }
    public string TenHuongDanVien { get; set; } = "";
    public int SoKhachHienTai { get; set; }

    public decimal TongThu { get; set; }
    public decimal TongChi { get; set; }
    public decimal LoiNhuan => TongThu - TongChi;

    public List<SettlementIncomeVM> Incomes { get; set; } = new();
    public List<SettlementExpenseVM> Expenses { get; set; } = new();

    public string TrangThaiQuyetToan { get; set; } = "Chưa quyết toán";
}

public class SettlementIncomeVM
{
    public int MaHoaDon { get; set; }
    public string TenKhachHang { get; set; } = "";
    public DateTime NgayLap { get; set; }
    public decimal SoTien { get; set; }
    public string HinhThucTt { get; set; } = "";
}

public class SettlementExpenseVM
{
    public int MaChiPhi { get; set; }
    public string LoaiChiPhi { get; set; } = "";
    public decimal SoTien { get; set; }
    public string GhiChu { get; set; } = "";
}
