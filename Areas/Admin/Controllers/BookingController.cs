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
    };

    private static readonly List<ChiTietKhachDiTour> MockMemberList = new()
    {
        new ChiTietKhachDiTour { MaDangKy = 1, MaKh = 1, LaNguoiDaiDien = true },
        new ChiTietKhachDiTour { MaDangKy = 1, MaKh = 2, LaNguoiDaiDien = false },
        new ChiTietKhachDiTour { MaDangKy = 2, MaKh = 2, LaNguoiDaiDien = true }
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

    // GET: Admin/Booking/Members
    public async Task<IActionResult> Members(int bookingId)
    {
        var model = new BookingMembersVM { MaDangKy = bookingId };
        
        try
        {
            var booking = await _context.KhachDiTours
                .Include(k => k.MaKhNavigation)
                .Include(k => k.MaDoanNavigation)
                .FirstOrDefaultAsync(k => k.MaDangKy == bookingId);

            if (booking != null)
            {
                model.TenKhachHangDaiDien = booking.MaKhNavigation.HoTen;
                model.TenDoan = booking.MaDoanNavigation.TenDoan;
                model.SoNguoiDi = booking.SoNguoiDi;

                var dbMembers = await _context.ChiTietKhachDiTours
                    .Include(c => c.MaKhNavigation)
                    .Where(c => c.MaDangKy == bookingId)
                    .Select(c => new MemberInfoVM
                    {
                        MaKh = c.MaKh,
                        HoTen = c.MaKhNavigation.HoTen,
                        Sdt = c.MaKhNavigation.Sdt,
                        Email = c.MaKhNavigation.Email ?? "N/A",
                        Cccd = c.MaKhNavigation.Cccd ?? "N/A",
                        LaNguoiDaiDien = c.LaNguoiDaiDien
                    })
                    .ToListAsync();
                
                model.Members = dbMembers;
            }
        }
        catch
        {
            // Fallback mock
            var mockBooking = MockBookingList.FirstOrDefault(b => b.MaDangKy == bookingId);
            if (mockBooking != null)
            {
                model.TenKhachHangDaiDien = mockBooking.TenKhachHang;
                model.TenDoan = mockBooking.TenDoan;
                model.SoNguoiDi = mockBooking.SoNguoiDi;

                model.Members = MockMemberList
                    .Where(m => m.MaDangKy == bookingId)
                    .Select(m => new MemberInfoVM
                    {
                        MaKh = m.MaKh,
                        HoTen = m.MaKh == 1 ? "Nguyễn Văn Anh" : (m.MaKh == 2 ? "Trần Thị Bé" : "Phạm Minh Hoàng"),
                        Sdt = m.MaKh == 1 ? "0987654321" : (m.MaKh == 2 ? "0912345678" : "0905556667"),
                        Email = m.MaKh == 1 ? "anhanh@gmail.com" : "betran@gmail.com",
                        Cccd = m.MaKh == 1 ? "123456789" : "987654321",
                        LaNguoiDaiDien = m.LaNguoiDaiDien
                    })
                    .ToList();
            }
        }

        // Tải danh sách khách hàng để chọn thêm vào
        try
        {
            var existingMemberIds = model.Members.Select(m => m.MaKh).ToList();
            var dbKhach = await _context.KhachHangs
                .Where(k => !existingMemberIds.Contains(k.MaKh))
                .ToListAsync();

            model.CustomerSelectList = dbKhach.Select(k => new SelectListItem
            {
                Value = k.MaKh.ToString(),
                Text = $"{k.HoTen} (CCCD: {k.Cccd} - SĐT: {k.Sdt})"
            }).ToList();
        }
        catch
        {
            // Mock select list
            var existingMemberIds = model.Members.Select(m => m.MaKh).ToList();
            var mockCustomers = new List<(int Id, string Name, string Cccd, string Sdt)>
            {
                (1, "Nguyễn Văn Anh", "123456789", "0987654321"),
                (2, "Trần Thị Bé", "987654321", "0912345678"),
                (3, "Phạm Minh Hoàng", "456789123", "0905556667"),
                (4, "Đỗ Thị Quỳnh", "789123456", "0933445566")
            };

            model.CustomerSelectList = mockCustomers
                .Where(c => !existingMemberIds.Contains(c.Id))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} (CCCD: {c.Cccd} - SĐT: {c.Sdt})"
                }).ToList();
        }

        return View(model);
    }

    // POST: Admin/Booking/AddMember
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMember(int bookingId, int newMaKh)
    {
        try
        {
            var exists = await _context.ChiTietKhachDiTours.AnyAsync(c => c.MaDangKy == bookingId && c.MaKh == newMaKh);
            if (!exists)
            {
                // Kiểm tra xem đã có người đại diện chưa, nếu chưa có thì người này làm đại diện
                var hasRep = await _context.ChiTietKhachDiTours.AnyAsync(c => c.MaDangKy == bookingId && c.LaNguoiDaiDien);

                var newMember = new ChiTietKhachDiTour
                {
                    MaDangKy = bookingId,
                    MaKh = newMaKh,
                    LaNguoiDaiDien = !hasRep
                };

                _context.ChiTietKhachDiTours.Add(newMember);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm thành viên đoàn thành công!";
            }
        }
        catch
        {
            // Fallback mock
            var exists = MockMemberList.Any(c => c.MaDangKy == bookingId && c.MaKh == newMaKh);
            if (!exists)
            {
                var hasRep = MockMemberList.Any(c => c.MaDangKy == bookingId && c.LaNguoiDaiDien);
                MockMemberList.Add(new ChiTietKhachDiTour
                {
                    MaDangKy = bookingId,
                    MaKh = newMaKh,
                    LaNguoiDaiDien = !hasRep
                });
                TempData["SuccessMessage"] = "Thêm thành viên đoàn giả lập thành công (Offline)!";
            }
        }

        return RedirectToAction(nameof(Members), new { bookingId = bookingId });
    }

    // POST: Admin/Booking/RemoveMember
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveMember(int bookingId, int maKh)
    {
        try
        {
            var member = await _context.ChiTietKhachDiTours.FirstOrDefaultAsync(c => c.MaDangKy == bookingId && c.MaKh == maKh);
            if (member != null)
            {
                _context.ChiTietKhachDiTours.Remove(member);
                await _context.SaveChangesAsync();

                // Nếu người bị xóa là người đại diện, tự động set người khác làm đại diện (nếu còn)
                if (member.LaNguoiDaiDien)
                {
                    var otherMember = await _context.ChiTietKhachDiTours.FirstOrDefaultAsync(c => c.MaDangKy == bookingId);
                    if (otherMember != null)
                    {
                        otherMember.LaNguoiDaiDien = true;
                        await _context.SaveChangesAsync();
                    }
                }
                TempData["SuccessMessage"] = "Xóa thành viên khỏi đoàn thành công!";
            }
        }
        catch
        {
            // Fallback mock
            var member = MockMemberList.FirstOrDefault(c => c.MaDangKy == bookingId && c.MaKh == maKh);
            if (member != null)
            {
                MockMemberList.Remove(member);
                if (member.LaNguoiDaiDien)
                {
                    var otherMember = MockMemberList.FirstOrDefault(c => c.MaDangKy == bookingId);
                    if (otherMember != null)
                    {
                        otherMember.LaNguoiDaiDien = true;
                    }
                }
                TempData["SuccessMessage"] = "Xóa thành viên đoàn giả lập thành công (Offline)!";
            }
        }

        return RedirectToAction(nameof(Members), new { bookingId = bookingId });
    }

    // POST: Admin/Booking/SetRepresentative
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetRepresentative(int bookingId, int maKh)
    {
        try
        {
            var members = await _context.ChiTietKhachDiTours.Where(c => c.MaDangKy == bookingId).ToListAsync();
            foreach (var m in members)
            {
                m.LaNguoiDaiDien = (m.MaKh == maKh);
            }
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật người đại diện thành công!";
        }
        catch
        {
            // Fallback mock
            var members = MockMemberList.Where(c => c.MaDangKy == bookingId).ToList();
            foreach (var m in members)
            {
                m.LaNguoiDaiDien = (m.MaKh == maKh);
            }
            TempData["SuccessMessage"] = "Cập nhật người đại diện giả lập thành công (Offline)!";
        }

        return RedirectToAction(nameof(Members), new { bookingId = bookingId });
    }
}
