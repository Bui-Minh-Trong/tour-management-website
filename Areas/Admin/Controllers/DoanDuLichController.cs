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
public class DoanDuLichController : Controller
{
    private readonly QlyTourDuLichContext _context;

    // Danh sách giả lập (Mock Data) để kiểm thử giao diện
    private static readonly List<DoanDuLichListVM> MockDoanList = new()
    {
        new DoanDuLichListVM
        {
            MaDoan = 1,
            TenDoan = "Đoàn Du Lịch Phú Quốc Hè 2026",
            TenTour = "Phú Quốc Đảo Ngọc 3 Ngày 2 Đêm",
            NgayKhoiHanh = DateOnly.FromDateTime(DateTime.Today.AddDays(15)),
            NgayKetThuc = DateOnly.FromDateTime(DateTime.Today.AddDays(18)),
            TenHuongDanVien = "Nguyễn Văn Hùng",
            SoLuongToiDa = 40,
            SoKhachHienTai = 25,
            TrangThaiDoan = "Đang nhận khách"
        },
        new DoanDuLichListVM
        {
            MaDoan = 2,
            TenDoan = "Đoàn Du Lịch Đà Lạt Ngàn Hoa",
            TenTour = "Hành Trình Khám Phá Đà Lạt Mộng Mơ",
            NgayKhoiHanh = DateOnly.FromDateTime(DateTime.Today.AddDays(25)),
            NgayKetThuc = DateOnly.FromDateTime(DateTime.Today.AddDays(29)),
            TenHuongDanVien = "Lê Thị Thu Thảo",
            SoLuongToiDa = 30,
            SoKhachHienTai = 30,
            TrangThaiDoan = "Đủ khách"
        },
        new DoanDuLichListVM
        {
            MaDoan = 3,
            TenDoan = "Đoàn Khám Phá Tây Bắc Vĩ Đại",
            TenTour = "Vòng Cung Tây Bắc - Mùa Lúa Chín Sapa",
            NgayKhoiHanh = DateOnly.FromDateTime(DateTime.Today.AddDays(40)),
            NgayKetThuc = DateOnly.FromDateTime(DateTime.Today.AddDays(45)),
            TenHuongDanVien = "Trần Thanh Sơn",
            SoLuongToiDa = 35,
            SoKhachHienTai = 12,
            TrangThaiDoan = "Lên kế hoạch"
        },
        new DoanDuLichListVM
        {
            MaDoan = 4,
            TenDoan = "Đoàn Hành Trình Di Sản Miền Trung",
            TenTour = "Đà Nẵng - Hội An - Huế Hành Trình Di Sản",
            NgayKhoiHanh = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            NgayKetThuc = DateOnly.FromDateTime(DateTime.Today.AddDays(-6)),
            TenHuongDanVien = "Phạm Hoàng Long",
            SoLuongToiDa = 30,
            SoKhachHienTai = 28,
            TrangThaiDoan = "Đã kết thúc"
        }
    };

    private readonly ITourBookingService _service;

    public DoanDuLichController(QlyTourDuLichContext context, ITourBookingService service)
    {
        _context = context;
        _service = service;
    }

    // GET: Admin/DoanDuLich or Admin/DoanDuLich/Index
    [Route("~/Admin/DoanDuLich")]
    [Route("~/Admin/DoanDuLich/Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            // Cố gắng đọc từ cơ sở dữ liệu trước
            var hasData = await _context.DoanDuLiches.AnyAsync();
            if (hasData)
            {
                var list = await _context.DoanDuLiches
                    .Include(d => d.MaTourNavigation)
                    .Include(d => d.MaNvHuongDanVienNavigation)
                    .Select(d => new DoanDuLichListVM
                    {
                        MaDoan = d.MaDoan,
                        TenDoan = d.TenDoan,
                        TenTour = d.MaTourNavigation.TenTour,
                        NgayKhoiHanh = d.NgayKhoiHanh,
                        NgayKetThuc = d.NgayKetThuc,
                        TenHuongDanVien = d.MaNvHuongDanVienNavigation.HoTen,
                        SoLuongToiDa = d.SoLuongToiDa,
                        SoKhachHienTai = d.SoKhachHienTai,
                        TrangThaiDoan = d.TrangThaiDoan
                    })
                    .ToListAsync();
                return View(list);
            }
        }
        catch
        {
            // Bỏ qua lỗi kết nối database để hiển thị mock data cho kiểm thử giao diện
        }

        return View(MockDoanList);
    }

    // GET: Admin/DoanDuLich/Create
    public async Task<IActionResult> Create()
    {
        var model = new DoanDuLichFormVM
        {
            NgayKhoiHanh = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            NgayKetThuc = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            SoLuongToiDa = 30
        };

        await PopulateSelectLists(model);
        return View(model);
    }

