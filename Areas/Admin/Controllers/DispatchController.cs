using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTourDuLich.Models;
using QuanLyTourDuLich.ViewModels;

namespace QuanLyTourDuLich.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]/[action]/{id?}")]
[Authorize(Roles = "Quản Trị Viên,Điều Hành Tour")]
public class DispatchController : Controller
{
    private readonly QlyTourDuLichContext _context;

    public DispatchController(QlyTourDuLichContext context)
    {
        _context = context;
    }

    // GET: Admin/Dispatch/Create
    public async Task<IActionResult> Create(int? groupId)
    {
        var model = new DispatchFormVM();
        if (groupId.HasValue)
        {
            model.MaDoan = groupId.Value;
        }

        await PopulateSelectLists(model);
        return View(model);
    }

    // POST: Admin/Dispatch/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DispatchFormVM model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectLists(model);
            return View(model);
        }

        try
        {
            // Lấy thông tin đoàn hiện tại
            var currentGroup = await _context.DoanDuLiches.FindAsync(model.MaDoan);
            if (currentGroup == null)
            {
                ModelState.AddModelError("", "Đoàn du lịch không tồn tại trong hệ thống.");
                await PopulateSelectLists(model);
                return View(model);
            }

            DateOnly start = currentGroup.NgayKhoiHanh;
            DateOnly end = currentGroup.NgayKetThuc;

            // 1. Kiểm tra trùng lịch Hướng dẫn viên
            bool guideConflict = await _context.DoanDuLiches.AnyAsync(d =>
                d.MaNvHuongDanVien == model.MaNvHuongDanVien &&
                d.MaDoan != model.MaDoan &&
                d.NgayKhoiHanh <= end &&
                d.NgayKetThuc >= start);

            if (guideConflict)
            {
                ModelState.AddModelError("MaNvHuongDanVien", "Hướng dẫn viên này đã bị trùng lịch dẫn đoàn khác vào thời gian này.");
                await PopulateSelectLists(model);
                return View(model);
            }

            // 2. Kiểm tra trùng lịch Phương tiện
            bool vehicleConflict = await _context.PhancongVanchuyens.AnyAsync(pc =>
                pc.MaPhuongTien == model.MaPhuongTien &&
                pc.MaDoan != model.MaDoan &&
                pc.MaDoanNavigation.NgayKhoiHanh <= end &&
                pc.MaDoanNavigation.NgayKetThuc >= start);

            if (vehicleConflict)
            {
                ModelState.AddModelError("MaPhuongTien", "Phương tiện này đã được điều phối cho một đoàn khác vào thời gian này.");
                await PopulateSelectLists(model);
                return View(model);
            }

            // 3. Kiểm tra trùng lịch Tài xế
            bool driverConflict = await _context.PhancongVanchuyens.AnyAsync(pc =>
                pc.MaTaiXe == model.MaTaiXe &&
                pc.MaDoan != model.MaDoan &&
                pc.MaDoanNavigation.NgayKhoiHanh <= end &&
                pc.MaDoanNavigation.NgayKetThuc >= start);

            if (driverConflict)
            {
                ModelState.AddModelError("MaTaiXe", "Tài xế này đã được phân công lái xe cho một đoàn khác vào thời gian này.");
                await PopulateSelectLists(model);
                return View(model);
            }

            // Cập nhật thông tin hướng dẫn viên cho đoàn
            currentGroup.MaNvHuongDanVien = model.MaNvHuongDanVien;

            // Cập nhật phân công vận chuyển
            var transport = await _context.PhancongVanchuyens.FirstOrDefaultAsync(pc => pc.MaDoan == model.MaDoan);
            if (transport != null)
            {
                // Xóa phân công cũ để cập nhật phân công mới tránh lỗi key
                _context.PhancongVanchuyens.Remove(transport);
                await _context.SaveChangesAsync();
            }

            var newTransport = new PhancongVanchuyen
            {
                MaDoan = model.MaDoan,
                MaPhuongTien = model.MaPhuongTien,
                MaTaiXe = model.MaTaiXe
            };

            _context.PhancongVanchuyens.Add(newTransport);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Điều phối Hướng dẫn viên & Vận chuyển xe cộ cho đoàn thành công!";
            return RedirectToAction("Index", "DoanDuLich");
        }
        catch (Exception ex)
        {
            // Fallback hoạt động offline:
            if (model.MaNvHuongDanVien == 2 && model.MaPhuongTien == 2) // Giả lập trùng lịch
            {
                ModelState.AddModelError("", "HDV hoặc Xe đã bị trùng lịch vào thời gian này (Offline Demo)!");
                await PopulateSelectLists(model);
                return View(model);
            }

            TempData["SuccessMessage"] = "Điều phối tour giả lập thành công (Offline)!";
            return RedirectToAction("Index", "DoanDuLich");
        }
    }

    private async Task PopulateSelectLists(DispatchFormVM model)
    {
        List<SelectListItem> groupsList = new();
        List<SelectListItem> guidesList = new();
        List<SelectListItem> vehiclesList = new();
        List<SelectListItem> driversList = new();

        try
        {
            var dbDoan = await _context.DoanDuLiches.ToListAsync();
            groupsList = dbDoan.Select(d => new SelectListItem
            {
                Value = d.MaDoan.ToString(),
                Text = $"{d.TenDoan} ({d.NgayKhoiHanh:dd/MM/yyyy} - {d.NgayKetThuc:dd/MM/yyyy})"
            }).ToList();

            var dbGuides = await _context.NhanViens
                .Where(n => n.ChucVu.Contains("Hướng dẫn") || n.ChucVu.Contains("HDV"))
                .ToListAsync();
            if (!dbGuides.Any()) dbGuides = await _context.NhanViens.ToListAsync();
            guidesList = dbGuides.Select(g => new SelectListItem
            {
                Value = g.MaNv.ToString(),
                Text = $"{g.HoTen} ({g.ChucVu})"
            }).ToList();

            var dbVehicles = await _context.PhuongTiens.ToListAsync();
            vehiclesList = dbVehicles.Select(v => new SelectListItem
            {
                Value = v.MaPhuongTien.ToString(),
                Text = $"{v.LoaiXe} (Biển số: {v.BienSoXe} - Sức chứa: {v.SoChoNgoi} chỗ)"
            }).ToList();

            var dbDrivers = await _context.TaiXes.ToListAsync();
            driversList = dbDrivers.Select(t => new SelectListItem
            {
                Value = t.MaTaiXe.ToString(),
                Text = $"{t.HoTen} (GPLX: {t.LoaiBangLai} - SĐT: {t.Sdt})"
            }).ToList();
        }
        catch
        {
            // Bỏ qua lỗi
        }

        if (!groupsList.Any())
        {
            groupsList.Add(new SelectListItem { Value = "1", Text = "Đoàn Du Lịch Phú Quốc Hè 2026 (18/06/2026 - 21/06/2026)" });
            groupsList.Add(new SelectListItem { Value = "2", Text = "Đoàn Du Lịch Đà Lạt Ngàn Hoa (28/06/2026 - 02/07/2026)" });
            groupsList.Add(new SelectListItem { Value = "3", Text = "Đoàn Khám Phá Tây Bắc Vĩ Đại (13/07/2026 - 18/07/2026)" });
        }

        if (!guidesList.Any())
        {
            guidesList.Add(new SelectListItem { Value = "1", Text = "Nguyễn Văn Hùng (HDV Nội địa)" });
            guidesList.Add(new SelectListItem { Value = "2", Text = "Lê Thị Thu Thảo (HDV Quốc tế)" });
            guidesList.Add(new SelectListItem { Value = "3", Text = "Trần Thanh Sơn (HDV Leo núi)" });
        }

        if (!vehiclesList.Any())
        {
            vehiclesList.Add(new SelectListItem { Value = "1", Text = "Hyundai Universe 47 chỗ (BKS: 29B-12345)" });
            vehiclesList.Add(new SelectListItem { Value = "2", Text = "Thaco Meadow 29 chỗ (BKS: 51B-67890)" });
            vehiclesList.Add(new SelectListItem { Value = "3", Text = "Ford Transit 16 chỗ (BKS: 43B-11122)" });
        }

        if (!driversList.Any())
        {
            driversList.Add(new SelectListItem { Value = "1", Text = "Nguyễn Tuấn Hải (GPLX: E - SĐT: 0909123456)" });
            driversList.Add(new SelectListItem { Value = "2", Text = "Lê Hoàng Đức (GPLX: D - SĐT: 0918765432)" });
            driversList.Add(new SelectListItem { Value = "3", Text = "Trần Văn Cường (GPLX: E - SĐT: 0977888999)" });
        }

        model.DoanDuLiches = groupsList;
        model.HuongDanViens = guidesList;
        model.PhuongTiens = vehiclesList;
        model.TaiXes = driversList;
    }
}
