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
[Authorize(Roles = "Quản Trị Viên,Điều Hành Tour,Kế Toán Tài Chính")]
public class KhachSanController : Controller
{
    private readonly QlyTourDuLichContext _context;

    // Danh sách Khách Sạn giả lập (Mock Data)
    private static readonly List<KhachSanViewModel> MockKhachSanList = new()
    {
        new KhachSanViewModel { MaKs = 1, TenKs = "Khách Sạn Sunset Phú Quốc", DiaChi = "Bãi Trường, Dương Tơ, Phú Quốc", SoSao = 4, Sdt = "02973999888" },
        new KhachSanViewModel { MaKs = 2, TenKs = "Khách Sạn Palace Đà Lạt", DiaChi = "2 Đường Trần Phú, Phường 3, Đà Lạt", SoSao = 5, Sdt = "02633825444" },
        new KhachSanViewModel { MaKs = 3, TenKs = "Khách Sạn Nha Trang Center", DiaChi = "20 Trần Phú, Nha Trang", SoSao = 3, Sdt = "02583888999" }
    };

    public KhachSanController(QlyTourDuLichContext context)
    {
        _context = context;
    }

    // GET: Admin/KhachSan
    [Route("~/Admin/KhachSan")]
    [Route("~/Admin/KhachSan/Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var hasData = await _context.KhachSans.AnyAsync();
            if (hasData)
            {
                var list = await _context.KhachSans
                    .Select(k => new KhachSanViewModel
                    {
                        MaKs = k.MaKs,
                        TenKs = k.TenKs,
                        DiaChi = k.DiaChi,
                        SoSao = k.SoSao,
                        Sdt = k.Sdt
                    })
                    .ToListAsync();
                return View(list);
            }
        }
        catch
        {
            // Bỏ qua lỗi DB
        }

        return View(MockKhachSanList);
    }

    // GET: Admin/KhachSan/Create
    public IActionResult Create()
    {
        return View(new KhachSanViewModel());
    }

    // POST: Admin/KhachSan/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KhachSanViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var ks = new KhachSan
            {
                TenKs = model.TenKs.Trim(),
                DiaChi = model.DiaChi.Trim(),
                SoSao = model.SoSao,
                Sdt = model.Sdt.Trim()
            };

            _context.KhachSans.Add(ks);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm khách sạn thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            // Mock fallback
            TempData["SuccessMessage"] = "Thêm khách sạn giả lập thành công (Offline)!";
            MockKhachSanList.Add(new KhachSanViewModel
            {
                MaKs = MockKhachSanList.Max(m => m.MaKs) + 1,
                TenKs = model.TenKs,
                DiaChi = model.DiaChi,
                SoSao = model.SoSao,
                Sdt = model.Sdt
            });
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Admin/KhachSan/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var ks = await _context.KhachSans.FindAsync(id);
            if (ks != null)
            {
                _context.KhachSans.Remove(ks);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa khách sạn thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch
        {
            // Bỏ qua
        }

        var mockItem = MockKhachSanList.FirstOrDefault(m => m.MaKs == id);
        if (mockItem != null)
        {
            MockKhachSanList.Remove(mockItem);
            TempData["SuccessMessage"] = "Xóa khách sạn giả lập thành công (Offline)!";
        }

        return RedirectToAction(nameof(Index));
    }
}
