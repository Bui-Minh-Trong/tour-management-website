using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTourDuLich.Models;
using QuanLyTourDuLich.Services;
using QuanLyTourDuLich.ViewModels;

namespace QuanLyTourDuLich.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]/[action]/{id?}")]
[Authorize]
public class BookingController : Controller
{
    private readonly QlyTourDuLichContext _context;
    private readonly ITourBookingService _service;

    public BookingController(QlyTourDuLichContext context, ITourBookingService service)
    {
        _context = context;
        _service = service;
    }

    // Mock Data List for testing view layout
    private static readonly List<BookingListVM> MockBookingList = new()
    {
        new BookingListVM
        {
            MaDangKy = 1,
            TenKhachHang = "Nguyễn Văn Anh",
            TenDoan = "Đoàn Du Lịch Phú Quốc Hè 2026",
            SoNguoiDi = 4,
            NgayDangKy = DateTime.Now.AddDays(-5),
            DonGia = 4500000,
            SoTienGiam = 200000,
            TrangThaiThanhToan = "Đã thanh toán"
        },
        new BookingListVM
        {
            MaDangKy = 2,
            TenKhachHang = "Trần Thị Bé",
            TenDoan = "Đoàn Du Lịch Đà Lạt Ngàn Hoa",
            SoNguoiDi = 2,
            NgayDangKy = DateTime.Now.AddDays(-3),
            DonGia = 3200000,
            SoTienGiam = 0,
            TrangThaiThanhToan = "Đã cọc"
        },
        new BookingListVM
        {
            MaDangKy = 3,
            TenKhachHang = "Phạm Minh Hoàng",
            TenDoan = "Đoàn Du Lịch Phú Quốc Hè 2026",
            SoNguoiDi = 5,
            NgayDangKy = DateTime.Now.AddDays(-1),
            DonGia = 4500000,
            SoTienGiam = 500000,
            TrangThaiThanhToan = "Chưa thanh toán"
        }
    };



    // GET: Admin/Booking
    [Route("~/Admin/Booking")]
    [Route("~/Admin/Booking/Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var hasData = await _context.KhachDiTours.AnyAsync();
            if (hasData)
            {
                var list = await _context.KhachDiTours
                    .Include(k => k.MaKhNavigation)
                    .Include(k => k.MaDoanNavigation)
                    .Select(k => new BookingListVM
                    {
                        MaDangKy = k.MaDangKy,
                        TenKhachHang = k.MaKhNavigation.HoTen,
                        TenDoan = k.MaDoanNavigation.TenDoan,
                        SoNguoiDi = k.SoNguoiDi,
                        NgayDangKy = k.NgayDangKy,
                        DonGia = k.DonGia,
                        SoTienGiam = k.SoTienGiam ?? 0m,
                        TrangThaiThanhToan = k.TrangThaiThanhToan
                    })
                    .ToListAsync();
                return View(list);
            }
        }
        catch
        {
            // Fallback to mock data if database is empty/offline
        }

        return View(MockBookingList);
    }

    // GET: Admin/Booking/Create
    public async Task<IActionResult> Create()
    {
        var model = new BookingFormVM
        {
            SoNguoiDi = 1,
            DonGia = 3000000
        };

        await PopulateSelectLists(model);
        return View(model);
    }

