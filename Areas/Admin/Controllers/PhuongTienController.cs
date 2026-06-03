using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTourDuLich.Models;
using QuanLyTourDuLich.ViewModels;

namespace QuanLyTourDuLich.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]/[action]/{id?}")]
[Authorize(Roles = "Quản Trị Viên,Điều Hành Tour")]
public class PhuongTienController : Controller
{
    private readonly QlyTourDuLichContext _context;

    // Danh sách Phương Tiện giả lập (Mock Data)
    private static readonly List<PhuongTienViewModel> MockPhuongTienList = new()
    {
        new PhuongTienViewModel { MaPhuongTien = 1, BienSoXe = "51B-12345", LoaiXe = "Hyundai Universe", SoChoNgoi = 45, TrangThai = "Hoạt động" },
        new PhuongTienViewModel { MaPhuongTien = 2, BienSoXe = "29B-99999", LoaiXe = "Thaco Bluesky 120s", SoChoNgoi = 45, TrangThai = "Hoạt động" },
        new PhuongTienViewModel { MaPhuongTien = 3, BienSoXe = "51A-45678", LoaiXe = "Ford Transit Limousine", SoChoNgoi = 16, TrangThai = "Bảo trì" }
    };

    public PhuongTienController(QlyTourDuLichContext context)
    {
        _context = context;
    }

    // GET: Admin/PhuongTien
    [Route("~/Admin/PhuongTien")]
    [Route("~/Admin/PhuongTien/Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var hasData = await _context.PhuongTiens.AnyAsync();
            if (hasData)
            {
                var list = await _context.PhuongTiens
                    .Select(p => new PhuongTienViewModel
                    {
                        MaPhuongTien = p.MaPhuongTien,
                        BienSoXe = p.BienSoXe,
                        LoaiXe = p.LoaiXe,
                        SoChoNgoi = p.SoChoNgoi,
                        TrangThai = p.TrangThai
                    })
                    .ToListAsync();
                return View(list);
            }
        }
        catch
        {
            // Bỏ qua lỗi DB
        }

        return View(MockPhuongTienList);
    }

    // GET: Admin/PhuongTien/Create
    public IActionResult Create()
    {
        return View(new PhuongTienViewModel());
    }

    // POST: Admin/PhuongTien/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PhuongTienViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var pt = new PhuongTien
            {
                BienSoXe = model.BienSoXe.Trim().ToUpper(),
                LoaiXe = model.LoaiXe.Trim(),
                SoChoNgoi = model.SoChoNgoi,
                TrangThai = model.TrangThai
            };

            _context.PhuongTiens.Add(pt);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký xe mới thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            // Mock fallback
            TempData["SuccessMessage"] = "Thêm xe giả lập thành công (Offline)!";
            MockPhuongTienList.Add(new PhuongTienViewModel
            {
                MaPhuongTien = MockPhuongTienList.Max(m => m.MaPhuongTien) + 1,
                BienSoXe = model.BienSoXe.Trim().ToUpper(),
                LoaiXe = model.LoaiXe,
                SoChoNgoi = model.SoChoNgoi,
                TrangThai = model.TrangThai
            });
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/PhuongTien/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var pt = await _context.PhuongTiens.FindAsync(id);
            if (pt != null)
            {
                _context.PhuongTiens.Remove(pt);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa phương tiện khỏi đội xe thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch
        {
            // Bỏ qua
        }

        var mockItem = MockPhuongTienList.FirstOrDefault(m => m.MaPhuongTien == id);
        if (mockItem != null)
        {
            MockPhuongTienList.Remove(mockItem);
            TempData["SuccessMessage"] = "Xóa xe giả lập thành công (Offline)!";
        }

        return RedirectToAction(nameof(Index));
    }
}
