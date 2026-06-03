using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyTourDuLich.Models;
using QuanLyTourDuLich.ViewModels;

namespace QuanLyTourDuLich.Services;

public class TourBookingService : ITourBookingService
{
    private readonly QlyTourDuLichContext _context;

    public TourBookingService(QlyTourDuLichContext context)
    {
        _context = context;
    }

    public async Task<bool> IsTourNameDuplicateAsync(string name, int? excludeTourId = null)
    {
        if (excludeTourId.HasValue)
        {
            return await _context.Tours.AnyAsync(t => t.TenTour.ToLower() == name.ToLower() && t.MaTour != excludeTourId.Value);
        }
        return await _context.Tours.AnyAsync(t => t.TenTour.ToLower() == name.ToLower());
    }

    public async Task<bool> IsGuideOverlappedAsync(int guideId, DateOnly start, DateOnly end, int? excludeGroupId = null)
    {
        if (excludeGroupId.HasValue)
        {
            return await _context.DoanDuLiches.AnyAsync(d =>
                d.MaNvHuongDanVien == guideId &&
                d.MaDoan != excludeGroupId.Value &&
                d.NgayKhoiHanh <= end &&
                d.NgayKetThuc >= start);
        }
        return await _context.DoanDuLiches.AnyAsync(d =>
            d.MaNvHuongDanVien == guideId &&
            d.NgayKhoiHanh <= end &&
            d.NgayKetThuc >= start);
    }

    public async Task<(bool isSuccess, string message)> CreateBookingAsync(BookingFormVM model)
    {
        var doan = await _context.DoanDuLiches.FirstOrDefaultAsync(d => d.MaDoan == model.MaDoan);
        if (doan == null)
        {
            return (false, "Lỗi: Đoàn du lịch không tồn tại trong hệ thống.");
        }

        int soChoConLai = doan.SoLuongToiDa - doan.SoKhachHienTai;
        if (model.SoNguoiDi > soChoConLai)
        {
            return (false, $"Lỗi: Đoàn này chỉ còn trống {soChoConLai} chỗ, không đủ cho yêu cầu {model.SoNguoiDi} người.");
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                decimal reduction = 0;
                int? promoId = null;
                if (!string.IsNullOrEmpty(model.CodeKhuyenMai))
                {
                    var promo = await _context.KhuyenMais.FirstOrDefaultAsync(p => p.Code == model.CodeKhuyenMai.Trim());
                    if (promo != null)
                    {
                        if (promo.NgayHetHan >= DateOnly.FromDateTime(DateTime.Today) && promo.NgayBatDau <= DateOnly.FromDateTime(DateTime.Today))
                        {
                            promoId = promo.MaKm;
                            if (promo.TienGiamTrucTiep > 0)
                            {
                                reduction = promo.TienGiamTrucTiep ?? 0;
                            }
                            else if (promo.PhanTramGiam > 0)
                            {
                                reduction = (model.DonGia * model.SoNguoiDi) * ((promo.PhanTramGiam ?? 0m) / 100m);
                            }
                            promo.SoLuongDaDung = (promo.SoLuongDaDung ?? 0) + 1;
                        }
                    }
                }

                var khachDi = new KhachDiTour
                {
                    MaDoan = model.MaDoan,
                    MaKh = model.MaKh,
                    MaKm = promoId,
                    // MaNvKinhDoanh: Lưu nhân viên kinh doanh lập phiếu (SaleID trong Class Diagram)
                    MaNvKinhDoanh = model.MaNvKinhDoanh,
                    NgayDangKy = DateTime.Now,
                    SoNguoiDi = model.SoNguoiDi,
                    DonGia = model.DonGia,
                    SoTienGiam = reduction,
                    TrangThaiThanhToan = model.TrangThaiThanhToan,
                    GhiChu = model.GhiChu
                };

                _context.KhachDiTours.Add(khachDi);
                await _context.SaveChangesAsync();

                doan.SoKhachHienTai += model.SoNguoiDi;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, "Đăng ký đặt tour thành công!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Đã xảy ra lỗi hệ thống trong quá trình thực hiện giao dịch đặt tour: " + ex.Message);
            }
        }
    }

    public async Task<(bool isSuccess, string message)> CreateInvoiceAsync(HoaDonFormVM model)
    {
        var booking = await _context.KhachDiTours.FindAsync(model.MaDangKy);
        if (booking == null)
        {
            return (false, "Đăng ký đặt tour không tồn tại.");
        }

        decimal totalCost = (booking.SoNguoiDi * booking.DonGia) - (booking.SoTienGiam ?? 0m);
        decimal paidAmount = await _context.HoaDons
            .Where(h => h.MaDangKy == model.MaDangKy)
            .SumAsync(h => h.SoTien);

        decimal remaining = totalCost - paidAmount;

        if (model.SoTien > remaining)
        {
            return (false, $"Số tiền thu vượt quá số dư cần thanh toán. Số dư còn lại: {remaining.ToString("N0")} VND.");
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var hd = new HoaDon
                {
                    MaDangKy = model.MaDangKy,
                    NgayLap = DateTime.Now,
                    SoTien = model.SoTien,
                    LoaiHoaDon = model.LoaiHoaDon,
                    HinhThucTt = model.HinhThucTt,
                    // GhiChu: Tương ứng với Note trong Class Diagram lớp PhieuThu
                    GhiChu = model.GhiChu,
                    MaNvLap = model.MaNvLap
                };

                _context.HoaDons.Add(hd);
                await _context.SaveChangesAsync();

                decimal newPaidAmount = paidAmount + model.SoTien;
                if (newPaidAmount >= totalCost)
                {
                    booking.TrangThaiThanhToan = "Đã thanh toán";
                }
                else if (newPaidAmount > 0)
                {
                    booking.TrangThaiThanhToan = "Đã cọc";
                }
                else
                {
                    booking.TrangThaiThanhToan = "Chưa thanh toán";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (true, "Lập hóa đơn và thu tiền thành công!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, "Đã xảy ra lỗi hệ thống khi lập hóa đơn: " + ex.Message);
            }
        }
    }

    public async Task<DashboardViewModel> GetDashboardStatsAsync()
    {
        var totalTours = await _context.Tours.CountAsync();
        var totalRevenue = await _context.HoaDons.SumAsync(h => (decimal?)h.SoTien) ?? 0m;
        var totalGuests = await _context.KhachDiTours.SumAsync(k => (int?)k.SoNguoiDi) ?? 0;
        var totalGroups = await _context.DoanDuLiches.CountAsync();

        var topTours = await _context.Tours
            .Select(t => new TourStatVM
            {
                TenTour = t.TenTour,
                SoKhachDat = t.DoanDuLiches.SelectMany(d => d.KhachDiTours).Sum(k => (int?)k.SoNguoiDi) ?? 0
            })
            .OrderByDescending(x => x.SoKhachDat)
            .Take(5)
            .ToListAsync();

        return new DashboardViewModel
        {
            TongSoTour = totalTours,
            TongDoanhThu = totalRevenue,
            TongLuotKhach = totalGuests,
            TongSoDoan = totalGroups,
            TopTours = topTours
        };
    }
}