    // POST: Admin/Booking/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingFormVM model)
    {
        // BƯỚC 1 (Nhận Request): Action Create [HttpPost] nhận BookingFormVM từ UI. Kiểm tra ModelState.IsValid.
        if (!ModelState.IsValid)
        {
            await PopulateSelectLists(model);
            return View(model);
        }

        var result = await _service.CreateBookingAsync(model);
        if (result.isSuccess)
        {
            TempData["SuccessMessage"] = result.message;
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", result.message);
        await PopulateSelectLists(model);
        return View(model);
    }

    // POST: Admin/Booking/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var booking = await _context.KhachDiTours.Include(k => k.MaDoanNavigation).FirstOrDefaultAsync(k => k.MaDangKy == id);
            if (booking != null)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    // Trả lại số chỗ đã đăng ký
                    if (booking.MaDoanNavigation != null)
                    {
                        booking.MaDoanNavigation.SoKhachHienTai = Math.Max(0, booking.MaDoanNavigation.SoKhachHienTai - booking.SoNguoiDi);
                    }

                    _context.KhachDiTours.Remove(booking);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                TempData["SuccessMessage"] = "Hủy đặt tour và hoàn lại số chỗ thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch
        {
            // Bỏ qua lỗi
        }

        // Xóa trong mock list
        var mockItem = MockBookingList.FirstOrDefault(m => m.MaDangKy == id);
        if (mockItem != null)
        {
            MockBookingList.Remove(mockItem);
            TempData["SuccessMessage"] = "Hủy đặt tour giả lập thành công (Offline)!";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSelectLists(BookingFormVM model)
    {
        List<SelectListItem> customersList = new();
        List<SelectListItem> groupsList = new();
        List<SelectListItem> saleStaffsList = new();

        try
        {
            var dbKhach = await _context.KhachHangs.ToListAsync();
            customersList = dbKhach.Select(k => new SelectListItem
            {
                Value = k.MaKh.ToString(),
                Text = $"{k.HoTen} (SĐT: {k.Sdt} - CCCD: {k.Cccd})"
            }).ToList();

            var dbDoan = await _context.DoanDuLiches.Include(d => d.MaTourNavigation).ToListAsync();
            groupsList = dbDoan.Select(d => new SelectListItem
            {
                Value = d.MaDoan.ToString(),
                Text = $"{d.TenDoan} [Chỗ: {d.SoKhachHienTai}/{d.SoLuongToiDa}]"
            }).ToList();

            // Tải danh sách Nhân viên Kinh doanh (SaleID trong Class Diagram)
            var dbSales = await _context.NhanViens
                .Where(n => n.ChucVu.Contains("Kinh doanh") || n.ChucVu.Contains("Sales"))
                .ToListAsync();
            if (!dbSales.Any())
            {
                dbSales = await _context.NhanViens.ToListAsync();
            }
            saleStaffsList = dbSales.Select(s => new SelectListItem
            {
                Value = s.MaNv.ToString(),
                Text = $"{s.HoTen} ({s.ChucVu})"
            }).ToList();
        }
        catch
        {
            // Bỏ qua lỗi
        }

        if (!customersList.Any())
        {
            customersList.Add(new SelectListItem { Value = "1", Text = "Nguyễn Văn Anh (SĐT: 0987654321)" });
            customersList.Add(new SelectListItem { Value = "2", Text = "Trần Thị Bé (SĐT: 0912345678)" });
            customersList.Add(new SelectListItem { Value = "3", Text = "Phạm Minh Hoàng (SĐT: 0905556667)" });
        }

        if (!groupsList.Any())
        {
            groupsList.Add(new SelectListItem { Value = "1", Text = "Đoàn Du Lịch Phú Quốc Hè 2026 [25/40 chỗ]" });
            groupsList.Add(new SelectListItem { Value = "2", Text = "Đoàn Du Lịch Đà Lạt Ngàn Hoa [30/30 chỗ]" });
            groupsList.Add(new SelectListItem { Value = "3", Text = "Đoàn Khám Phá Tây Bắc Vĩ Đại [12/35 chỗ]" });
        }

        if (!saleStaffsList.Any())
        {
            saleStaffsList.Add(new SelectListItem { Value = "1", Text = "Lê Văn Đức (Nhân viên Kinh doanh)" });
            saleStaffsList.Add(new SelectListItem { Value = "2", Text = "Nguyễn Thị Lan (Nhân viên Kinh doanh)" });
        }

        model.Customers = customersList;
        model.TourGroups = groupsList;
        model.SaleStaffs = saleStaffsList;
    }
}