    // POST: Admin/DoanDuLich/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DoanDuLichFormVM model)
    {
        // BƯỚC 1 (Nhận Request): Action Create [HttpPost] nhận dữ liệu DoanDuLichFormVM từ UI.

        // BƯỚC 2 (Validation Cơ bản):
        if (model.NgayKetThuc < model.NgayKhoiHanh)
        {
            ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải lớn hơn hoặc bằng ngày khởi hành.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateSelectLists(model);
            return View(model);
        }

        try
        {
            // BƯỚC 3: KIỂM TRA TRÙNG LỊCH (OVERLAP CHECK)
            bool isOverlapped = await _service.IsGuideOverlappedAsync(model.MaNvHuongDanVien, model.NgayKhoiHanh, model.NgayKetThuc);
            var conflictingDoan = isOverlapped 
                ? await _context.DoanDuLiches.FirstOrDefaultAsync(d =>
                    d.MaNvHuongDanVien == model.MaNvHuongDanVien &&
                    d.NgayKhoiHanh <= model.NgayKetThuc &&
                    d.NgayKetThuc >= model.NgayKhoiHanh)
                : null;

            // BƯỚC 4 (Rẽ nhánh ALT):
            // [NHÁNH BỊ CHẶN - SCHEDULE CONFLICT]
            if (conflictingDoan != null)
            {
                ModelState.AddModelError("MaNvHuongDanVien", 
                    $"Lỗi: Hướng dẫn viên này đã được phân công cho một đoàn khác trong khoảng thời gian từ {conflictingDoan.NgayKhoiHanh:dd/MM/yyyy} đến {conflictingDoan.NgayKetThuc:dd/MM/yyyy}. Vui lòng chọn người khác!");
                await PopulateSelectLists(model);
                return View(model);
            }

            // BƯỚC 5 (Xử lý Thành công):
            // [NHÁNH CHO PHÉP]
            var doan = new DoanDuLich
            {
                TenDoan = model.TenDoan.Trim(),
                MaTour = model.MaTour,
                NgayKhoiHanh = model.NgayKhoiHanh,
                NgayKetThuc = model.NgayKetThuc,
                MaNvHuongDanVien = model.MaNvHuongDanVien,
                // MaNvDieuHanh: Tương ứng với OperatorID trong Class Diagram
                MaNvDieuHanh = model.MaNvDieuHanh,
                SoLuongToiDa = model.SoLuongToiDa,
                SoKhachHienTai = 0,
                TrangThaiDoan = model.TrangThaiDoan
            };

            _context.DoanDuLiches.Add(doan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm mới đoàn du lịch thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            // Trình diễn mock data hoạt động:
            TempData["SuccessMessage"] = "Thêm mới đoàn giả lập thành công (Chế độ offline)!";
            
            // Lưu tạm vào mock list để thấy thay đổi khi load
            MockDoanList.Add(new DoanDuLichListVM
            {
                MaDoan = MockDoanList.Max(m => m.MaDoan) + 1,
                TenDoan = model.TenDoan,
                TenTour = "Tour mẫu đã chọn #" + model.MaTour,
                NgayKhoiHanh = model.NgayKhoiHanh,
                NgayKetThuc = model.NgayKetThuc,
                TenHuongDanVien = "Hướng dẫn viên #" + model.MaNvHuongDanVien,
                SoLuongToiDa = model.SoLuongToiDa,
                SoKhachHienTai = 0,
                TrangThaiDoan = model.TrangThaiDoan
            });

            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Admin/DoanDuLich/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        DoanDuLichFormVM? model = null;

        try
        {
            var d = await _context.DoanDuLiches.FindAsync(id);
            if (d != null)
            {
                model = new DoanDuLichFormVM
                {
                    MaDoan = d.MaDoan,
                    TenDoan = d.TenDoan,
                    MaTour = d.MaTour,
                    NgayKhoiHanh = d.NgayKhoiHanh,
                    NgayKetThuc = d.NgayKetThuc,
                    MaNvHuongDanVien = d.MaNvHuongDanVien,
                    // MaNvDieuHanh: Tương ứng OperatorID trong Class Diagram
                    MaNvDieuHanh = d.MaNvDieuHanh,
                    SoLuongToiDa = d.SoLuongToiDa,
                    SoKhachHienTai = d.SoKhachHienTai,
                    TrangThaiDoan = d.TrangThaiDoan
                };
            }
        }
        catch
        {
            // Bỏ qua lỗi
        }

        // Nếu DB không có/lỗi, tìm trong mock data
        if (model == null)
        {
            var mockItem = MockDoanList.FirstOrDefault(m => m.MaDoan == id);
            if (mockItem == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đoàn du lịch có mã #" + id;
                return RedirectToAction(nameof(Index));
            }

            model = new DoanDuLichFormVM
            {
                MaDoan = mockItem.MaDoan,
                TenDoan = mockItem.TenDoan,
                MaTour = 1,
                NgayKhoiHanh = mockItem.NgayKhoiHanh,
                NgayKetThuc = mockItem.NgayKetThuc,
                MaNvHuongDanVien = 1,
                SoLuongToiDa = mockItem.SoLuongToiDa,
                SoKhachHienTai = mockItem.SoKhachHienTai,
                TrangThaiDoan = mockItem.TrangThaiDoan
            };
        }

        await PopulateSelectLists(model);
        return View(model);
    }

    // POST: Admin/DoanDuLich/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DoanDuLichFormVM model)
    {
        // BƯỚC 1 (Nhận Request): Action Edit [HttpPost] nhận dữ liệu DoanDuLichFormVM và id từ UI.
        if (id != model.MaDoan) return BadRequest();

        // BƯỚC 2 (Validation Cơ bản):
        if (model.NgayKetThuc < model.NgayKhoiHanh)
        {
            ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải lớn hơn hoặc bằng ngày khởi hành.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateSelectLists(model);
            return View(model);
        }

        try
        {
            // BƯỚC 3: KIỂM TRA TRÙNG LỊCH (OVERLAP CHECK)
            // Thuật toán: (A.NgayKhoiHanh <= B.NgayKetThuc) AND (A.NgayKetThuc >= B.NgayKhoiHanh) và loại trừ chính đoàn này
            // BƯỚC 3: KIỂM TRA TRÙNG LỊCH (OVERLAP CHECK)
            bool isOverlapped = await _service.IsGuideOverlappedAsync(model.MaNvHuongDanVien, model.NgayKhoiHanh, model.NgayKetThuc, id);
            var conflictingDoan = isOverlapped 
                ? await _context.DoanDuLiches.FirstOrDefaultAsync(d =>
                    d.MaDoan != id &&
                    d.MaNvHuongDanVien == model.MaNvHuongDanVien &&
                    d.NgayKhoiHanh <= model.NgayKetThuc &&
                    d.NgayKetThuc >= model.NgayKhoiHanh)
                : null;

            // BƯỚC 4 (Rẽ nhánh ALT):
            // [NHÁNH BỊ CHẶN - SCHEDULE CONFLICT]
            if (conflictingDoan != null)
            {
                ModelState.AddModelError("MaNvHuongDanVien", 
                    $"Lỗi: Hướng dẫn viên này đã được phân công cho một đoàn khác trong khoảng thời gian từ {conflictingDoan.NgayKhoiHanh:dd/MM/yyyy} đến {conflictingDoan.NgayKetThuc:dd/MM/yyyy}. Vui lòng chọn người khác!");
                await PopulateSelectLists(model);
                return View(model);
            }

            // BƯỚC 5 (Xử lý Thành công):
            // [NHÁNH CHO PHÉP]
            var d = await _context.DoanDuLiches.FindAsync(id);
            if (d != null)
            {
                d.TenDoan = model.TenDoan.Trim();
                d.MaTour = model.MaTour;
                d.NgayKhoiHanh = model.NgayKhoiHanh;
                d.NgayKetThuc = model.NgayKetThuc;
                d.MaNvHuongDanVien = model.MaNvHuongDanVien;
                // MaNvDieuHanh: Tương ứng với OperatorID trong Class Diagram
                d.MaNvDieuHanh = model.MaNvDieuHanh;
                d.SoLuongToiDa = model.SoLuongToiDa;
                d.TrangThaiDoan = model.TrangThaiDoan;

                _context.Entry(d).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật đoàn du lịch thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception)
        {
            // Bỏ qua lỗi, thực hiện cập nhật mock list
        }

        // Cập nhật mock list
        var mockIndex = MockDoanList.FindIndex(m => m.MaDoan == id);
        if (mockIndex >= 0)
        {
            MockDoanList[mockIndex].TenDoan = model.TenDoan;
            MockDoanList[mockIndex].NgayKhoiHanh = model.NgayKhoiHanh;
            MockDoanList[mockIndex].NgayKetThuc = model.NgayKetThuc;
            MockDoanList[mockIndex].SoLuongToiDa = model.SoLuongToiDa;
            MockDoanList[mockIndex].TrangThaiDoan = model.TrangThaiDoan;
            TempData["SuccessMessage"] = "Cập nhật đoàn du lịch giả lập thành công (Offline)!";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/DoanDuLich/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var d = await _context.DoanDuLiches.FindAsync(id);
            if (d != null)
            {
                // Kiểm tra ràng buộc: xem đã có Khách đi tour đăng ký chưa
                bool hasBooking = await _context.KhachDiTours.AnyAsync(b => b.MaDoan == id);
                if (hasBooking)
                {
                    TempData["ErrorMessage"] = "Không thể xóa đoàn vì đã có khách hàng đặt tour.";
                    return RedirectToAction(nameof(Index));
                }

                _context.DoanDuLiches.Remove(d);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa đoàn thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch
        {
            // Bỏ qua lỗi
        }

        // Xóa trong mock list
        var mockItem = MockDoanList.FirstOrDefault(m => m.MaDoan == id);
        if (mockItem != null)
        {
            if (mockItem.SoKhachHienTai > 0)
            {
                TempData["ErrorMessage"] = "Không thể xóa đoàn vì đã có khách hàng đặt tour (Số khách hiện tại > 0).";
            }
            else
            {
                MockDoanList.Remove(mockItem);
                TempData["SuccessMessage"] = "Xóa đoàn giả lập thành công (Offline)!";
            }
        }

        return RedirectToAction(nameof(Index));
    }

    // Hỗ trợ điền Dropdown list bằng dữ liệu DB hoặc Mock Data nếu DB trống
    private async Task PopulateSelectLists(DoanDuLichFormVM model)
    {
        List<SelectListItem> toursList = new();
        List<SelectListItem> guidesList = new();
        List<SelectListItem> operatorsList = new();

        try
        {
            var dbTours = await _context.Tours.ToListAsync();
            toursList = dbTours.Select(t => new SelectListItem 
            { 
                Value = t.MaTour.ToString(), 
                Text = $"{t.TenTour} ({t.DiaDiem} - {t.ThoiGian} ngày)" 
            }).ToList();

            var dbStaffs = await _context.NhanViens.Where(n => n.ChucVu.Contains("Hướng dẫn viên") || n.ChucVu.Contains("HDV")).ToListAsync();
            if (!dbStaffs.Any())
            {
                dbStaffs = await _context.NhanViens.ToListAsync();
            }
            guidesList = dbStaffs.Select(n => new SelectListItem
            {
                Value = n.MaNv.ToString(),
                Text = $"{n.HoTen} ({n.ChucVu})"
            }).ToList();

            // Tải danh sách Nhân viên Điều hành (OperatorID trong Class Diagram)
            var dbOperators = await _context.NhanViens
                .Where(n => n.ChucVu.Contains("Điều hành") || n.ChucVu.Contains("Operator"))
                .ToListAsync();
            if (!dbOperators.Any())
            {
                dbOperators = await _context.NhanViens.ToListAsync();
            }
            operatorsList = dbOperators.Select(n => new SelectListItem
            {
                Value = n.MaNv.ToString(),
                Text = $"{n.HoTen} ({n.ChucVu})"
            }).ToList();
        }
        catch
        {
            // Bỏ qua lỗi DB và nạp mock items bên dưới
        }

        if (!toursList.Any())
        {
            toursList.Add(new SelectListItem { Value = "1", Text = "Phú Quốc Đảo Ngọc 3 Ngày 2 Đêm" });
            toursList.Add(new SelectListItem { Value = "2", Text = "Hành Trình Khám Phá Đà Lạt Mộng Mơ" });
            toursList.Add(new SelectListItem { Value = "3", Text = "Vòng Cung Tây Bắc - Mùa Lúa Chín Sapa" });
            toursList.Add(new SelectListItem { Value = "4", Text = "Đà Nẵng - Hội An - Huế Hành Trình Di Sản" });
        }

        if (!guidesList.Any())
        {
            guidesList.Add(new SelectListItem { Value = "1", Text = "Nguyễn Văn Hùng (HDV Nội địa)" });
            guidesList.Add(new SelectListItem { Value = "2", Text = "Lê Thị Thu Thảo (HDV Quốc tế)" });
            guidesList.Add(new SelectListItem { Value = "3", Text = "Trần Thanh Sơn (HDV Leo núi)" });
            guidesList.Add(new SelectListItem { Value = "4", Text = "Phạm Hoàng Long (HDV Văn hóa)" });
        }

        if (!operatorsList.Any())
        {
            operatorsList.Add(new SelectListItem { Value = "1", Text = "Trần Văn Nam (Nhân viên Điều hành)" });
            operatorsList.Add(new SelectListItem { Value = "2", Text = "Hoàng Thị Mai (Điều hành tour)" });
        }

        model.Tours = toursList;
        model.Guides = guidesList;
        model.Operators = operatorsList;
    }
}
