using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTourDuLich.Models;
using QuanLyTourDuLich.Services;
using QuanLyTourDuLich.ViewModels;

namespace QuanLyTourDuLich.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]/[action]/{id?}")]
[Authorize(Roles = "Quản trị,Nhân viên,Hướng dẫn viên")]
public class TourController : Controller
{
    private readonly QlyTourDuLichContext _context;
    private readonly ITourBookingService _service;
    private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _webHostEnvironment;

    public TourController(QlyTourDuLichContext context, ITourBookingService service, Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _service = service;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: Admin/Tour or Admin/Tour/Index
    [Route("~/Admin/Tour")]
    [Route("~/Admin/Tour/Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var tours = await _context.Tours
                .Select(t => new TourViewModel
                {
                    MaTour = t.MaTour,
                    TenTour = t.TenTour,
                    DiaDiem = t.DiaDiem,
                    ThoiGian = t.ThoiGian,
                    PhuongTienChinh = t.PhuongTienChinh,
                    MoTa = t.MoTa,
                    GiaThamKhao = t.GiaThamKhao
                })
                .ToListAsync();

            return View(tours);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi lấy danh sách tour: " + ex.Message;
            return View(new List<TourViewModel>());
        }
    }

    // GET: Admin/Tour/Create
    public IActionResult Create()
    {
        return View(new TourViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TourViewModel model)
    {
        // BƯỚC 1 (Nhận Request): Action Create [HttpPost] nhận TourViewModel từ giao diện.

        // BƯỚC 2 (Validation cơ bản): Kiểm tra ModelState.IsValid. Nếu false, lập tức return View(model).
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // BƯỚC 3 (Check Logic DB - Activity Diagram Alt):
            bool isDuplicate = await _service.IsTourNameDuplicateAsync(model.TenTour);

            // [NHÁNH LỖI]: Nếu kết quả là true (Đã tồn tại), LẬP TỨC từ chối và thêm lỗi vào ModelState
            if (isDuplicate)
            {
                ModelState.AddModelError("TenTour", "Tên Tour này đã tồn tại trong hệ thống. Vui lòng chọn tên khác.");
                return View(model);
            }

            // Xử lý lưu File hình ảnh lên Server (nếu có) - Khớp 100% Activity Diagram "Thêm mới Tour"
            string? uniqueFileName = null;
            if (model.HinhAnhUpload != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "tours");
                Directory.CreateDirectory(uploadsFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.HinhAnhUpload.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.HinhAnhUpload.CopyToAsync(fileStream);
                }
            }

            // BƯỚC 4 (Xử lý Thành công):
            // [NHÁNH ĐÚNG]: Nếu chưa tồn tại, Map dữ liệu từ TourViewModel sang Entity Tour.
            var tour = new Tour
            {
                TenTour = model.TenTour.Trim(),
                DiaDiem = model.DiaDiem.Trim(),
                ThoiGian = model.ThoiGian,
                PhuongTienChinh = model.PhuongTienChinh,
                MoTa = model.MoTa?.Trim(),
                GiaThamKhao = model.GiaThamKhao
            };

            // Thêm và lưu vào Database
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();

            // BƯỚC 5 (Kết thúc): Sử dụng TempData và Redirect về action Index.
            TempData["Success"] = "Thêm tour và tải lên hình ảnh thành công!";
            TempData["SuccessMessage"] = "Thêm tour và tải lên hình ảnh thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình lưu dữ liệu: " + ex.Message);
            return View(model);
        }
    }

    // GET: Admin/Tour/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var t = await _context.Tours.FindAsync(id);
            if (t == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy Tour có mã #" + id;
                return RedirectToAction(nameof(Index));
            }

            var model = new TourViewModel
            {
                MaTour = t.MaTour,
                TenTour = t.TenTour,
                DiaDiem = t.DiaDiem,
                ThoiGian = t.ThoiGian,
                PhuongTienChinh = t.PhuongTienChinh,
                MoTa = t.MoTa,
                GiaThamKhao = t.GiaThamKhao
            };

            return View(model);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải thông tin chỉnh sửa: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/Tour/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TourViewModel model)
    {
        // A. CHỨC NĂNG CẬP NHẬT (EDIT - POST):
        
        // BƯỚC 1: Nhận TourViewModel và MaTour tương ứng từ UI. Kiểm tra ModelState.IsValid.
        if (id != model.MaTour)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // BƯỚC 2 (Check Logic DB): Kiểm tra trùng lặp Tên Tour NHƯNG phải loại trừ chính Tour đang sửa
            bool isDuplicate = await _service.IsTourNameDuplicateAsync(model.TenTour, id);

            // BƯỚC 3 (Rẽ nhánh ALT):
            // [NHÁNH LỖI]: Nếu true (Tên bị trùng với Tour khác), gán ModelState.AddModelError và return View().
            if (isDuplicate)
            {
                ModelState.AddModelError("TenTour", "Tên Tour du lịch này đã được sử dụng bởi một tour khác trong hệ thống.");
                return View(model);
            }

            // [NHÁNH ĐÚNG]: Nếu hợp lệ, lấy entity cũ, cập nhật dữ liệu từ model sang và await _context.SaveChangesAsync().
            var tourEntity = await _context.Tours.FindAsync(id);
            if (tourEntity == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy Tour tương ứng trong hệ thống.";
                return RedirectToAction(nameof(Index));
            }

            // Xử lý lưu File hình ảnh lên Server (nếu có) - Khớp 100% Activity Diagram "Chỉnh sửa Tour"
            if (model.HinhAnhUpload != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "tours");
                Directory.CreateDirectory(uploadsFolder);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.HinhAnhUpload.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.HinhAnhUpload.CopyToAsync(fileStream);
                }
            }

            tourEntity.TenTour = model.TenTour.Trim();
            tourEntity.DiaDiem = model.DiaDiem.Trim();
            tourEntity.ThoiGian = model.ThoiGian;
            tourEntity.PhuongTienChinh = model.PhuongTienChinh;
            tourEntity.MoTa = model.MoTa?.Trim();
            tourEntity.GiaThamKhao = model.GiaThamKhao;

            _context.Entry(tourEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Redirect về Index báo thành công.
            TempData["Success"] = "Cập nhật tour thành công!";
            TempData["SuccessMessage"] = "Cập nhật tour thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu thông tin cập nhật: " + ex.Message);
            return View(model);
        }
    }

    // POST: Admin/Tour/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        // B. CHỨC NĂNG XÓA (DELETE - POST) - BẮT BUỘC KIỂM TRA KHÓA NGOẠI:

        // BƯỚC 1: Nhận yêu cầu Xóa kèm id (MaTour).

        try
        {
            // BƯỚC 2 (Referential Integrity Check): Trước khi xóa, PHẢI kiểm tra xem Tour này đã có Đoàn Du Lịch nào được mở chưa
            bool hasAssociatedGroup = await _context.DoanDuLiches.AnyAsync(d => d.MaTour == id);

            // BƯỚC 3 (Rẽ nhánh nghiêm ngặt):
            // [NHÁNH BỊ CHẶN]: Nếu kết quả là true (Tức là đã có Đoàn liên kết), BẮT BUỘC CHẶN LỆNH XÓA.
            if (hasAssociatedGroup)
            {
                // Dùng TempData để gán thông báo lỗi và lập tức Redirect về trang Index. Tuyệt đối không gọi lệnh Remove.
                TempData["ErrorMessage"] = "Lỗi: Không thể xóa Tour này vì đã có Đoàn du lịch liên kết trong hệ thống!";
                return RedirectToAction(nameof(Index));
            }

            // [NHÁNH CHO PHÉP]: Nếu false, tiến hành _context.Tours.Remove() và await _context.SaveChangesAsync().
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy Tour cần xóa.";
                return RedirectToAction(nameof(Index));
            }

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();

            // BƯỚC 4: Redirect về trang Index báo xóa thành công.
            TempData["Success"] = "Xóa tour thành công!";
            TempData["SuccessMessage"] = "Xóa tour thành công!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Đã xảy ra lỗi không mong muốn khi xóa tour: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
